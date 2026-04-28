using Servicio.Atraccion.DataAccess.Common;
using Servicio.Atraccion.DataAccess.Entities;
using Servicio.Atraccion.DataAccess.Queries.Models; // Importamos el DTO aquí

namespace Servicio.Atraccion.DataAccess.Queries;

public interface IAttractionQueries
{
    Task<PagedResult<Attraction>> SearchAttractionsAsync(QueryFilters filters);

    // Ahora el contrato es limpio y fuertemente tipado
    Task<IEnumerable<AvailabilitySummaryDto>> GetAttractionAvailabilitySummaryAsync(Guid attractionId);

    Task<IEnumerable<Attraction>> GetTopRatedAttractionsAsync(int count);
}