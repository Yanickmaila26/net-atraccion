using Mapster;
using Servicio.Atraccion.DataAccess.Entities;
using Servicio.Atraccion.DataManagement.Models;

namespace Servicio.Atraccion.DataManagement.Mappers;

public class BookingMapperConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Booking, BookingNode>()
            .Map(dest => dest.StatusName, src => src.Status != null ? src.Status.Name : string.Empty)
            .Map(dest => dest.UserEmail, src => src.User != null ? src.User.Email : string.Empty)
            .Map(dest => dest.ClientName, src => src.User != null && src.User.Client != null ? src.User.Client.FirstName + " " + src.User.Client.LastName : string.Empty)
            .Map(dest => dest.AttractionId, src => src.AvailabilitySlot != null && src.AvailabilitySlot.ProductOption != null ? src.AvailabilitySlot.ProductOption.AttractionId : Guid.Empty)
            .Map(dest => dest.AttractionName, src => src.AvailabilitySlot != null && src.AvailabilitySlot.ProductOption != null && src.AvailabilitySlot.ProductOption.Attraction != null ? src.AvailabilitySlot.ProductOption.Attraction.Name : string.Empty)
            .Map(dest => dest.AttractionSlug, src => src.AvailabilitySlot != null && src.AvailabilitySlot.ProductOption != null && src.AvailabilitySlot.ProductOption.Attraction != null ? src.AvailabilitySlot.ProductOption.Attraction.Slug : string.Empty)
            .Map(dest => dest.ProductTitle, src => src.AvailabilitySlot != null && src.AvailabilitySlot.ProductOption != null ? src.AvailabilitySlot.ProductOption.Title : string.Empty)
            .Map(dest => dest.SlotDate, src => src.AvailabilitySlot != null ? src.AvailabilitySlot.SlotDate : default)
            .Map(dest => dest.SlotStartTime, src => src.AvailabilitySlot != null ? src.AvailabilitySlot.StartTime : default)
            .Map(dest => dest.Details, src => src.Details);

        config.NewConfig<BookingDetail, BookingDetailNode>()
            .Map(dest => dest.PriceTierLabel, src => src.PriceTier != null && src.PriceTier.TicketCategory != null ? src.PriceTier.TicketCategory.Name : string.Empty);
            
        config.NewConfig<BookingNode, Booking>()
            .Ignore(dest => dest.Status)
            .Ignore(dest => dest.User)
            .Ignore(dest => dest.AvailabilitySlot)
            .Map(dest => dest.Details, src => src.Details);
            
        config.NewConfig<BookingDetailNode, BookingDetail>()
            .Ignore(dest => dest.PriceTier)
            .Ignore(dest => dest.Booking)
            .Ignore(dest => dest.CreatedAt);
    }
}
