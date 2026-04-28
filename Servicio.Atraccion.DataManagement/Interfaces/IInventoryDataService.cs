using Servicio.Atraccion.DataManagement.Models;

namespace Servicio.Atraccion.DataManagement.Interfaces;

public interface IInventoryDataService
{
    // Obtener los slots de disponibilidad y reglas para una Atracción
    Task<IEnumerable<AvailabilitySlotNode>> GetAvailabilityAsync(Guid attractionId, DateTime startDate, DateTime endDate);
    
    // Obtener las modalidades de producto y sus respectivos precios
    Task<IEnumerable<ProductNode>> GetProductsAsync(Guid attractionId, short? languageId = null);

    // Obtener un slot específico (para verificar disponibilidad al reservar)
    Task<AvailabilitySlotNode?> GetSlotByIdAsync(Guid slotId);

    // Decrementar la capacidad disponible de un slot (dentro de una transacción)
    Task<bool> DecrementSlotCapacityAsync(Guid slotId, short quantity);

    // Obtener tiers de precio por sus IDs (para validar precios al reservar)
    Task<IEnumerable<PriceTierNode>> GetPriceTiersByIdsAsync(IEnumerable<Guid> tierIds);
}
