namespace Servicio.Atraccion.DataAccess.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    // Geografía
    ILocationRepository Locations { get; }

    // Idiomas
    ILanguageRepository Languages { get; }

    // RBAC
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    IClienteRepository Clients { get; }

    // Catálogo
    ICategoryRepository Categories { get; }
    ISubcategoryRepository Subcategories { get; }
    ITagRepository Tags { get; }

    // Atracción
    IAttractionRepository Attractions { get; }
    IAudioGuideRepository AudioGuides { get; }
    ITourItineraryRepository TourItineraries { get; }

    // Inclusiones y Modalidades
    IInclusionItemRepository InclusionItems { get; }
    IProductOptionRepository ProductOptions { get; }

    // Inventario
    IAvailabilitySlotRepository AvailabilitySlots { get; }
    IPriceTierRepository PriceTiers { get; }

    // Reservas y Pagos
    IBookingRepository Bookings { get; }
    IBookingDetailRepository BookingDetails { get; }
    IPaymentRepository Payments { get; }

    // Reseñas
    IReviewRepository Reviews { get; }

    // Otros
    ITicketCategoryRepository TicketCategories { get; }
    ITourStopRepository TourStops { get; }

    // Horarios
    IProductScheduleTemplateRepository ScheduleTemplates { get; }
    IProductScheduleTimeRepository ScheduleTimes { get; }

    Task<int> CompleteAsync();
}