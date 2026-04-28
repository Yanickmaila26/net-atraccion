using Servicio.Atraccion.Business.Interfaces;
using Servicio.Atraccion.DataAccess.Entities;
using Servicio.Atraccion.DataManagement.Interfaces;
using Servicio.Atraccion.DataManagement.Models;

namespace Servicio.Atraccion.Business.Services;

public class LocationService : ILocationService
{
    private readonly ILocationDataService _locationData;

    public LocationService(ILocationDataService locationData)
    {
        _locationData = locationData;
    }

    public Task<IEnumerable<LocationNode>> GetHierarchyAsync() => _locationData.GetHierarchyAsync();

    public Task<LocationNode?> GetByIdAsync(Guid id) => _locationData.GetByIdAsync(id);

    public async Task<Guid> CreateAsync(CreateLocationRequest request)
    {
        var entity = new Location
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Type = request.Type,
            ParentId = request.ParentId,
            CountryCode = request.CountryCode,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await _locationData.AddLocationAsync(entity);
    }

    public async Task<bool> UpdateAsync(Guid id, CreateLocationRequest request)
    {
        var entity = new Location
        {
            Id = id,
            Name = request.Name,
            Type = request.Type,
            ParentId = request.ParentId,
            CountryCode = request.CountryCode
        };

        return await _locationData.UpdateLocationAsync(entity);
    }

    public Task<bool> DeleteAsync(Guid id) => _locationData.DeleteLocationAsync(id);
}
