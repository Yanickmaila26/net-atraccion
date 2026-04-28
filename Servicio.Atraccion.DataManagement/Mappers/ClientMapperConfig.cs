using Mapster;
using Servicio.Atraccion.DataAccess.Entities;
using Servicio.Atraccion.DataManagement.Models;

namespace Servicio.Atraccion.DataManagement.Mappers;

public class ClientMapperConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Client, ClientNode>()
            .Map(dest => dest.LocationName, src => src.Location != null ? src.Location.Name : string.Empty)
            .Map(dest => dest.UserEmail, src => src.User != null ? src.User.Email : string.Empty)
            .Map(dest => dest.BirthDate, src => src.BirthDate.HasValue ? src.BirthDate.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null);
            
        config.NewConfig<ClientNode, Client>()
            .Ignore(dest => dest.User!)
            .Ignore(dest => dest.Location!)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.UpdatedAt)
            .Map(dest => dest.BirthDate, src => src.BirthDate.HasValue ? DateOnly.FromDateTime(src.BirthDate.Value) : (DateOnly?)null)
            .Ignore(dest => dest.Id!); // Generalmente no sobreescribimos el Id en un mapeo hacia DTO
    }
}
