using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Servicio.Atraccion.DataAccess.Entities;

namespace Servicio.Atraccion.DataAccess.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Booking");
        builder.HasKey(b => b.Id);
        builder.HasIndex(b => b.PnrCode).IsUnique();

        builder.Property(b => b.PnrCode).HasMaxLength(8).IsRequired();
        builder.Property(b => b.TotalAmount).HasPrecision(12, 2);
        builder.Property(b => b.CurrencyCode).HasMaxLength(3).HasDefaultValue("USD");
        builder.Property(b => b.StatusId).HasDefaultValue((short)1);
        builder.Property(b => b.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        builder.Property(b => b.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(b => b.User)
               .WithMany(u => u.Bookings)
               .HasForeignKey(b => b.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.AvailabilitySlot)
               .WithMany(s => s.Bookings)
               .HasForeignKey(b => b.SlotId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Status)
               .WithMany(st => st.Bookings)
               .HasForeignKey(b => b.StatusId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Review)
               .WithOne(r => r.Booking)
               .HasForeignKey<Review>(r => r.BookingId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(b => b.Details)
               .WithOne(d => d.Booking)
               .HasForeignKey(d => d.BookingId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.Payments)
               .WithOne(p => p.Booking)
               .HasForeignKey(p => p.BookingId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class BookingDetailConfiguration : IEntityTypeConfiguration<BookingDetail>
{
    public void Configure(EntityTypeBuilder<BookingDetail> builder)
    {
        builder.ToTable("BookingDetail");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(d => d.LastName).HasMaxLength(100).IsRequired();
        builder.Property(d => d.DocumentType).HasMaxLength(20);
        builder.Property(d => d.DocumentNumber).HasMaxLength(50).IsRequired();
        builder.Property(d => d.UnitPrice).HasPrecision(12, 2);
        builder.Property(d => d.Quantity).HasDefaultValue((short)1);
        builder.Property(d => d.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(d => d.PriceTier)
               .WithMany(pt => pt.BookingDetails)
               .HasForeignKey(d => d.PriceTierId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}