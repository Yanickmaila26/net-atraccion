using Servicio.Atraccion.DataAccess.Common;

namespace Servicio.Atraccion.DataAccess.Entities;

public class Role
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty; // Admin, Client, Partner
    public string? Description { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}