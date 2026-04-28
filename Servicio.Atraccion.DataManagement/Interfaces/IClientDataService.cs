using Servicio.Atraccion.DataAccess.Common;
using Servicio.Atraccion.DataManagement.Models;

namespace Servicio.Atraccion.DataManagement.Interfaces;

public interface IClientDataService
{
    Task<ClientNode?> GetByIdAsync(Guid id);
    Task<ClientNode?> GetByDocumentNumberAsync(string documentNumber);
    Task<ClientNode?> GetByUserIdAsync(Guid userId);
    Task<PagedResult<ClientNode>> SearchAsync(QueryFilters filters);
    
    Task<bool> CreateAsync(ClientNode clientNode);
    Task<bool> UpdateAsync(ClientNode clientNode);
    Task<bool> DeleteAsync(Guid id);
}
