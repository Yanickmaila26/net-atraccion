using Microsoft.EntityFrameworkCore;
using Servicio.Atraccion.Business.DTOs.Booking;
using Servicio.Atraccion.Business.Interfaces;
using Servicio.Atraccion.DataAccess.Repositories.Interfaces;
using Servicio.Atraccion.DataManagement.Interfaces;

namespace Servicio.Atraccion.Business.Services;

/// <summary>
/// Implementación del servicio de integración con el sistema central de Booking.
/// Convierte los datos internos de la plataforma de atracciones al formato estándar del contrato.
/// </summary>
public class BookingIntegrationService : IBookingIntegrationService
{
    private readonly IAttractionDataService _attractionData;
    private readonly IInventoryDataService _inventoryData;
    private readonly IUnitOfWork _uow;

    public BookingIntegrationService(
        IAttractionDataService attractionData,
        IInventoryDataService inventoryData,
        IUnitOfWork uow)
    {
        _attractionData = attractionData;
        _inventoryData = inventoryData;
        _uow = uow;
    }

    // ══════════════════════════════════════════════════
    // LISTAR ATRACCIONES
    // ══════════════════════════════════════════════════

    public async Task<PagedApiResponse<AtraccionBookingDto>> ListarAtraccionesAsync(AtraccionSearchBookingRequest request)
    {
        // 1. Construir filtros internos
        var filters = new DataAccess.Common.AttractionQueryFilters
        {
            SearchTerm = request.Search ?? string.Empty,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            IsPublished = true,
            IsActive = true
        };

        // Filtro por ubicación (por nombre de ciudad/provincia)
        // En versiones futuras se puede agregar LocationId directo
        // if (request.Ubicacion != null) → resolver a LocationId mediante geocoding

        var paged = await _attractionData.SearchAttractionsAsync(filters);

        // 2. Enriquecer cada atracción con sus modalidades y tarifas
        var dtos = new List<AtraccionBookingDto>();

        foreach (var node in paged.Items)
        {
            var dto = await MapToBookingDtoAsync(node.Id);
            if (dto == null) continue;

            // Filtro de disponibilidad: si se pide, verificar que tenga slots futuros
            if (request.Disponible == true && !dto.Disponible) continue;

            // Filtro de rango de precios sobre precio
            if (request.PrecioMinimo.HasValue && dto.Precio < request.PrecioMinimo.Value) continue;
            if (request.PrecioMaximo.HasValue && dto.Precio > request.PrecioMaximo.Value) continue;

            dtos.Add(dto);
        }

        return PagedApiResponse<AtraccionBookingDto>.Ok(dtos, paged.TotalCount, request.Page, request.PageSize);
    }

    // ══════════════════════════════════════════════════
    // OBTENER ATRACCIÓN POR ID
    // ══════════════════════════════════════════════════

    public async Task<ApiResponse<AtraccionBookingDto>> ObtenerAtraccionAsync(Guid id)
    {
        var dto = await MapToBookingDtoAsync(id);

        if (dto == null)
            return ApiResponse<AtraccionBookingDto>.Fail("Atracción no encontrada o no está disponible públicamente.");

        return ApiResponse<AtraccionBookingDto>.Ok(dto);
    }

    // ══════════════════════════════════════════════════
    // DISPONIBILIDAD AGRUPADA POR DÍA
    // ══════════════════════════════════════════════════

    public async Task<ApiResponse<List<DisponibilidadDiariaDto>>> ObtenerDisponibilidadAsync(Guid attractionId, DateOnly? fecha = null)
    {
        // Determinar rango de fechas
        var fechaInicio = fecha ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var fechaFin = fecha.HasValue
            ? fecha.Value                          // Solo ese día específico
            : DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)); // Próximos 30 días

        // Consultar todos los slots activos de todos los productos de esta atracción
        // en el rango de fechas pedido
        var slots = await _uow.AvailabilitySlots.Query()
            .Include(s => s.ProductOption)
            .Where(s =>
                s.ProductOption.AttractionId == attractionId &&
                s.IsActive &&
                s.SlotDate >= fechaInicio &&
                s.SlotDate <= fechaFin &&
                s.CapacityAvailable > 0)
            .OrderBy(s => s.SlotDate)
            .ThenBy(s => s.StartTime)
            .ToListAsync();

        // Agrupar por fecha y sumar cupos (compatible con disponibilidad_productos de Booking)
        var disponibilidadPorDia = slots
            .GroupBy(s => s.SlotDate)
            .Select(g => new DisponibilidadDiariaDto
            {
                Fecha = g.Key.ToString("yyyy-MM-dd"),
                CuposDisponibles = g.Sum(s => s.CapacityAvailable),  // Suma total del día
                Horarios = g.Select(s => new HorarioDto
                {
                    SlotId = s.Id,
                    HoraInicio = s.StartTime.ToString(@"HH\:mm"),
                    HoraFin = s.EndTime?.ToString(@"HH\:mm"),
                    CuposDisponibles = s.CapacityAvailable,
                    CuposTotales = s.CapacityTotal
                }).ToList()
            })
            .ToList();

        return ApiResponse<List<DisponibilidadDiariaDto>>.Ok(disponibilidadPorDia);
    }

    // ══════════════════════════════════════════════════
    // MAPPER CENTRAL PRIVADO
    // ══════════════════════════════════════════════════

    /// <summary>
    /// Construye el DTO de integración completo para una atracción,
    /// incluyendo modalidades, tarifas con rango de edad, y precio mínimo.
    /// </summary>
    private async Task<AtraccionBookingDto?> MapToBookingDtoAsync(Guid attractionId)
    {
        // Cargar la atracción completa con sus productos
        var attraction = await _uow.Attractions.Query()
            .Include(a => a.Location)
            .Include(a => a.Subcategory)
                .ThenInclude(s => s!.Category)
            .Include(a => a.Media)
            .Include(a => a.ProductOptions.Where(po => po.IsActive))
                .ThenInclude(po => po.PriceTiers.Where(pt => pt.IsActive))
                    .ThenInclude(pt => pt.TicketCategory)
            .FirstOrDefaultAsync(a =>
                a.Id == attractionId &&
                a.IsActive &&
                a.IsPublished);

        if (attraction == null) return null;

        // Imagen principal
        var mainImage = attraction.Media
            .Where(m => m.IsMain)
            .OrderBy(m => m.SortOrder)
            .FirstOrDefault()?.Url
            ?? attraction.Media.FirstOrDefault()?.Url;

        // En lugar de exponer todas las tarifas, calculamos el precio mínimo desde los PriceTiers activos
        var precio = attraction.ProductOptions
            .SelectMany(po => po.PriceTiers)
            .Where(pt => pt.IsActive)
            .Select(t => t.Price)
            .DefaultIfEmpty(0)
            .Min();

        // Moneda dominante (la del price_tier más bajo, por consistencia)
        var moneda = attraction.ProductOptions
            .SelectMany(po => po.PriceTiers)
            .Where(pt => pt.IsActive)
            .OrderBy(t => t.Price)
            .FirstOrDefault()?.CurrencyCode ?? "USD";

        // Verificar si tiene disponibilidad futura activa
        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
        var tieneDisponibilidad = await _uow.AvailabilitySlots.Query()
            .AnyAsync(s =>
                s.ProductOption.AttractionId == attractionId &&
                s.IsActive &&
                s.SlotDate >= hoy &&
                s.CapacityAvailable > 0);

        // Construir ubicación legible
        var ubicacion = BuildUbicacion(attraction.Location);

        return new AtraccionBookingDto
        {
            Id = attraction.Id,
            Nombre = attraction.Name,
            Descripcion = attraction.DescriptionShort ?? attraction.DescriptionFull ?? string.Empty,
            Precio = precio,
            Moneda = moneda,
            Ubicacion = ubicacion,
            ImagenUrl = mainImage,
            Disponible = tieneDisponibilidad
        };
    }

    /// <summary>
    /// Construye una cadena de ubicación legible navegando la jerarquía de Location.
    /// </summary>
    private static string BuildUbicacion(DataAccess.Entities.Location? location)
    {
        if (location == null) return string.Empty;

        // Si es una ciudad, su padre sería provincia/estado y el abuelo el país
        return location.Type?.ToLower() switch
        {
            "city" => location.Name,
            "state" => location.Name,
            "country" => location.Name,
            _ => location.Name
        };
    }
}
