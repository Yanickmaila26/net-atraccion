using Microsoft.EntityFrameworkCore;
using Servicio.Atraccion.DataAccess.Context;
using Servicio.Atraccion.DataAccess.Entities;
using Servicio.Atraccion.DataAccess.Repositories.Interfaces;

namespace Servicio.Atraccion.DataAccess.Repositories;

// ── Geografía ───────────────────────────────────────
public class LocationRepository : GenericRepository<Location>, ILocationRepository 
{ public LocationRepository(AtraccionDbContext context) : base(context) { } }

// ── Idiomas ─────────────────────────────────────────
public class LanguageRepository : GenericRepository<Language>, ILanguageRepository 
{ public LanguageRepository(AtraccionDbContext context) : base(context) { } }

// ── RBAC ────────────────────────────────────────────
public class UserRepository : GenericRepository<User>, IUserRepository 
{ public UserRepository(AtraccionDbContext context) : base(context) { } }

public class RoleRepository : GenericRepository<Role>, IRoleRepository 
{ public RoleRepository(AtraccionDbContext context) : base(context) { } }

public class ClienteRepository : GenericRepository<Client>, IClienteRepository 
{ public ClienteRepository(AtraccionDbContext context) : base(context) { } }

// ── Catálogo ────────────────────────────────────────
public class CategoryRepository : GenericRepository<Category>, ICategoryRepository 
{ public CategoryRepository(AtraccionDbContext context) : base(context) { } }

public class SubcategoryRepository : GenericRepository<Subcategory>, ISubcategoryRepository 
{ public SubcategoryRepository(AtraccionDbContext context) : base(context) { } }

public class TagRepository : GenericRepository<Tag>, ITagRepository 
{ public TagRepository(AtraccionDbContext context) : base(context) { } }

// ── Atracción ───────────────────────────────────────
public class AttractionRepository : GenericRepository<Attraction>, IAttractionRepository 
{
    public AttractionRepository(AtraccionDbContext context) : base(context) { }

    public async Task<Attraction?> GetAttractionWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(a => a.Media)
            .Include(a => a.Subcategory)
            .ThenInclude(s => s.Category)
            .Include(a => a.Translations)
            .Include(a => a.Languages).ThenInclude(l => l.Language)
            .Include(a => a.Tags).ThenInclude(t => t.Tag)
            .Include(a => a.Inclusions).ThenInclude(i => i.InclusionItem)
            .FirstOrDefaultAsync(a => a.Id == id);
    }
}

public class AudioGuideRepository : GenericRepository<AudioGuide>, IAudioGuideRepository 
{ public AudioGuideRepository(AtraccionDbContext context) : base(context) { } }

public class TourItineraryRepository : GenericRepository<TourItinerary>, ITourItineraryRepository 
{ public TourItineraryRepository(AtraccionDbContext context) : base(context) { } }

// ── Inclusiones y Modalidades ───────────────────────
public class InclusionItemRepository : GenericRepository<InclusionItem>, IInclusionItemRepository 
{ public InclusionItemRepository(AtraccionDbContext context) : base(context) { } }

public class ProductOptionRepository : GenericRepository<ProductOption>, IProductOptionRepository 
{ public ProductOptionRepository(AtraccionDbContext context) : base(context) { } }

// ── Inventario ──────────────────────────────────────
public class AvailabilitySlotRepository : GenericRepository<AvailabilitySlot>, IAvailabilitySlotRepository 
{ public AvailabilitySlotRepository(AtraccionDbContext context) : base(context) { } }

public class PriceTierRepository : GenericRepository<PriceTier>, IPriceTierRepository 
{ public PriceTierRepository(AtraccionDbContext context) : base(context) { } }

// ── Reservas y Pagos ────────────────────────────────
public class BookingRepository : GenericRepository<Booking>, IBookingRepository 
{ public BookingRepository(AtraccionDbContext context) : base(context) { } }

public class BookingDetailRepository : GenericRepository<BookingDetail>, IBookingDetailRepository 
{ public BookingDetailRepository(AtraccionDbContext context) : base(context) { } }

public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository 
{ public PaymentRepository(AtraccionDbContext context) : base(context) { } }

// ── Reseñas ─────────────────────────────────────────
public class ReviewRepository : GenericRepository<Review>, IReviewRepository 
{ public ReviewRepository(AtraccionDbContext context) : base(context) { } }

// ── Otros ───────────────────────────────────────────
public class TicketCategoryRepository : GenericRepository<TicketCategory>, ITicketCategoryRepository
{ public TicketCategoryRepository(AtraccionDbContext context) : base(context) { } }

public class TourStopRepository : GenericRepository<TourStop>, ITourStopRepository
{ public TourStopRepository(AtraccionDbContext context) : base(context) { } }

// ── Horarios ─────────────────────────────────────────
public class ProductScheduleTemplateRepository : GenericRepository<ProductScheduleTemplate>, IProductScheduleTemplateRepository
{ public ProductScheduleTemplateRepository(AtraccionDbContext context) : base(context) { } }

public class ProductScheduleTimeRepository : GenericRepository<ProductScheduleTime>, IProductScheduleTimeRepository
{ public ProductScheduleTimeRepository(AtraccionDbContext context) : base(context) { } }
