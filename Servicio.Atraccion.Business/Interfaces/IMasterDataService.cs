using Servicio.Atraccion.Business.DTOs.Common;

namespace Servicio.Atraccion.Business.Interfaces;

public interface IMasterDataService
{
    Task<IEnumerable<TagResponse>> GetTagsAsync();
    Task<TagResponse?> GetTagByIdAsync(Guid id);
    Task<Guid> CreateTagAsync(CreateTagRequest request);
    Task<bool> UpdateTagAsync(Guid id, CreateTagRequest request);
    Task<bool> DeleteTagAsync(Guid id);

    Task<IEnumerable<InclusionResponse>> GetInclusionsAsync();
    Task<InclusionResponse?> GetInclusionByIdAsync(Guid id);
    Task<Guid> CreateInclusionAsync(CreateInclusionRequest request);
    Task<bool> UpdateInclusionAsync(Guid id, CreateInclusionRequest request);
    Task<bool> DeleteInclusionAsync(Guid id);

    Task<IEnumerable<LanguageResponse>> GetLanguagesAsync();
    Task<IEnumerable<TicketCategoryResponse>> GetTicketCategoriesAsync();
    Task<TicketCategoryResponse?> GetTicketCategoryByIdAsync(Guid id);
    Task<Guid> CreateTicketCategoryAsync(CreateTicketCategoryRequest request);
    Task<bool> UpdateTicketCategoryAsync(Guid id, CreateTicketCategoryRequest request);
    Task<bool> DeleteTicketCategoryAsync(Guid id);
}
