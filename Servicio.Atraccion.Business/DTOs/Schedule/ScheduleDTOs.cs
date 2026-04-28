namespace Servicio.Atraccion.Business.DTOs.Schedule;

// ── Requests ──────────────────────────────────────────────────

public class CreateScheduleTemplateRequest
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;

    // Días activos
    public bool Monday    { get; set; }
    public bool Tuesday   { get; set; }
    public bool Wednesday { get; set; }
    public bool Thursday  { get; set; }
    public bool Friday    { get; set; }
    public bool Saturday  { get; set; }
    public bool Sunday    { get; set; }

    public DateOnly ValidFrom { get; set; }
    public DateOnly? ValidTo  { get; set; }
    public short DefaultCapacity { get; set; } = 20;
    public string? Notes { get; set; }

    /// <summary>Lista de horas de salida para este patrón.</summary>
    public List<ScheduleTimeRequest> Times { get; set; } = [];
}

public class ScheduleTimeRequest
{
    /// <summary>Hora de inicio en formato "HH:mm" ej. "09:00", "10:30"</summary>
    public TimeOnly StartTime { get; set; }
    public TimeOnly? EndTime  { get; set; }
    /// <summary>null = usar DefaultCapacity del template</summary>
    public short? CapacityOverride { get; set; }
    public short SortOrder { get; set; }
}

public class UpdateScheduleTemplateRequest
{
    public string Name { get; set; } = string.Empty;
    public bool Monday    { get; set; }
    public bool Tuesday   { get; set; }
    public bool Wednesday { get; set; }
    public bool Thursday  { get; set; }
    public bool Friday    { get; set; }
    public bool Saturday  { get; set; }
    public bool Sunday    { get; set; }
    public DateOnly ValidFrom { get; set; }
    public DateOnly? ValidTo  { get; set; }
    public short DefaultCapacity { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
    public List<ScheduleTimeRequest> Times { get; set; } = [];
}

/// <summary>Genera AvailabilitySlots a partir de la plantilla para un rango de fechas.</summary>
public class GenerateSlotsRequest
{
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate   { get; set; }
    /// <summary>
    /// Si true, borra los slots existentes sin reservas en ese rango antes de regenerar.
    /// </summary>
    public bool OverwriteExisting { get; set; } = false;
}

/// <summary>Filtros para borrar slots de disponibilidad en bloque.</summary>
public class DeleteSlotsRequest
{
    public Guid ProductId { get; set; }
    public DateOnly? FromDate   { get; set; }
    public DateOnly? ToDate     { get; set; }
    /// <summary>0=Dom, 1=Lun, ... 6=Sáb  (null = todos los días)</summary>
    public int? DayOfWeek { get; set; }
    /// <summary>Fecha exacta para borrar todos los slots de ese día.</summary>
    public DateOnly? ExactDate  { get; set; }
}

// ── Responses ─────────────────────────────────────────────────

public class ScheduleTemplateResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Monday    { get; set; }
    public bool Tuesday   { get; set; }
    public bool Wednesday { get; set; }
    public bool Thursday  { get; set; }
    public bool Friday    { get; set; }
    public bool Saturday  { get; set; }
    public bool Sunday    { get; set; }
    public DateOnly ValidFrom { get; set; }
    public DateOnly? ValidTo  { get; set; }
    public short DefaultCapacity { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
    public List<ScheduleTimeResponse> Times { get; set; } = [];
}

public class ScheduleTimeResponse
{
    public Guid Id { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly? EndTime  { get; set; }
    public short? CapacityOverride { get; set; }
    public short SortOrder { get; set; }
}

public class GenerateSlotsResult
{
    public int SlotsCreated { get; set; }
    public int SlotsSkipped { get; set; }    // días sin horario o fuera de rango
    public int SlotsDeleted { get; set; }    // si OverwriteExisting = true
}

public class DeleteSlotsResult
{
    public int SlotsDeleted { get; set; }
    public int SlotsSkipped { get; set; }   // slots con reservas activas no borrados
}
