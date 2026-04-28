using Servicio.Atraccion.DataAccess.Context;
using Servicio.Atraccion.DataAccess.Repositories.Interfaces;

namespace Servicio.Atraccion.DataAccess.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AtraccionDbContext _context;

    public UnitOfWork(AtraccionDbContext context)
    {
        _context = context;
    }

    // Geografía
    private ILocationRepository? _locations;
    public ILocationRepository Locations => _locations ??= new LocationRepository(_context);

    // Idiomas
    private ILanguageRepository? _languages;
    public ILanguageRepository Languages => _languages ??= new LanguageRepository(_context);

    // RBAC
    private IUserRepository? _users;
    private IRoleRepository? _roles;
    private IClienteRepository? _clients;
    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IRoleRepository Roles => _roles ??= new RoleRepository(_context);
    public IClienteRepository Clients => _clients ??= new ClienteRepository(_context);

    // Catálogo
    private ICategoryRepository? _categories;
    private ISubcategoryRepository? _subcategories;
    private ITagRepository? _tags;
    public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);
    public ISubcategoryRepository Subcategories => _subcategories ??= new SubcategoryRepository(_context);
    public ITagRepository Tags => _tags ??= new TagRepository(_context);

    // Atracción
    private IAttractionRepository? _attractions;
    private IAudioGuideRepository? _audioGuides;
    private ITourItineraryRepository? _tourItineraries;
    public IAttractionRepository Attractions => _attractions ??= new AttractionRepository(_context);
    public IAudioGuideRepository AudioGuides => _audioGuides ??= new AudioGuideRepository(_context);
    public ITourItineraryRepository TourItineraries => _tourItineraries ??= new TourItineraryRepository(_context);

    // Inclusiones y Modalidades
    private IInclusionItemRepository? _inclusionItems;
    private IProductOptionRepository? _productOptions;
    public IInclusionItemRepository InclusionItems => _inclusionItems ??= new InclusionItemRepository(_context);
    public IProductOptionRepository ProductOptions => _productOptions ??= new ProductOptionRepository(_context);

    // Inventario
    private IAvailabilitySlotRepository? _availabilitySlots;
    private IPriceTierRepository? _priceTiers;
    public IAvailabilitySlotRepository AvailabilitySlots => _availabilitySlots ??= new AvailabilitySlotRepository(_context);
    public IPriceTierRepository PriceTiers => _priceTiers ??= new PriceTierRepository(_context);

    // Reservas y Pagos
    private IBookingRepository? _bookings;
    private IBookingDetailRepository? _bookingDetails;
    private IPaymentRepository? _payments;
    public IBookingRepository Bookings => _bookings ??= new BookingRepository(_context);
    public IBookingDetailRepository BookingDetails => _bookingDetails ??= new BookingDetailRepository(_context);
    public IPaymentRepository Payments => _payments ??= new PaymentRepository(_context);

    // Reseñas
    private IReviewRepository? _reviews;
    public IReviewRepository Reviews => _reviews ??= new ReviewRepository(_context);

    // Otros
    private ITicketCategoryRepository? _ticketCategories;
    private ITourStopRepository? _tourStops;
    public ITicketCategoryRepository TicketCategories => _ticketCategories ??= new TicketCategoryRepository(_context);
    public ITourStopRepository TourStops => _tourStops ??= new TourStopRepository(_context);

    // Horarios
    private IProductScheduleTemplateRepository? _scheduleTemplates;
    private IProductScheduleTimeRepository? _scheduleTimes;
    public IProductScheduleTemplateRepository ScheduleTemplates => _scheduleTemplates ??= new ProductScheduleTemplateRepository(_context);
    public IProductScheduleTimeRepository ScheduleTimes => _scheduleTimes ??= new ProductScheduleTimeRepository(_context);

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}