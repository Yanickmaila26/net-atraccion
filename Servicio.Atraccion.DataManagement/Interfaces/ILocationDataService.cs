using Servicio.Atraccion.DataAccess.Entities;
using Servicio.Atraccion.DataManagement.Models;

namespace Servicio.Atraccion.DataManagement.Interfaces;

public interface ILocationDataService
{
    Task<IEnumerable<LocationNode>> GetHierarchyAsync();
    Task<LocationNode?> GetByIdAsync(Guid id);
    Task<Guid> AddLocationAsync(Location location);
    Task<bool> UpdateLocationAsync(Location location);
    Task<bool> DeleteLocationAsync(Guid id);
}
