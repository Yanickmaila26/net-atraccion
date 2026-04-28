using Servicio.Atraccion.DataAccess.Common;
using Servicio.Atraccion.DataAccess.Entities;
using Servicio.Atraccion.DataManagement.Models;

namespace Servicio.Atraccion.DataManagement.Interfaces;

public interface IAttractionDataService
{
    Task<PagedResult<AttractionNode>> SearchAttractionsAsync(QueryFilters filters);
    Task<AttractionNode?> GetAttractionBySlugAsync(string slug, short? languageId = null);
    Task<Attraction?> GetByIdAsync(Guid id);
    Task<Attraction?> GetFullByIdAsync(Guid id);
    Task<IEnumerable<AttractionNode>> GetTopRatedAsync(int count);
    Task<Guid> AddAttractionAsync(Attraction attraction);
    Task<bool> UpdateAsync(Attraction attraction);
    Task<bool> DeleteAsync(Guid id);
}
