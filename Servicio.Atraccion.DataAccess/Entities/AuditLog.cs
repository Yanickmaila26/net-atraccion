namespace Servicio.Atraccion.DataAccess.Entities;

/// <summary>
/// Registro de auditoría completo.
/// Se popula desde AtraccionDbContext.SaveChangesAsync() — NO usa triggers SQL.
/// </summary>
public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TableName { get; set; } = string.Empty;
    public Guid RecordId { get; set; }

    /// <summary>"INSERT" | "UPDATE" | "DELETE"</summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>Email o ID del usuario autenticado. Null = proceso automático.</summary>
    public string? ChangedBy { get; set; }

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Estado anterior en JSON. Null en INSERT.</summary>
    public string? OldValues { get; set; }

    /// <summary>Estado nuevo en JSON. Null en DELETE.</summary>
    public string? NewValues { get; set; }

    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Endpoint { get; set; }
}