using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Servicio.Atraccion.DataAccess;       // Capa 1
using Servicio.Atraccion.DataManagement;   // Capa 2
using Servicio.Atraccion.Business;         // Capa 3
using Asp.Versioning;
using Servicio.Atraccion.API.Middleware;   // <-- Añadido

var builder = WebApplication.CreateBuilder(args);

// IHttpContextAccessor: requerido por el factory de conexiones por rol
builder.Services.AddHttpContextAccessor();

// ======================================================
// 1. CONFIGURACIÓN DE CAPAS (CLEAN ARCHITECTURE)
// ======================================================
builder.Services.AddDataAccessServices(builder.Configuration);
builder.Services.AddDataManagementServices();
builder.Services.AddBusinessServices();

// ======================================================
// 2. CONFIGURACIÓN API & CORS
// ======================================================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Esto asegura que la API entienda "13:00" como un TimeOnly correctamente
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
builder.Services.AddEndpointsApiExplorer();

// ======================================================
// 4. SWAGGER CON SEGURIDAD JWT
// ======================================================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Servicio Atracción API", Version = "v1" });

    // Soporte para tipos específicos en Swagger (.NET 8/10)
    c.MapType<DateOnly>(() => new Microsoft.OpenApi.Models.OpenApiSchema { Type = "string", Format = "date" });
    c.MapType<TimeOnly>(() => new Microsoft.OpenApi.Models.OpenApiSchema { Type = "string", Format = "time" });
    c.MapType<TimeSpan>(() => new Microsoft.OpenApi.Models.OpenApiSchema { Type = "string", Format = "duration" });

    // Evitar conflictos de nombres de esquema si hay clases con el mismo nombre en distintos namespaces
    c.CustomSchemaIds(type => type.FullName);

    // Definición de seguridad JWT para Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Ingresa tu token JWT en el formato: Bearer {tu_token_aqui}",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new Microsoft.OpenApi.Models.OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme
        }
    });

    // Requisito de seguridad global en Swagger
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// ======================================================
// 3. JWT AUTHENTICATION
// ======================================================
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ServicioAtraccion_Super_Secret_Key_2026_Minimum_Length_Requirement_Long_String"; // Cambiar esto en appsettings.json
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ServicioAtraccion";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ServicioAtraccionUsers";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();

// ======================================================
// 5. CONFIGURACIÓN DEL PIPELINE HTTP
// ======================================================
// ======================================================
// 5. CONFIGURACIÓN DEL PIPELINE HTTP
// ======================================================

// Esto hace que Swagger sea accesible siempre (o podrías añadir una condición más flexible)
app.UseSwagger();
app.UseSwaggerUI(c => 
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Servicio Atracción API v1");
    c.RoutePrefix = string.Empty; // <-- ESTO ES CLAVE: Hace que Swagger aparezca en la raíz
});

// Middleware Global de Excepciones
app.UseMiddleware<Servicio.Atraccion.API.Middleware.ExceptionMiddleware>();
// Habilitar el servicio de archivos estáticos para la carpeta wwwroot (uploads locales)
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Orden indispensable: Autenticación ANTES que Autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();