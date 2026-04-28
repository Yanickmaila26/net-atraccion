using Servicio.Atraccion.Business.DTOs.Schedule;

namespace Servicio.Atraccion.Business.DTOs.Inventory;

/// <summary>Modalidad/producto de una atracción con sus precios.</summary>
public class ProductResponse
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? DurationMinutes { get; set; }
    public string? DurationDescription { get; set; }
    public int CancelPolicyHours { get; set; }
    public string? CancelPolicyText { get; set; }
    public short? MaxGroupSize { get; set; }
    public short MinParticipants { get; set; }
    public bool IsPrivate { get; set; }
    public List<PriceTierResponse> PriceTiers { get; set; } = [];
    public List<Servicio.Atraccion.Business.DTOs.Schedule.ScheduleTemplateResponse> ScheduleTemplates { get; set; } = [];
}

public class PriceTierResponse
{
    public Guid Id { get; set; }
    public Guid TicketCategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string CurrencyCode { get; set; } = "USD";
}

/// <summary>Slot de disponibilidad en el calendario.</summary>
public class AvailabilitySlotResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public DateOnly SlotDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public short CapacityTotal { get; set; }
    public short CapacityAvailable { get; set; }
    public bool IsActive { get; set; }
    /// <summary>Indica si el slot tiene al menos una reserva activa (no cancelada). No se puede eliminar si es true.</summary>
    public bool HasBookings { get; set; }
    public string? Notes { get; set; }
}

/// <summary>Petición de consulta de disponibilidad.</summary>
public class AvailabilityRequest
{
    public Guid AttractionId { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime EndDate { get; set; } = DateTime.UtcNow.AddDays(30);
}
