using Servicio.Atraccion.DataAccess.Common;

namespace Servicio.Atraccion.DataAccess.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool EmailVerified { get; set; } = false;
    public DateTime? LastLoginAt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navegación
    public virtual Client? Client { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; } = [];
    public virtual ICollection<Booking> Bookings { get; set; } = [];
    public virtual ICollection<Review> Reviews { get; set; } = [];
}