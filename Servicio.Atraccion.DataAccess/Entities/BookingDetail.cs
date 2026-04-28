namespace Servicio.Atraccion.DataAccess.Entities;

public class BookingDetail
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BookingId { get; set; }
    public Guid PriceTierId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? DocumentType { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public short Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }      // Precio capturado al momento de reservar
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Booking Booking { get; set; } = null!;
    public virtual PriceTier PriceTier { get; set; } = null!;
}