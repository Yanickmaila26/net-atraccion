using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Servicio.Atraccion.Business.DTOs.Booking;
using Servicio.Atraccion.Business.DTOs.Review;
using Servicio.Atraccion.Business.Interfaces;
using Servicio.Atraccion.Business.Services;
using Servicio.Atraccion.Business.Validators;

namespace Servicio.Atraccion.Business;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        // ─── Servicios de negocio ──────────────────────────────────────────────
        services.AddScoped<IAttractionService, AttractionService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IMasterDataService, MasterDataService>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<IProductOptionService, ProductOptionService>();
        services.AddScoped<IStorageService, LocalStorageService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IUserService, UserService>();

        // ─── Validadores FluentValidation ──────────────────────────────────────
        services.AddScoped<IValidator<CreateBookingRequest>, CreateBookingValidator>();
        services.AddScoped<IValidator<CancelBookingRequest>, CancelBookingValidator>();
        services.AddScoped<IValidator<CreateReviewRequest>, CreateReviewValidator>();
        services.AddScoped<IValidator<DTOs.Cliente.CrearClienteRequest>, CrearClienteValidator>();
        services.AddScoped<IValidator<DTOs.Cliente.ActualizarClienteRequest>, ActualizarClienteValidator>();

        return services;
    }
}
