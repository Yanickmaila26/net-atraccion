using Servicio.Atraccion.DataAccess.Entities;

namespace Servicio.Atraccion.DataAccess.Queries.Models;

public class AvailabilitySummaryDto
{
    public string Title { get; set; } = string.Empty;
    public int DurationTotal { get; set; }
    public List<AvailabilitySlot> NextSlots { get; set; } = new();
}