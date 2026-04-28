using Servicio.Atraccion.DataAccess.Common;
using Servicio.Atraccion.DataManagement.Models;

namespace Servicio.Atraccion.DataManagement.Interfaces;

public interface IReviewDataService
{
    Task<bool> AddReviewAsync(ReviewNode reviewNode);
    Task<PagedResult<ReviewNode>> GetReviewsByAttractionAsync(Guid attractionId, QueryFilters filters);
    Task<PagedResult<ReviewNode>> GetReviewsByAttractionSlugAsync(string slug, QueryFilters filters);
    Task<PagedResult<ReviewNode>> SearchReviewsAsync(ReviewQueryFilters filters);
    Task<decimal> GetAverageRatingByAttractionAsync(Guid attractionId);
    Task<bool> DeleteAsync(Guid reviewId);
}
