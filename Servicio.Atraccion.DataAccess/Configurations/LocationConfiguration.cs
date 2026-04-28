using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Servicio.Atraccion.DataAccess.Entities;

namespace Servicio.Atraccion.DataAccess.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("Locations");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Name).HasMaxLength(100).IsRequired();
        builder.Property(l => l.Type).HasMaxLength(50).IsRequired();
        builder.Property(l => l.CountryCode).HasMaxLength(2);
        
        builder.Property(l => l.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        builder.Property(l => l.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

        // Relación jerárquica
        builder.HasOne(l => l.Parent)
               .WithMany(p => p.Children)
               .HasForeignKey(l => l.ParentId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
