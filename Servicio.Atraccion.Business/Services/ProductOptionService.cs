using Microsoft.EntityFrameworkCore;
using Servicio.Atraccion.Business.DTOs.Attraction;
using Servicio.Atraccion.Business.DTOs.Inventory;
using Servicio.Atraccion.Business.Exceptions;
using Servicio.Atraccion.Business.Interfaces;
using Servicio.Atraccion.DataAccess.Context;
using Servicio.Atraccion.DataAccess.Entities;
using Servicio.Atraccion.DataAccess.Repositories.Interfaces;

namespace Servicio.Atraccion.Business.Services;

public class ProductOptionService : IProductOptionService
{
    private readonly IUnitOfWork _uow;
    private readonly AtraccionDbContext _db;

    public ProductOptionService(IUnitOfWork uow, AtraccionDbContext db)
    {
        _uow = uow;
        _db = db;
    }

    // ── CONSULTAS ────────────────────────────────────────────────

    public async Task<IEnumerable<ProductOptionDetailResponse>> GetByAttractionAsync(Guid attractionId)
    {
        var products = await _db.ProductOptions
            .Include(p => p.PriceTiers.Where(pt => pt.IsActive))
                .ThenInclude(pt => pt.TicketCategory)
            .Where(p => p.AttractionId == attractionId)
            .OrderBy(p => p.SortOrder)
            .ToListAsync();

        return products.Select(MapToDetail);
    }

    public async Task<ProductOptionDetailResponse> GetByIdAsync(Guid productId)
    {
        var product = await _db.ProductOptions
            .Include(p => p.PriceTiers.Where(pt => pt.IsActive))
                .ThenInclude(pt => pt.TicketCategory)
            .FirstOrDefaultAsync(p => p.Id == productId)
            ?? throw new NotFoundException("Modalidad", productId);

        return MapToDetail(product);
    }

    // ── CREACIÓN ─────────────────────────────────────────────────

    public async Task<Guid> CreateAsync(CreateProductOptionRequest request)
    {
        // Verificar que la atracción existe
        var exists = await _db.Attractions.AnyAsync(a => a.Id == request.AttractionId);
        if (!exists)
            throw new NotFoundException("Atracción", request.AttractionId);

        var slug = GenerateSlug(request.Title);
        // Asegurar slug único dentro de la atracción
        var slugExists = await _db.ProductOptions
            .AnyAsync(p => p.AttractionId == request.AttractionId && p.Slug == slug);
        if (slugExists) slug = $"{slug}-{Guid.NewGuid().ToString()[..4]}";

        var product = new ProductOption
        {
            Id               = Guid.NewGuid(),
            AttractionId     = request.AttractionId,
            Title            = request.Title,
            Slug             = slug,
            Description      = request.Description,
            DurationMinutes  = request.DurationMinutes,
            DurationDescription = request.DurationDescription,
            CancelPolicyHours   = request.CancelPolicyHours,
            CancelPolicyText    = request.CancelPolicyText,
            MaxGroupSize     = request.MaxGroupSize,
            MinParticipants  = request.MinParticipants,
            IsPrivate        = request.IsPrivate,
            IsActive         = true
        };

        foreach (var tier in request.PriceTiers)
        {
            product.PriceTiers.Add(new PriceTier
            {
                TicketCategoryId = tier.TicketCategoryId,
                Price            = tier.Price,
                CurrencyCode     = tier.CurrencyCode,
                IsActive         = true
            });
        }

        await _uow.ProductOptions.AddAsync(product);
        await _uow.CompleteAsync();
        return product.Id;
    }

    // ── EDICIÓN ──────────────────────────────────────────────────

    public async Task UpdateAsync(Guid productId, UpdateProductOptionRequest request)
    {
        var product = await _db.ProductOptions.FindAsync(productId)
            ?? throw new NotFoundException("Modalidad", productId);

        product.Title               = request.Title;
        product.Description         = request.Description;
        product.DurationMinutes     = request.DurationMinutes;
        product.DurationDescription = request.DurationDescription;
        product.CancelPolicyHours   = request.CancelPolicyHours;
        product.CancelPolicyText    = request.CancelPolicyText;
        product.MaxGroupSize        = request.MaxGroupSize;
        product.MinParticipants     = request.MinParticipants;
        product.IsPrivate           = request.IsPrivate;
        product.IsActive            = request.IsActive;

        await _uow.CompleteAsync();
    }

    // ── ELIMINACIÓN ──────────────────────────────────────────────

    public async Task<bool> DeleteAsync(Guid productId)
    {
        var product = await _db.ProductOptions.FindAsync(productId);
        if (product == null) return false;

        // Verificar que no tiene reservas activas en sus slots
        var hasBookings = await _db.Bookings
            .Include(b => b.AvailabilitySlot)
            .AnyAsync(b => b.AvailabilitySlot.ProductId == productId && b.StatusId != 4);

        if (hasBookings)
            throw new BusinessException("No se puede eliminar esta modalidad porque tiene reservas activas.");

        _db.ProductOptions.Remove(product);
        await _uow.CompleteAsync();
        return true;
    }

    // ── TOGGLE ACTIVO ─────────────────────────────────────────────

    public async Task<bool> ToggleActiveAsync(Guid productId, bool isActive)
    {
        var product = await _db.ProductOptions.FindAsync(productId);
        if (product == null) return false;

        product.IsActive = isActive;
        await _uow.CompleteAsync();
        return true;
    }

    // ── HELPERS ──────────────────────────────────────────────────

    private static string GenerateSlug(string title)
    {
        return title.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("á","a").Replace("é","e").Replace("í","i").Replace("ó","o").Replace("ú","u")
            .Replace("ñ","n")
            .Replace("'", "").Replace("\"", "")
            .Trim('-');
    }

    private static ProductOptionDetailResponse MapToDetail(ProductOption p) => new()
    {
        Id               = p.Id,
        AttractionId     = p.AttractionId,
        Slug             = p.Slug,
        Title            = p.Title,
        Description      = p.Description,
        DurationMinutes  = p.DurationMinutes,
        DurationDescription = p.DurationDescription,
        CancelPolicyHours   = p.CancelPolicyHours,
        CancelPolicyText    = p.CancelPolicyText,
        MaxGroupSize     = p.MaxGroupSize,
        MinParticipants  = p.MinParticipants,
        IsPrivate        = p.IsPrivate,
        IsActive         = p.IsActive,
        PriceTiers       = p.PriceTiers.Select(t => new PriceTierResponse
        {
            Id               = t.Id,
            TicketCategoryId = t.TicketCategoryId,
            CategoryName     = t.TicketCategory?.Name ?? "",
            Price            = t.Price,
            CurrencyCode     = t.CurrencyCode
        }).ToList()
    };
}
