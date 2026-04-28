using Servicio.Atraccion.Business.DTOs.Review;
using Servicio.Atraccion.DataAccess.Common;

namespace Servicio.Atraccion.Business.Interfaces;

public interface IReviewService
{
    /// <summary>Publica una reseña. Solo puede hacerlo el usuario dueño de una reserva completada (o Admin para mantenimiento).</summary>
    Task<ReviewResponse> CreateReviewAsync(Guid userId, bool isAdmin, CreateReviewRequest request);
    
    /// <summary>Obtiene reseñas paginadas de una atracción.</summary>
    Task<PagedResult<ReviewResponse>> GetByAttractionAsync(Guid attractionId, int page = 1, int pageSize = 10);

    /// <summary>Obtiene reseñas paginadas de una atracción mediante su slug.</summary>
    Task<PagedResult<ReviewResponse>> GetByAttractionSlugAsync(string slug, int page = 1, int pageSize = 10);

    /// <summary>Búsqueda administrativa de reseñas con filtrado por rol (Partner/Admin).</summary>
    Task<PagedResult<ReviewResponse>> SearchManagementAsync(ReviewSearchRequest request, Guid currentUserId, bool isAdmin);

    /// <summary>Elimina una reseña (solo Admin o Partner propietario de la atracción).</summary>
    Task<bool> DeleteReviewAsync(Guid reviewId, Guid userId, bool isAdmin);
}
