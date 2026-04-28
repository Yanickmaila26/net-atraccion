using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Servicio.Atraccion.DataAccess.Context;

/// <summary>
/// Determina la cadena de conexión correcta según el rol del usuario autenticado en el JWT.
/// Mapa de roles → usuario de base de datos:
///   Admin   → yanick_admin   (DDL + DML total)
///   Partner → yanick_partner (DML limitado a su catálogo + POS)
///   Client  → yanick_booking (DML transaccional: reservas y pagos)
///   Anónimo → yanick_readonly (solo SELECT en catálogo público)
/// </summary>
public interface IDbConnectionFactory
{
    string GetConnectionString();
}

public class RoleBasedConnectionFactory : IDbConnectionFactory
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public RoleBasedConnectionFactory(
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public string GetConnectionString()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        // Si no hay contexto HTTP (ej. login, register, background) usamos ApiConnection
        // ya que ReadOnlyConnection no tiene acceso a las tablas de usuarios y auth.
        if (user == null || !user.Identity?.IsAuthenticated == true)
            return _configuration.GetConnectionString("ApiConnection")!;

        // Prioridad: Admin > Partner > Client (en caso de roles múltiples)
        if (user.IsInRole("Admin"))
            return _configuration.GetConnectionString("AdminConnection")!;

        if (user.IsInRole("Partner"))
            return _configuration.GetConnectionString("PartnerConnection")!;

        if (user.IsInRole("Client"))
            return _configuration.GetConnectionString("BookingConnection")!;

        // Cualquier usuario autenticado sin rol conocido → lectura pública
        return _configuration.GetConnectionString("ReadOnlyConnection")!;
    }
}
