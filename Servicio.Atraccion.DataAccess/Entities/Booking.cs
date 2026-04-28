using Servicio.Atraccion.DataAccess.Common;

namespace Servicio.Atraccion.DataAccess.Entities;

public class Booking : BaseEntity
{
    public string PnrCode { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public Guid SlotId { get; set; }
    public short StatusId { get; set; } = 1;               // 1 = Pending
    public decimal TotalAmount { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public short? LanguageId { get; set; }                 // Idioma preferido para el tour
    public string? Notes { get; set; }                     // Notas del cliente
    public string? InternalNotes { get; set; }             // Notas del operador
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }

    // Navegación
    public virtual User User { get; set; } = null!;
    public virtual AvailabilitySlot AvailabilitySlot { get; set; } = null!;
    public virtual BookingStatus Status { get; set; } = null!;
    public virtual Language? Language { get; set; }
    public virtual ICollection<BookingDetail> Details { get; set; } = [];
    public virtual ICollection<Payment> Payments { get; set; } = [];
    public virtual Review? Review { get; set; }
}