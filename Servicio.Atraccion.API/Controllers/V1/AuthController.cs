using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Servicio.Atraccion.Business.DTOs.Auth;
using Servicio.Atraccion.Business.Interfaces;

namespace Servicio.Atraccion.API.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Iniciar sesión en el sistema y obtener un token JWT.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Iniciar sesión administrativa (Solo para Admin y Partner).
    /// </summary>
    [HttpPost("login-admin")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> LoginAdmin([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAdminAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Registrar una nueva cuenta de cliente.
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return StatusCode(201, result);
    }

    /// <summary>
    /// Endpoint de prueba para verificar si el token Bearer funciona.
    /// Requiere un usuario con el rol Client.
    /// </summary>
    [HttpGet("test-client")]
    [Authorize(Roles = "Client")]
    public IActionResult TestClientAuth()
    {
        return Ok(new { message = "Tienes acceso como cliente", userId = User.Identity?.Name });
    }

    /// <summary>
    /// Endpoint de prueba para verificar si el token Bearer funciona.
    /// Permite Admin y Partner.
    /// </summary>
    [HttpGet("test-management")]
    [Authorize(Roles = "Admin,Partner")]
    public IActionResult TestManagementAuth()
    {
        return Ok(new { message = "Tienes acceso administrativo", userId = User.Identity?.Name });
    }

    /// <summary>
    /// Endpoint de prueba para verificar si el token Bearer funciona.
    /// Requiere un usuario con el rol Partner.
    /// </summary>
    [HttpGet("test-partner")]
    [Authorize(Roles = "Partner")]
    public IActionResult TestPartnerAuth()
    {
        return Ok(new { message = "Tienes acceso como partner", userId = User.Identity?.Name });
    }
}
