using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Servicio.Atraccion.DataAccess.Entities;

public class PriceTierConfiguration : IEntityTypeConfiguration<PriceTier>
{
    public void Configure(EntityTypeBuilder<PriceTier> builder)
    {
        builder.Property(e => e.Price).HasPrecision(12, 2);
    }
}