using Servicio.Atraccion.DataAccess.Entities;

namespace Servicio.Atraccion.DataManagement.Interfaces;

public interface ICategoryDataService
{
    Task<IEnumerable<Category>> GetAllWithSubcategoriesAsync();
    Task<Category?> GetByIdAsync(Guid id);
    Task<Guid> AddCategoryAsync(Category category);
    Task<bool> UpdateCategoryAsync(Category category);
    Task<bool> DeleteCategoryAsync(Guid id);

    Task<Guid> AddSubcategoryAsync(Subcategory subcategory);
    Task<bool> UpdateSubcategoryAsync(Subcategory subcategory);
    Task<bool> DeleteSubcategoryAsync(Guid id);
}
