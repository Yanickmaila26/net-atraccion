using Servicio.Atraccion.Business.DTOs.Cliente;

namespace Servicio.Atraccion.Business.Interfaces;

public interface IClienteService
{
    Task<ClienteResponse> RegistrarClienteAsync(CrearClienteRequest request);
    Task<ClienteResponse> ObtenerPorIdAsync(Guid id);
    Task<ClienteResponse> ActualizarClienteAsync(Guid userId, ActualizarClienteRequest request);
    Task<DataAccess.Common.PagedResult<ClienteResponse>> BuscarClientesAsync(ClienteFiltroRequest request);
    Task<bool> EliminarClienteAsync(Guid id);
}