using Mapster;
using Servicio.Atraccion.DataAccess.Entities;
using Servicio.Atraccion.DataManagement.Models;

namespace Servicio.Atraccion.DataManagement.Mappers;

public class InventoryMapperConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ProductOption, ProductNode>()
            .Map(dest => dest.PriceTiers, src => src.PriceTiers);

        config.NewConfig<AvailabilitySlot, AvailabilitySlotNode>();
        config.NewConfig<PriceTier, PriceTierNode>()
            .Map(dest => dest.CategoryName, src => src.TicketCategory != null ? src.TicketCategory.Name : string.Empty);
    }
}
