using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Servicio.Atraccion.Business.DTOs.Auth;
using Servicio.Atraccion.Business.Exceptions;
using Servicio.Atraccion.Business.Interfaces;
using Servicio.Atraccion.DataAccess.Repositories.Interfaces;
using BCryptNet = BCrypt.Net.BCrypt;

namespace Servicio.Atraccion.Business.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _unitOfWork.Users.Query()
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

        if (user == null || !BCryptNet.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedBusinessException("Credenciales inválidas.");

        // Validar que el usuario sea exclusivamente un Cliente
        var roles = user.UserRoles?.Select(ur => ur.Role?.Name).Where(n => n != null).ToList() ?? [];
        var isAdminOrPartner = roles.Any(r => r == "Admin" || r == "Partner");

        if (isAdminOrPartner)
            throw new UnauthorizedBusinessException("Esta ruta es exclusiva para clientes. Usa /login-admin para acceder al panel administrativo.");

        return GenerateTokenResponse(user);
    }

    public async Task<LoginResponse> LoginAdminAsync(LoginRequest request)
    {
        var user = await _unitOfWork.Users.Query()
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

        if (user == null || !BCryptNet.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedBusinessException("Credenciales inválidas.");

        // Validar que el usuario sea Admin o Partner (no Cliente)
        var roles = user.UserRoles?.Select(ur => ur.Role?.Name).Where(n => n != null).ToList() ?? [];
        var hasAdminAccess = roles.Any(r => r == "Admin" || r == "Partner");

        if (!hasAdminAccess)
            throw new UnauthorizedBusinessException("Esta ruta es exclusiva para administradores y partners. Usa /login para acceder como cliente.");

        return GenerateTokenResponse(user);
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
    {
        // 1. Validar existencia
        var exists = await _unitOfWork.Users.Query()
            .AnyAsync(u => u.Email == request.Email);

        if (exists)
            throw new ConflictException("El email ya está registrado.");

        // 2. Obtener rol por defecto (Cliente)
        var clientRole = await _unitOfWork.Roles.Query()
            .FirstOrDefaultAsync(r => r.Name == "Client");

        if (clientRole == null)
            throw new BusinessException("No se encontró el rol de cliente en la base de datos.");

        // 3. Crear Usuario y Perfil (Cliente)
        var newUser = new DataAccess.Entities.User
        {
            Email = request.Email,
            PasswordHash = BCryptNet.HashPassword(request.Password),
            IsActive = true,
            Client = new DataAccess.Entities.Client
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                DocumentType = request.DocumentType,
                DocumentNumber = request.DocumentNumber
            },
            UserRoles = new List<DataAccess.Entities.UserRole>
            {
                new DataAccess.Entities.UserRole
                {
                    RoleId = clientRole.Id,
                }
            }
        };

        await _unitOfWork.Users.AddAsync(newUser);
        var created = await _unitOfWork.CompleteAsync();

        if (created == 0)
            throw new BusinessException("No se pudo registrar el usuario.");

        // 4. Re-consultar para incluir roles y datos relacionados (evita NullReference en GenerateTokenResponse)
        var userWithRoles = await _unitOfWork.Users.Query()
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == newUser.Id);

        return GenerateTokenResponse(userWithRoles!);
    }

    private LoginResponse GenerateTokenResponse(DataAccess.Entities.User user)
    {
        var roles = user.UserRoles?.Select(ur => ur.Role?.Name).Where(n => n != null).Cast<string>().ToList() ?? new List<string>();

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Agregar los roles como claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"] ?? "ServicioAtraccion_Super_Secret_Key_2026_Minimum_Length_Requirement_Long_String"));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationMinutes"] ?? "30"));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "ServicioAtraccion",
            audience: _configuration["Jwt:Audience"] ?? "ServicioAtraccionUsers",
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new LoginResponse
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            TokenType = "Bearer",
            ExpiresInSeconds = (int)(expires - DateTime.UtcNow).TotalSeconds,
            User = new UserClaimsResponse
            {
                UserId = user.Id,
                Email = user.Email,
                Roles = roles
            }
        };
    }
}




