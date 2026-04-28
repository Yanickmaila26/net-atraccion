using Servicio.Atraccion.Business.DTOs.Auth;

namespace Servicio.Atraccion.Business.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<LoginResponse> LoginAdminAsync(LoginRequest request);
    Task<LoginResponse> RegisterAsync(RegisterRequest request);
}
