using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Servicio.Atraccion.DataAccess.Entities;

namespace Servicio.Atraccion.DataAccess.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payment");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount).HasPrecision(12, 2);
        builder.Property(p => p.CurrencyCode).HasMaxLength(3).HasDefaultValue("USD");
        builder.Property(p => p.TransactionExternalId).HasMaxLength(100);
        builder.Property(p => p.StatusId).HasDefaultValue((short)1);
        builder.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        builder.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(p => p.PaymentMethod)
               .WithMany(pm => pm.Payments)
               .HasForeignKey(p => p.PaymentMethodId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Status)
               .WithMany(ps => ps.Payments)
               .HasForeignKey(p => p.StatusId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Review", t => t.HasCheckConstraint("CK_Review_Score",
            "[OverallScore] BETWEEN 1.00 AND 5.00"));
        builder.HasKey(r => r.Id);
        builder.HasIndex(r => r.BookingId).IsUnique();

        builder.Property(r => r.OverallScore).HasPrecision(3, 2);
        builder.Property(r => r.Title).HasMaxLength(150);
        builder.Property(r => r.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        builder.Property(r => r.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        builder.Property(r => r.IsVisible).HasDefaultValue(true);
        builder.Property(r => r.IsVerified).HasDefaultValue(true);

        builder.HasOne(r => r.User)
               .WithMany(u => u.Reviews)
               .HasForeignKey(r => r.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.Ratings)
               .WithOne(rr => rr.Review)
               .HasForeignKey(rr => rr.ReviewId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Media)
               .WithOne(rm => rm.Review)
               .HasForeignKey(rm => rm.ReviewId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}