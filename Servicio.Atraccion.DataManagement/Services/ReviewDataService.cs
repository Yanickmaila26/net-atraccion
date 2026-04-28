using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Servicio.Atraccion.DataAccess.Common;
using Servicio.Atraccion.DataAccess.Entities;
using Servicio.Atraccion.DataAccess.Repositories.Interfaces;
using Servicio.Atraccion.DataManagement.Interfaces;
using Servicio.Atraccion.DataManagement.Models;

namespace Servicio.Atraccion.DataManagement.Services;

public class ReviewDataService : IReviewDataService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ReviewDataService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<bool> AddReviewAsync(ReviewNode reviewNode)
    {
        var entity = _mapper.Map<Review>(reviewNode);
        
        // 1. Guardar Review
        await _uow.Reviews.AddAsync(entity);
        var success = await _uow.CompleteAsync() > 0;
        
        if (success)
        {
            // 2. Recalcular el rating average de la Atracción
            // Obtenemos el Booking -> Slot -> Product -> Attraction
            var booking = await _uow.Bookings.Query()
                .Include(b => b.AvailabilitySlot)
                    .ThenInclude(s => s.ProductOption)
                .FirstOrDefaultAsync(b => b.Id == reviewNode.BookingId);
                
            if (booking != null)
            {
                var attractionId = booking.AvailabilitySlot.ProductOption.AttractionId;
                var avg = await GetAverageRatingByAttractionAsync(attractionId);
                var count = await _uow.Reviews.Query()
                    .Where(r => r.Booking.AvailabilitySlot.ProductOption.AttractionId == attractionId)
                    .CountAsync();
                    
                var attraction = await _uow.Attractions.GetByIdAsync(attractionId);
                if (attraction != null)
                {
                    attraction.RatingAverage = avg;
                    attraction.RatingCount = count;
                    _uow.Attractions.Update(attraction);
                    await _uow.CompleteAsync();
                }
            }
        }
        
        return success;
    }

    public async Task<decimal> GetAverageRatingByAttractionAsync(Guid attractionId)
    {
        var avg = await _uow.Reviews.Query()
            .Where(r => r.Booking.AvailabilitySlot.ProductOption.AttractionId == attractionId)
            .AverageAsync(r => (decimal?)r.OverallScore);
            
        return avg ?? 0;
    }

    public async Task<PagedResult<ReviewNode>> GetReviewsByAttractionAsync(Guid attractionId, QueryFilters filters)
    {
        IQueryable<Review> query = _uow.Reviews.Query()
            .Include(r => r.Booking)
                .ThenInclude(b => b.User)
                    .ThenInclude(u => u.Client)
            .Include(r => r.Booking)
                .ThenInclude(b => b.AvailabilitySlot)
                    .ThenInclude(s => s.ProductOption)
                        .ThenInclude(p => p.Attraction)
            .Where(r => r.Booking.AvailabilitySlot.ProductOption.AttractionId == attractionId);

        var totalCount = await query.CountAsync();
        
        var items = await query.OrderByDescending(r => r.CreatedAt)
                               .Skip(filters.Offset)
                               .Take(filters.PageSize)
                               .ToListAsync();

        return new PagedResult<ReviewNode>
        {
            Items = _mapper.Map<IEnumerable<ReviewNode>>(items).ToList(),
            TotalCount = totalCount,
            PageNumber = filters.PageNumber,
            PageSize = filters.PageSize
        };
    }

    public async Task<PagedResult<ReviewNode>> GetReviewsByAttractionSlugAsync(string slug, QueryFilters filters)
    {
        IQueryable<Review> query = _uow.Reviews.Query()
            .Include(r => r.Booking)
                .ThenInclude(b => b.User)
                    .ThenInclude(u => u.Client)
            .Include(r => r.Booking)
                .ThenInclude(b => b.AvailabilitySlot)
                    .ThenInclude(s => s.ProductOption)
                        .ThenInclude(p => p.Attraction)
            .Where(r => r.Booking.AvailabilitySlot.ProductOption.Attraction.Slug == slug);

        var totalCount = await query.CountAsync();
        
        var items = await query.OrderByDescending(r => r.CreatedAt)
                               .Skip(filters.Offset)
                               .Take(filters.PageSize)
                               .ToListAsync();

        return new PagedResult<ReviewNode>
        {
            Items = _mapper.Map<IEnumerable<ReviewNode>>(items).ToList(),
            TotalCount = totalCount,
            PageNumber = filters.PageNumber,
            PageSize = filters.PageSize
        };
    }

    public async Task<PagedResult<ReviewNode>> SearchReviewsAsync(ReviewQueryFilters filters)
    {
        IQueryable<Review> query = _uow.Reviews.Query()
            .Include(r => r.Booking)
                .ThenInclude(b => b.User)
                    .ThenInclude(u => u.Client)
            .Include(r => r.Booking)
                .ThenInclude(b => b.AvailabilitySlot)
                    .ThenInclude(s => s.ProductOption)
                        .ThenInclude(p => p.Attraction);

        // Filtrado por Partner (ManagedBy)
        if (filters.ManagedById.HasValue)
        {
            query = query.Where(r => r.Booking.AvailabilitySlot.ProductOption.Attraction.ManagedById == filters.ManagedById.Value);
        }

        // Otros filtros
        if (filters.AttractionId.HasValue)
        {
            query = query.Where(r => r.Booking.AvailabilitySlot.ProductOption.AttractionId == filters.AttractionId.Value);
        }

        var totalCount = await query.CountAsync();
        
        var items = await query.OrderByDescending(x => x.CreatedAt)
                               .Skip(filters.Offset)
                               .Take(filters.PageSize)
                               .ToListAsync();

        return new PagedResult<ReviewNode>
        {
            Items = _mapper.Map<IEnumerable<ReviewNode>>(items).ToList(),
            TotalCount = totalCount,
            PageNumber = filters.PageNumber,
            PageSize = filters.PageSize
        };
    }

    public async Task<bool> DeleteAsync(Guid reviewId)
    {
        var entity = await _uow.Reviews.GetByIdAsync(reviewId);
        if (entity == null) return false;

        _uow.Reviews.Delete(entity);
        return await _uow.CompleteAsync() > 0;
    }
}
