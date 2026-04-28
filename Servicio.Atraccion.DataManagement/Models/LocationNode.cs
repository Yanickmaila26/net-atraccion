namespace Servicio.Atraccion.DataManagement.Models;

public class LocationNode
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "country", "city", etc
    public string? CountryCode { get; set; }
    public Guid? ParentId { get; set; }
    
    // Lista plana de hijos si es requerida en árbol
    public List<LocationNode> Children { get; set; } = [];
}
