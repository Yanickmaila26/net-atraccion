namespace Servicio.Atraccion.DataAccess.Entities;

/// <summary>
/// Plantilla de horario recurrente para un ProductOption.
/// Define en qué días de la semana opera y el período de vigencia.
/// De aquí se generan automáticamente los AvailabilitySlots.
/// </summary>
public class ProductScheduleTemplate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;          // ej. "Temporada alta"

    // Días de la semana activos
    public bool Monday    { get; set; }
    public bool Tuesday   { get; set; }
    public bool Wednesday { get; set; }
    public bool Thursday  { get; set; }
    public bool Friday    { get; set; }
    public bool Saturday  { get; set; }
    public bool Sunday    { get; set; }

    public DateOnly ValidFrom { get; set; }
    public DateOnly? ValidTo  { get; set; }       // null = indefinido
    public short DefaultCapacity { get; set; } = 20;
    public bool IsActive  { get; set; } = true;
    public string? Notes  { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navegación
    public virtual ProductOption Product { get; set; } = null!;
    public virtual ICollection<ProductScheduleTime> Times { get; set; } = [];
}
