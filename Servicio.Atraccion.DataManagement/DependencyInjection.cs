using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using Servicio.Atraccion.DataManagement.Interfaces;
using Servicio.Atraccion.DataManagement.Services;
using System.Reflection;

namespace Servicio.Atraccion.DataManagement;

public static class DependencyInjection
{
    public static IServiceCollection AddDataManagementServices(this IServiceCollection services)
    {
        // 1. Configurar Mapster
        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        // Escanear el ensamblado actual para encontrar implementaciones de IRegister (ej: AttractionMapperConfig)
        typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());
        
        services.AddSingleton(typeAdapterConfig);
        services.AddScoped<IMapper, ServiceMapper>();

        // 2. Registrar Data Services
        services.AddScoped<IAttractionDataService, AttractionDataService>();
        services.AddScoped<IInventoryDataService, InventoryDataService>();
        services.AddScoped<ILocationDataService, LocationDataService>();
        services.AddScoped<IClientDataService, ClientDataService>();
        services.AddScoped<IBookingDataService, BookingDataService>();
        services.AddScoped<IReviewDataService, ReviewDataService>();
        services.AddScoped<ICategoryDataService, CategoryDataService>();
        services.AddScoped<IMasterDataDataService, MasterDataDataService>();
        services.AddScoped<IPaymentDataService, PaymentDataService>();

        return services;
    }
}
