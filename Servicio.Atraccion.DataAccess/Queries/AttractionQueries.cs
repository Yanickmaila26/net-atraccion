using Microsoft.EntityFrameworkCore;
using Servicio.Atraccion.DataAccess.Common;
using Servicio.Atraccion.DataAccess.Context;
using Servicio.Atraccion.DataAccess.Entities;
using Servicio.Atraccion.DataAccess.Queries.Models;

namespace Servicio.Atraccion.DataAccess.Queries;

public class AttractionQueries : IAttractionQueries
{
    private readonly AtraccionDbContext _context;

    public AttractionQueries(AtraccionDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Attraction>> SearchAttractionsAsync(QueryFilters filters)
    {
        // El filtro de borrado lógico se aplica automáticamente gracias al Global Query Filter
        var query = _context.Attractions
            .AsNoTracking()
            .Include(a => a.Media.Where(m => m.IsMain))
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
        {
            query = query.Where(a => a.Name.Contains(filters.SearchTerm) ||
                                    (a.DescriptionShort != null && a.DescriptionShort.Contains(filters.SearchTerm)));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((filters.PageNumber - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .ToListAsync();

        return new PagedResult<Attraction>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filters.PageNumber,
            PageSize = filters.PageSize
        };
    }
    public async Task<IEnumerable<Attraction>> GetTopRatedAttractionsAsync(int count)
    {
        // Igualmente aquí, eliminamos el .Where(a => a.IsActive)
        return await _context.Attractions
            .AsNoTracking()
            .OrderByDescending(a => a.RatingAverage)
            .Take(count)
            .ToListAsync();
    }
    public async Task<IEnumerable<AvailabilitySummaryDto>> GetAttractionAvailabilitySummaryAsync(Guid attractionId)
    {
        return await _context.ProductOptions
            .AsNoTracking()
            .Where(p => p.AttractionId == attractionId)
            .Select(p => new AvailabilitySummaryDto // Usamos el DTO aquí
            {
                Title = p.Title,
                DurationTotal = p.DurationMinutes ?? 0,
                NextSlots = p.AvailabilitySlots
                             .Where(s => s.CapacityAvailable > 0)
                             .OrderBy(s => s.StartTime)
                             .Take(3)
                             .ToList()
            })
            .ToListAsync();
    }

}