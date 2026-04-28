using Servicio.Atraccion.Business.DTOs.User;
using Servicio.Atraccion.DataAccess.Common;

namespace Servicio.Atraccion.Business.Interfaces;

public interface IUserService
{
    Task<UserSummaryResponse> CreateUserAsync(CreateUserRequest request);
    Task<PagedResult<UserSummaryResponse>> GetUsersAsync(UserSearchRequest request);
    Task<bool> UpdateStatusAsync(Guid id, bool isActive);
    Task<bool> DeleteUserAsync(Guid id);
}
