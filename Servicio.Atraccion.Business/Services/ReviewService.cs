using FluentValidation;
using Servicio.Atraccion.Business.DTOs.Review;
using Servicio.Atraccion.Business.Exceptions;
using Servicio.Atraccion.Business.Interfaces;
using Servicio.Atraccion.DataAccess.Common;
using Servicio.Atraccion.DataManagement.Interfaces;
using Servicio.Atraccion.DataManagement.Models;

namespace Servicio.Atraccion.Business.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewDataService _reviewData;
    private readonly IBookingDataService _bookingData;
    private readonly IValidator<CreateReviewRequest> _validator;

    public ReviewService(
        IReviewDataService reviewData,
        IBookingDataService bookingData,
        IValidator<CreateReviewRequest> validator)
    {
        _reviewData = reviewData;
        _bookingData = bookingData;
        _validator = validator;
    }

    public async Task<ReviewResponse> CreateReviewAsync(Guid userId, bool isAdmin, CreateReviewRequest request)
    {
        // ── PASO 1: Validar formato ───────────────────────────────────────────
        var validation = await _validator.ValidateAsync(request);
        if (!validation.IsValid)
            throw new Exceptions.ValidationException(validation.Errors.Select(e => e.ErrorMessage).ToList());

        // ── PASO 2: Verificar que la reserva existe y validar permisos ────
        var booking = await _bookingData.GetByPnrAsync(request.PnrCode.ToUpperInvariant());
        if (booking == null)
            throw new NotFoundException("Reserva", request.PnrCode);

        // Si no es admin, debe ser el dueño de la reserva
        if (!isAdmin && booking.UserId != userId)
            throw new UnauthorizedBusinessException("No puedes reseñar una reserva que no te pertenece.");

        // ── PASO 3: Solo se puede reseñar una reserva COMPLETADA o CONFIRMADA que ya pasó ─────
        bool isCompleted = booking.StatusId == 3;
        bool isConfirmedAndPassed = booking.StatusId == 2 && 
                                   DateTime.UtcNow > booking.SlotDate.ToDateTime(booking.SlotStartTime);

        if (!isCompleted && !isConfirmedAndPassed)
            throw new BusinessException("Solo puedes reseñar tours que ya hayan finalizado o estén marcados como completados.");

        // ── PASO 4: Construir ReviewNode ──────────────────────────────────────
        var reviewNode = new ReviewNode
        {
            BookingId = booking.Id,
            Rating = request.OverallRating,
            Comment = request.Comment,
            Ratings = request.Ratings.Select(r => new ReviewRatingNode
            {
                Criteria = r.CriteriaId.ToString(), // El DataService resolverá el nombre
                Rating = r.Score
            }).ToList()
        };

        // ── PASO 5: Persistir (el DataService recalcula el rating de la atracción)
        var success = await _reviewData.AddReviewAsync(reviewNode);
        if (!success)
            throw new BusinessException("No se pudo guardar la reseña. Intente nuevamente.");

        // ── PASO 6: Retornar respuesta ────────────────────────────────────────
        return new ReviewResponse
        {
            Id = reviewNode.Id,
            ClientName = reviewNode.ClientName,
            OverallRating = request.OverallRating,
            Title = request.Title,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow,
            Ratings = request.Ratings.Select(r => new CriteriaRatingResponse
            {
                CriteriaName = r.CriteriaId.ToString(),
                Score = r.Score
            }).ToList()
        };
    }

    public async Task<PagedResult<ReviewResponse>> GetByAttractionAsync(Guid attractionId, int page = 1, int pageSize = 10)
    {
        var filters = new QueryFilters { PageNumber = page, PageSize = pageSize };
        var paged = await _reviewData.GetReviewsByAttractionAsync(attractionId, filters);

        return new PagedResult<ReviewResponse>
        {
            Items = paged.Items.Select(MapToResponse).ToList(),
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }

    public async Task<PagedResult<ReviewResponse>> GetByAttractionSlugAsync(string slug, int page = 1, int pageSize = 10)
    {
        var filters = new QueryFilters { PageNumber = page, PageSize = pageSize };
        var paged = await _reviewData.GetReviewsByAttractionSlugAsync(slug, filters);

        return new PagedResult<ReviewResponse>
        {
            Items = paged.Items.Select(MapToResponse).ToList(),
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }

    public async Task<PagedResult<ReviewResponse>> SearchManagementAsync(ReviewSearchRequest request, Guid currentUserId, bool isAdmin)
    {
        var filters = new ReviewQueryFilters
        {
            AttractionId = request.AttractionId,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        // Si es Partner, solo sus atracciones
        if (!isAdmin)
        {
            filters.ManagedById = currentUserId;
        }

        var paged = await _reviewData.SearchReviewsAsync(filters);

        return new PagedResult<ReviewResponse>
        {
            Items = paged.Items.Select(MapToResponse).ToList(),
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }

    private static ReviewResponse MapToResponse(ReviewNode node) => new()
    {
        Id = node.Id,
        ClientName = node.ClientName,
        OverallRating = node.Rating,
        Comment = node.Comment,
        CreatedAt = node.CreatedAt,
        Ratings = node.Ratings.Select(r => new CriteriaRatingResponse
        {
            CriteriaName = r.Criteria,
            Score = r.Rating
        }).ToList()
    };

    public async Task<bool> DeleteReviewAsync(Guid reviewId, Guid userId, bool isAdmin)
    {
        if (!isAdmin)
            throw new UnauthorizedBusinessException("Solo los administradores pueden eliminar reseñas.");

        return await _reviewData.DeleteAsync(reviewId);
    }
}
