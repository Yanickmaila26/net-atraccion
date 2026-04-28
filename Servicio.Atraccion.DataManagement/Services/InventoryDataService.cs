using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Servicio.Atraccion.DataAccess.Repositories.Interfaces;
using Servicio.Atraccion.DataManagement.Interfaces;
using Servicio.Atraccion.DataManagement.Models;

namespace Servicio.Atraccion.DataManagement.Services;

public class InventoryDataService : IInventoryDataService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public InventoryDataService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AvailabilitySlotNode>> GetAvailabilityAsync(Guid attractionId, DateTime startDate, DateTime endDate)
    {
        // En lugar de llamar repositorio genérico as-is, consultamos los slots anidados
        // O usamos repositorios específicos
        var slots = await _unitOfWork.AvailabilitySlots.Query()
            .Include(s => s.ProductOption)
            .Where(s => s.ProductOption.AttractionId == attractionId 
                        && s.SlotDate >= DateOnly.FromDateTime(startDate) 
                        && s.SlotDate <= DateOnly.FromDateTime(endDate)
                        && s.CapacityAvailable > 0
                        && s.IsActive)
            .OrderBy(s => s.SlotDate).ThenBy(s => s.StartTime)
            .ToListAsync();

        return _mapper.Map<IEnumerable<AvailabilitySlotNode>>(slots);
    }

    public async Task<IEnumerable<ProductNode>> GetProductsAsync(Guid attractionId, short? languageId = null)
    {
        var products = await _unitOfWork.ProductOptions.Query()
            .Include(p => p.PriceTiers.Where(pt => pt.IsActive))
                .ThenInclude(pt => pt.TicketCategory)
            .Include(p => p.Translations)
            .Where(p => p.AttractionId == attractionId && p.IsActive)
            .OrderBy(p => p.SortOrder)
            .ToListAsync();

        // Aplicar traducciones si es necesario
        if (languageId.HasValue)
        {
            foreach (var product in products)
            {
                var translation = product.Translations.FirstOrDefault(t => t.LanguageId == languageId.Value);
                if (translation != null)
                {
                    product.Title = translation.Title;
                    product.Description = translation.Description ?? product.Description;
                    product.DurationDescription = translation.DurationDescription ?? product.DurationDescription;
                    product.CancelPolicyText = translation.CancelPolicyText ?? product.CancelPolicyText;
                }
            }
        }

        return _mapper.Map<IEnumerable<ProductNode>>(products);
    }

    public async Task<AvailabilitySlotNode?> GetSlotByIdAsync(Guid slotId)
    {
        var slot = await _unitOfWork.AvailabilitySlots.Query()
            .Include(s => s.ProductOption)
            .FirstOrDefaultAsync(s => s.Id == slotId && s.IsActive);

        return slot == null ? null : _mapper.Map<AvailabilitySlotNode>(slot);
    }

    public async Task<bool> DecrementSlotCapacityAsync(Guid slotId, short quantity)
    {
        var slot = await _unitOfWork.AvailabilitySlots.Query()
            .FirstOrDefaultAsync(s => s.Id == slotId);

        if (slot == null || slot.CapacityAvailable < quantity)
            return false;

        slot.CapacityAvailable -= quantity;
        slot.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.AvailabilitySlots.Update(slot);

        return await _unitOfWork.CompleteAsync() > 0;
    }

    public async Task<IEnumerable<PriceTierNode>> GetPriceTiersByIdsAsync(IEnumerable<Guid> tierIds)
    {
        var ids = tierIds.ToList();
        var tiers = await _unitOfWork.PriceTiers.Query()
            .Where(t => ids.Contains(t.Id) && t.IsActive)
            .ToListAsync();

        return _mapper.Map<IEnumerable<PriceTierNode>>(tiers);
    }
}
