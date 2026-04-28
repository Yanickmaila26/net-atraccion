using Servicio.Atraccion.DataAccess.Common;

namespace Servicio.Atraccion.DataAccess.Entities;

/// <summary>
/// Modelo jerárquico representativo de entidades geográficas (países, estados, ciudades).
/// </summary>
public class Location : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;  // "country", "state", "city", etc.
    public Guid? ParentId { get; set; }
    public string? CountryCode { get; set; }

    // Navegación
    public virtual Location? Parent { get; set; }
    public virtual ICollection<Location> Children { get; set; } = [];
    public virtual ICollection<Client> Clients { get; set; } = [];
    public virtual ICollection<Attraction> Attractions { get; set; } = [];
}
