using Servicio.Atraccion.DataAccess.Entities;

namespace Servicio.Atraccion.DataAccess.Repositories.Interfaces;

// ── Geografía ───────────────────────────────────────
public interface ILocationRepository : IGenericRepository<Location> { }

// ── Idiomas ─────────────────────────────────────────
public interface ILanguageRepository : IGenericRepository<Language> { }

// ── RBAC ────────────────────────────────────────────
public interface IUserRepository : IGenericRepository<User> { }
public interface IRoleRepository : IGenericRepository<Role> { }
public interface IClienteRepository : IGenericRepository<Client> { }

// ── Catálogo ────────────────────────────────────────
public interface ICategoryRepository : IGenericRepository<Category> { }
public interface ISubcategoryRepository : IGenericRepository<Subcategory> { }
public interface ITagRepository : IGenericRepository<Tag> { }

// ── Atracción ───────────────────────────────────────
public interface IAttractionRepository : IGenericRepository<Attraction> 
{
    Task<Attraction?> GetAttractionWithDetailsAsync(Guid id);
}
public interface IAudioGuideRepository : IGenericRepository<AudioGuide> { }
public interface ITourItineraryRepository : IGenericRepository<TourItinerary> { }

// ── Inclusiones y Modalidades ───────────────────────
public interface IInclusionItemRepository : IGenericRepository<InclusionItem> { }
public interface IProductOptionRepository : IGenericRepository<ProductOption> { }

// ── Inventario ──────────────────────────────────────
public interface IAvailabilitySlotRepository : IGenericRepository<AvailabilitySlot> { }
public interface IPriceTierRepository : IGenericRepository<PriceTier> { }

// ── Reservas y Pagos ────────────────────────────────
public interface IBookingRepository : IGenericRepository<Booking> { }
public interface IBookingDetailRepository : IGenericRepository<BookingDetail> { }
public interface IPaymentRepository : IGenericRepository<Payment> { }

// ── Reseñas ─────────────────────────────────────────
public interface IReviewRepository : IGenericRepository<Review> { }

// ── Otros ───────────────────────────────────────────
public interface ITicketCategoryRepository : IGenericRepository<TicketCategory> { }
public interface ITourStopRepository : IGenericRepository<TourStop> { }

// ── Horarios ─────────────────────────────────────────
public interface IProductScheduleTemplateRepository : IGenericRepository<ProductScheduleTemplate> { }
public interface IProductScheduleTimeRepository : IGenericRepository<ProductScheduleTime> { }
