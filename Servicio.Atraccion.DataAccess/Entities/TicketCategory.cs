using System.ComponentModel.DataAnnotations;

namespace Servicio.Atraccion.DataAccess.Entities;

public class TicketCategory
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty; // e.g. "Niño"

    [MaxLength(50)]
    public string? NameEn { get; set; } // e.g. "Child"

    public short? AgeRangeMin { get; set; }
    public short? AgeRangeMax { get; set; }
    public bool IsActive { get; set; } = true;
    public short SortOrder { get; set; } = 0;

    public virtual ICollection<PriceTier> PriceTiers { get; set; } = [];
}
