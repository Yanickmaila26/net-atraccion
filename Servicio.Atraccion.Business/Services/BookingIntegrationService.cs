using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Servicio.Atraccion.Business.DTOs.Booking;
using Servicio.Atraccion.Business.Interfaces;
using QuestPDF.Fluent;

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
    private readonly IBillingService _billingService;
    private readonly IUnitOfWork _uow;
    private readonly IConfiguration _configuration;

    public BookingIntegrationService(
        IAttractionDataService attractionData,
        IInventoryDataService inventoryData,
        IBillingService billingService,
        IUnitOfWork uow,
        IConfiguration configuration)
    {
        _attractionData = attractionData;
        _inventoryData = inventoryData;
        _billingService = billingService;
        _uow = uow;
        _configuration = configuration;
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
                .ThenInclude(po => po.Attraction)
            .Where(s =>
                s.ProductOption.AttractionId == attractionId &&
                s.ProductOption.Attraction.DeletedAt == null &&
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
    // TRANSACCIONES: CREAR RESERVA
    // ══════════════════════════════════════════════════

    public async Task<ApiResponse<AtraccionBookingResponseDto>> CrearReservaAsync(AtraccionBookingRequestDto request, Guid userId)
    {
        // 1. Obtener el slot y validar capacidad
        var slot = await _uow.AvailabilitySlots.Query()
            .Include(s => s.ProductOption)
                .ThenInclude(po => po.Attraction)
            .Include(s => s.ProductOption)
                .ThenInclude(po => po.PriceTiers)
            .FirstOrDefaultAsync(s => s.Id == request.SlotId && s.IsActive);

        if (slot == null)
            return ApiResponse<AtraccionBookingResponseDto>.Fail("El horario seleccionado ya no está disponible.");

        int totalTickets = request.Tickets!.Count;
        if (slot.CapacityAvailable < totalTickets)
            return ApiResponse<AtraccionBookingResponseDto>.Fail($"No hay cupos suficientes. Cupos restantes: {slot.CapacityAvailable}");

        // 2. Calcular montos
        decimal totalAmount = 0;
        var details = new List<DataAccess.Entities.BookingDetail>();
        
        // Obtener moneda por defecto del producto
        string currency = slot.ProductOption.PriceTiers.FirstOrDefault(pt => pt.IsActive)?.CurrencyCode ?? "USD";

        foreach (var t in request.Tickets)
        {
            // Buscamos el precio para esta categoría en este producto usando PriceTierId o TicketCategoryId
            var priceTier = await _uow.PriceTiers.Query()
                .FirstOrDefaultAsync(pt => 
                    pt.ProductId == slot.ProductId && 
                    ((t.PriceTierId != null && pt.Id == t.PriceTierId) || 
                     (t.PriceTierId == null && pt.TicketCategoryId == t.TicketCategoryId)) && 
                    pt.IsActive);

            if (priceTier == null)
                return ApiResponse<AtraccionBookingResponseDto>.Fail("Una de las categorías de ticket seleccionadas no es válida para esta opción.");

            totalAmount += priceTier.Price;

            details.Add(new DataAccess.Entities.BookingDetail
            {
                Id = Guid.NewGuid(),
                PriceTierId = priceTier.Id,
                FirstName = t.FirstName,
                LastName = t.LastName,
                DocumentNumber = t.DocumentNumber,
                DocumentType = t.DocumentType,
                Quantity = 1, // Por simplicidad, cada ticket es una fila de detalle
                UnitPrice = priceTier.Price
            });
        }

        // Aplicar IVA al total con fallback seguro
        decimal taxRate = 0.15m;
        try {
            var configValue = _configuration["Billing:TaxRate"];
            if (!string.IsNullOrEmpty(configValue)) {
                taxRate = decimal.Parse(configValue, System.Globalization.CultureInfo.InvariantCulture);
            }
        } catch { /* Usar default 0.15 */ }
        
        totalAmount = totalAmount * (1 + taxRate);

        // 3. Crear la cabecera de la reserva
        var booking = new DataAccess.Entities.Booking
        {
            Id = Guid.NewGuid(),
            PnrCode = GeneratePnrCode(),
            UserId = userId,
            SlotId = slot.Id,
            StatusId = 2, // Confirmed (asumimos pago o reserva inmediata para este flujo)
            TotalAmount = Math.Round(totalAmount, 2),
            CurrencyCode = currency,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // 4. Ejecutar cambios en la DB (Transacción lógica en UoW)
        try 
        {
            await _uow.Bookings.AddAsync(booking);
            
            foreach (var detail in details)
            {
                detail.BookingId = booking.Id;
                await _uow.BookingDetails.AddAsync(detail);
            }

            // RESTAR CUPO
            slot.CapacityAvailable -= (short)totalTickets;
            slot.UpdatedAt = DateTime.UtcNow;

            // GENERAR FACTURA (A través del servicio de Billing)
            await _billingService.CrearFacturaAsync(booking, request.Billing, details);

            await _uow.CompleteAsync();

            return ApiResponse<AtraccionBookingResponseDto>.Ok(new AtraccionBookingResponseDto
            {
                BookingId = booking.Id,
                PnrCode = booking.PnrCode,
                Status = "Confirmed",
                TotalAmount = totalAmount,
                Currency = booking.CurrencyCode,
                ActivityDate = slot.SlotDate.ToDateTime(slot.StartTime),
                AttractionName = slot.ProductOption.Attraction.Name
            }, "Reserva creada exitosamente.");
        }
        catch (Exception ex)
        {
            var errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            return ApiResponse<AtraccionBookingResponseDto>.Fail("Error de BD/Interno al procesar la reserva: " + errorMsg);
        }
    }



    // ══════════════════════════════════════════════════
    // TRANSACCIONES: CANCELAR RESERVA
    // ══════════════════════════════════════════════════

    public async Task<ApiResponse<bool>> CancelarReservaAsync(Guid bookingId, Guid userId)
    {
        var booking = await _uow.Bookings.Query()
            .Include(b => b.AvailabilitySlot)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

        if (booking == null)
            return ApiResponse<bool>.Fail("Reserva no encontrada.");

        if (booking.StatusId == 3) // Cancelled
            return ApiResponse<bool>.Fail("La reserva ya se encuentra cancelada.");

        // 1. Cambiar estado
        booking.StatusId = 3; // Cancelled
        booking.CancelledAt = DateTime.UtcNow;
        booking.UpdatedAt = DateTime.UtcNow;

        // 2. Devolver cupos
        var totalTickets = await _uow.BookingDetails.Query()
            .Where(d => d.BookingId == bookingId)
            .SumAsync(d => d.Quantity);
        booking.AvailabilitySlot.CapacityAvailable += (short)totalTickets;
        booking.AvailabilitySlot.UpdatedAt = DateTime.UtcNow;

        // 3. Anular Factura
        await _billingService.CancelarFacturaAsync(bookingId);

        await _uow.CompleteAsync();

        return ApiResponse<bool>.Ok(true, "Reserva cancelada, cupos liberados y factura marcada para anulación.");
    }

    // ══════════════════════════════════════════════════
    // LISTAR MIS RESERVAS
    // ══════════════════════════════════════════════════

    public async Task<ApiResponse<List<AtraccionBookingResponseDto>>> ListarMisReservasAsync(Guid userId)
    {
        var bookings = await _uow.Bookings.Query()
            .Include(b => b.AvailabilitySlot)
                .ThenInclude(s => s.ProductOption)
                    .ThenInclude(po => po.Attraction)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        var dtos = bookings.Select(b => new AtraccionBookingResponseDto
        {
            BookingId = b.Id,
            PnrCode = b.PnrCode,
            Status = b.StatusId switch { 1 => "Pending", 2 => "Confirmed", 3 => "Cancelled", _ => "Unknown" },
            TotalAmount = b.TotalAmount,
            Currency = b.CurrencyCode,
            ActivityDate = b.AvailabilitySlot.SlotDate.ToDateTime(b.AvailabilitySlot.StartTime),
            AttractionName = b.AvailabilitySlot.ProductOption.Attraction.Name
        }).ToList();

        return ApiResponse<List<AtraccionBookingResponseDto>>.Ok(dtos);
    }

    public async Task<byte[]?> GenerarPdfFacturaAsync(Guid bookingId)
    {
        var invoice = await _uow.Invoices.Query()
            .Include(i => i.Details)
            .FirstOrDefaultAsync(i => i.BookingId == bookingId);

        if (invoice == null) return null;

        var document = new Templates.InvoiceTemplate(invoice);
        return document.GeneratePdf();
    }

    private string GeneratePnrCode()
    {
        return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
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
                a.IsPublished &&
                a.DeletedAt == null);

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
