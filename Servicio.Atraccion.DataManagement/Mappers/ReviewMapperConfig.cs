using Mapster;
using Servicio.Atraccion.DataAccess.Entities;
using Servicio.Atraccion.DataManagement.Models;

namespace Servicio.Atraccion.DataManagement.Mappers;

public class ReviewMapperConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Review, ReviewNode>()
            .Map(dest => dest.ClientName, src => src.Booking != null && src.Booking.User != null && src.Booking.User.Client != null 
                ? $"{src.Booking.User.Client.FirstName} {src.Booking.User.Client.LastName}" 
                : string.Empty)
            .Map(dest => dest.AttractionName, src => src.Booking != null && src.Booking.AvailabilitySlot != null && src.Booking.AvailabilitySlot.ProductOption != null && src.Booking.AvailabilitySlot.ProductOption.Attraction != null 
                ? src.Booking.AvailabilitySlot.ProductOption.Attraction.Name 
                : string.Empty)
            .Map(dest => dest.Rating, src => (byte)src.OverallScore)
            .Map(dest => dest.Ratings, src => src.Ratings);

        config.NewConfig<ReviewRating, ReviewRatingNode>()
            .Map(dest => dest.Criteria, src => src.Criteria != null ? src.Criteria.Name : string.Empty)
            .Map(dest => dest.Rating, src => (byte)src.Score);

        config.NewConfig<ReviewNode, Review>()
            .Map(dest => dest.OverallScore, src => (decimal)src.Rating)
            .Map(dest => dest.Ratings, src => src.Ratings)
            .Ignore(dest => dest.Booking!)
            .Ignore(dest => dest.User!)
            .Ignore(dest => dest.Language!)
            .Ignore(dest => dest.Media!);
            
        config.NewConfig<ReviewRatingNode, ReviewRating>()
            .Map(dest => dest.Score, src => (short)src.Rating)
            .Ignore(dest => dest.Review)
            .Ignore(dest => dest.Criteria);
    }
}
