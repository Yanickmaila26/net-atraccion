using Servicio.Atraccion.DataAccess.Common;

namespace Servicio.Atraccion.DataAccess.Entities;

public class Client : BaseEntity
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateOnly? BirthDate { get; set; }
    public string? Nationality { get; set; }
    public string? DocumentType { get; set; }       // "passport" | "dni" | "ruc"
    public string? DocumentNumber { get; set; }
    public Guid? LocationId { get; set; }
    public string? AvatarUrl { get; set; }
    public short? PreferredLang { get; set; }        // FK → Language.Id
    public DateTime? DeletedAt { get; set; }

    /// <summary>Nombre completo calculado (no mapeado).</summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    // Navegación
    public virtual User User { get; set; } = null!;
    public virtual Location? Location { get; set; }
    public virtual Language? Language { get; set; }
}