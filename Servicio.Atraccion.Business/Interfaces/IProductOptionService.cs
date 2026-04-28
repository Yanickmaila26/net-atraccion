using Servicio.Atraccion.Business.DTOs.Attraction;

namespace Servicio.Atraccion.Business.Interfaces;

/// <summary>
/// Gestiona las modalidades (ProductOption) de una atracción de forma independiente.
/// Permite crear, editar, listar y eliminar modalidades sin tocar la atracción completa.
/// </summary>
public interface IProductOptionService
{
    Task<IEnumerable<ProductOptionDetailResponse>> GetByAttractionAsync(Guid attractionId);
    Task<ProductOptionDetailResponse> GetByIdAsync(Guid productId);
    Task<Guid> CreateAsync(CreateProductOptionRequest request);
    Task UpdateAsync(Guid productId, UpdateProductOptionRequest request);
    Task<bool> DeleteAsync(Guid productId);
    Task<bool> ToggleActiveAsync(Guid productId, bool isActive);
}
