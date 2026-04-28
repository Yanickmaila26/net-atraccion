using FluentValidation;
using Servicio.Atraccion.Business.DTOs.Booking;
using Servicio.Atraccion.Business.Exceptions;
using Servicio.Atraccion.Business.Interfaces;
using Servicio.Atraccion.DataAccess.Common;
using Servicio.Atraccion.DataManagement.Interfaces;
using Servicio.Atraccion.DataManagement.Models;

namespace Servicio.Atraccion.Business.Services;

public class BookingService : IBookingService
{
    private readonly IBookingDataService _bookingData;
    private readonly IInventoryDataService _inventoryData;
    private readonly IClientDataService _clientData;
    private readonly IValidator<CreateBookingRequest> _createValidator;
    private readonly IValidator<CancelBookingRequest> _cancelValidator;

    public BookingService(
        IBookingDataService bookingData,
        IInventoryDataService inventoryData,
        IClientDataService clientData,
        IValidator<CreateBookingRequest> createValidator,
        IValidator<CancelBookingRequest> cancelValidator)
    {
        _bookingData = bookingData;
        _inventoryData = inventoryData;
        _clientData = clientData;
        _createValidator = createValidator;
        _cancelValidator = cancelValidator;
    }

    public async Task<BookingConfirmationResponse> CreateBookingAsync(Guid userId, CreateBookingRequest request)
    {
        // ── PASO 1: Validación de formato ─────────────────────────────────────
        var validation = await _createValidator.ValidateAsync(request);
        if (!validation.IsValid)
            throw new Exceptions.ValidationException(validation.Errors.Select(e => e.ErrorMessage).ToList());

        // ── PASO 2: Verificar slot existe y tiene disponibilidad ──────────────
        var slot = await _inventoryData.GetSlotByIdAsync(request.SlotId);
        if (slot == null)
            throw new NotFoundException("Slot de disponibilidad", request.SlotId);

        // Verificar que el slot sea futuro (SlotDate viene como DateTime con hora 00:00)
        if (slot.SlotDate < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new BusinessException("No se puede reservar en un slot de fecha pasada.");

        // Calcular total de pasajeros que se quieren incluir
        short totalPassengers = (short)request.Passengers.Sum(p => p.Quantity);
        if (slot.CapacityAvailable < totalPassengers)
            throw new BusinessException(
                $"No hay suficiente disponibilidad. Disponible: {slot.CapacityAvailable}, Solicitado: {totalPassengers}.");

        // ── PASO 3: Validar y cargar PriceTiers ──────────────────────────────
        var requestedTierIds = request.Passengers.Select(p => p.PriceTierId).Distinct();
        var validTiers = (await _inventoryData.GetPriceTiersByIdsAsync(requestedTierIds))
                         .ToDictionary(t => t.Id);

        foreach (var passenger in request.Passengers)
        {
            if (!validTiers.ContainsKey(passenger.PriceTierId))
                throw new BusinessException(
                    $"El tier de precio '{passenger.PriceTierId}' no es válido o no está activo.");
        }

        // ── PASO 4: Calcular TotalAmount ──────────────────────────────────────
        decimal totalAmount = request.Passengers.Sum(p =>
            validTiers[p.PriceTierId].Price * p.Quantity);

        string currencyCode = validTiers.Values.First().CurrencyCode;

        // ── PASO 5: Construir BookingNode ─────────────────────────────────────
        var bookingNode = new BookingNode
        {
            UserId = userId,
            SlotId = request.SlotId,
            StatusId = 1,  // Pending
            TotalAmount = totalAmount,
            CurrencyCode = currencyCode,
            Notes = request.Notes,
            Details = request.Passengers.Select(p => new BookingDetailNode
            {
                PriceTierId = p.PriceTierId,
                PriceTierLabel = validTiers[p.PriceTierId].CategoryName,
                FirstName = p.FirstName,
                LastName = p.LastName,
                DocumentType = p.DocumentType,
                DocumentNumber = p.DocumentNumber,
                Quantity = p.Quantity,
                UnitPrice = validTiers[p.PriceTierId].Price
            }).ToList()
        };

        // ── PASO 6: Persistir reserva ─────────────────────────────────────────
        var created = await _bookingData.CreateBookingAsync(bookingNode);
        if (created == null)
            throw new BusinessException("No se pudo completar la reserva. Intente nuevamente.");

        // ── PASO 7: Decrementar capacidad del slot ────────────────────────────
        await _inventoryData.DecrementSlotCapacityAsync(request.SlotId, totalPassengers);

        // ── PASO 8: Retornar confirmación ─────────────────────────────────────
        return new BookingConfirmationResponse
        {
            Id = created.Id,
            PnrCode = created.PnrCode,
            StatusName = created.StatusName,
            TotalAmount = created.TotalAmount,
            CurrencyCode = created.CurrencyCode,
            AttractionName = created.AttractionName,
            SlotDate = created.SlotDate,
            SlotStartTime = created.SlotStartTime,
            TotalPassengers = totalPassengers,
            CreatedAt = created.CreatedAt
        };
    }

    public async Task<BookingDetailResponse> GetByPnrAsync(string pnrCode)
    {
        var booking = await _bookingData.GetByPnrAsync(pnrCode.ToUpperInvariant());
        if (booking == null)
            throw new NotFoundException("Reserva", pnrCode);

        return MapToDetail(booking);
    }

    public async Task<BookingDetailResponse> GetByIdAsync(Guid id, Guid currentUserId, bool isAdmin)
    {
        var booking = await _bookingData.GetByIdAsync(id);
        if (booking == null)
            throw new NotFoundException("Reserva", id);

        // Si no es admin, verificar que es el dueño
        if (!isAdmin && booking.UserId != currentUserId)
            throw new UnauthorizedBusinessException("No tienes permiso para ver esta reserva.");

        return MapToDetail(booking);
    }

    public async Task<PagedResult<BookingSummaryResponse>> GetUserHistoryAsync(Guid userId, int page = 1, int pageSize = 10)
    {
        var filters = new QueryFilters { PageNumber = page, PageSize = pageSize };
        var paged = await _bookingData.GetBookingsByUserAsync(userId, filters);

        return new PagedResult<BookingSummaryResponse>
        {
            Items = paged.Items.Select(MapToSummary).ToList(),
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }

    public async Task<PagedResult<BookingSummaryResponse>> SearchManagementAsync(BookingSearchRequest request, Guid userId, bool isAdmin)
    {
        var filter = new BookingQueryFilters
        {
            SearchTerm = request.SearchTerm,
            StatusId = request.StatusId,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        // Si es Partner, solo sus atracciones
        if (!isAdmin)
        {
            filter.ManagedById = userId;
        }

        var paged = await _bookingData.SearchBookingsAsync(filter);

        return new PagedResult<BookingSummaryResponse>
        {
            Items = paged.Items.Select(MapToSummary).ToList(),
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }

    public async Task CancelBookingAsync(Guid userId, bool isAdmin, CancelBookingRequest request)
    {
        // ── PASO 1: Validar formato ───────────────────────────────────────────
        var validation = await _cancelValidator.ValidateAsync(request);
        if (!validation.IsValid)
            throw new Exceptions.ValidationException(validation.Errors.Select(e => e.ErrorMessage).ToList());

        // ── PASO 2: Verificar que la reserva existe y validar permisos ────
        var booking = await _bookingData.GetByPnrAsync(request.PnrCode.ToUpperInvariant());
        if (booking == null)
            throw new NotFoundException("Reserva", request.PnrCode);

        // Si no es admin, debe ser el dueño de la reserva
        if (!isAdmin && booking.UserId != userId)
            throw new UnauthorizedBusinessException("No tienes permiso para cancelar esta reserva.");

        // ── PASO 3: Verificar que se puede cancelar (no ya cancelada/completada)
        if (booking.StatusId == 4) // Cancelled
            throw new BusinessException("La reserva ya está cancelada.");
        if (booking.StatusId == 3) // Completed
            throw new BusinessException("No se puede cancelar una reserva ya completada.");

        // ── PASO 4: Actualizar estado a Cancelada (4) ─────────────────────────
        var ok = await _bookingData.UpdateBookingStatusAsync(booking.Id, statusId: 4, request.CancelReason);
        if (!ok)
            throw new BusinessException("No se pudo cancelar la reserva. Intente nuevamente.");

        // ── PASO 5: Devolver capacidad al slot ────────────────────────────────
        // Revertimos: incrementamos la capacidad restándola con negativo no aplica,
        // necesitamos IncrementSlotCapacity o usar Decrement en negativo.
        // Por simplicidad, decrementamos con valor negativo equivale a incrementar.
        short totalPassengers = (short)booking.Details.Sum(d => d.Quantity);
        await _inventoryData.DecrementSlotCapacityAsync(booking.SlotId, (short)-totalPassengers);
    }

    // ─── MAPPERS PRIVADOS ──────────────────────────────────────────────────────

    private static BookingDetailResponse MapToDetail(BookingNode b) => new()
    {
        Id = b.Id,
        PnrCode = b.PnrCode,
        StatusName = b.StatusName,
        TotalAmount = b.TotalAmount,
        CurrencyCode = b.CurrencyCode,
        Notes = b.Notes,
        CreatedAt = b.CreatedAt,
        AttractionName = b.AttractionName,
        SlotDate = b.SlotDate,
        SlotStartTime = b.SlotStartTime,
        Passengers = b.Details.Select(d => new PassengerDetailResponse
        {
            FullName = $"{d.FirstName} {d.LastName}",
            DocumentNumber = d.DocumentNumber,
            PriceTierLabel = d.PriceTierLabel,
            UnitPrice = d.UnitPrice,
            Quantity = d.Quantity
        }).ToList(),
        CanReview = CalculateCanReview(b)
    };

    private static BookingSummaryResponse MapToSummary(BookingNode b) => new()
{
    Id = b.Id,
    PnrCode = b.PnrCode,
    AttractionName = b.AttractionName,
    AttractionSlug = b.AttractionSlug,
    ProductTitle = b.ProductTitle,
    StatusName = b.StatusName,
    StatusId = b.StatusId,
    TotalAmount = b.TotalAmount,
    CurrencyCode = b.CurrencyCode,
    SlotDate = b.SlotDate,
    SlotStartTime = b.SlotStartTime,
    TotalPassengers = b.Details.Sum(d => d.Quantity),
    CreatedAt = b.CreatedAt,
    ClientName = b.ClientName,
    ClientEmail = b.UserEmail,
    CanReview = CalculateCanReview(b),
    Tickets = b.Details.Select(d => new BookingTicketSummary
    {
        CategoryName = d.PriceTierLabel,
        Quantity = d.Quantity,
        UnitPrice = d.UnitPrice
    }).ToList()
};

    private static bool CalculateCanReview(BookingNode b)
    {
        if (b.StatusId == 3) return true; // Completed
        if (b.StatusId == 2) // Confirmed
        {
            // Combinar fecha y hora para comparar con ahora
            var slotDateTime = b.SlotDate.ToDateTime(b.SlotStartTime);
            return DateTime.UtcNow > slotDateTime;
        }
        return false;
    }
}
