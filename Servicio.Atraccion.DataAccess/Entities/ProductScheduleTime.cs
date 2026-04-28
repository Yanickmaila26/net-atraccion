namespace Servicio.Atraccion.DataAccess.Entities;

/// <summary>
/// Hora específica dentro de una plantilla de horario.
/// Una plantilla puede tener múltiples horas de salida en el mismo día.
/// Ej: 09:00, 10:30, 12:00, 13:00
/// </summary>
public class ProductScheduleTime
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TemplateId { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly? EndTime  { get; set; }

    /// <summary>
    /// Si es null, usa DefaultCapacity del template.
    /// Permite sobreescribir por horario específico (ej. el de las 9am tiene más cupos).
    /// </summary>
    public short? CapacityOverride { get; set; }
    public short SortOrder { get; set; }

    // Navegación
    public virtual ProductScheduleTemplate Template { get; set; } = null!;
}
