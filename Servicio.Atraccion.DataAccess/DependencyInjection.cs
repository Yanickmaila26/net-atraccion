using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Servicio.Atraccion.DataAccess.Context;
using Servicio.Atraccion.DataAccess.Queries;
using Servicio.Atraccion.DataAccess.Repositories;
using Servicio.Atraccion.DataAccess.Repositories.Interfaces;

namespace Servicio.Atraccion.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
    {
        // ── Registrar el factory de conexiones por rol ──
        // Nota: IHttpContextAccessor se registra en Program.cs (capa API)
        services.AddScoped<IDbConnectionFactory, RoleBasedConnectionFactory>();

        // ── DbContext con conexión dinámica por rol ──
        // Se registra como Scoped-factory para que cada request resuelva
        // su cadena de conexión en función del JWT activo.
        services.AddDbContext<AtraccionDbContext>((serviceProvider, options) =>
        {
            var factory = serviceProvider.GetRequiredService<IDbConnectionFactory>();
            var connectionString = factory.GetConnectionString();

            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            });
        });

        // ── Unit of Work ──
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // ── Queries (Lectura Optimizada sin Tracking) ──
        services.AddScoped<IAttractionQueries, AttractionQueries>();
        services.AddScoped<IClienteQueryRepository, ClienteQueryRepository>();

        // ── Repositorio genérico ──
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        return services;
    }
}
