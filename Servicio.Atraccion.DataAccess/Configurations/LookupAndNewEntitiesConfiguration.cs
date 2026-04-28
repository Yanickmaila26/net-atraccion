using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Servicio.Atraccion.DataAccess.Entities;

namespace Servicio.Atraccion.DataAccess.Configurations;

// ── Lookup Tables (con HasData seed) ────────────────────

public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable("Language");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedOnAdd();
        builder.HasIndex(l => l.IsoCode).IsUnique();
        builder.Property(l => l.IsoCode).HasMaxLength(5).IsRequired();
        builder.Property(l => l.Name).HasMaxLength(50).IsRequired();

        builder.HasData(
            new Language { Id = 1, IsoCode = "es", Name = "Español" },
            new Language { Id = 2, IsoCode = "en", Name = "English" },
            new Language { Id = 3, IsoCode = "fr", Name = "Français" },
            new Language { Id = 4, IsoCode = "pt", Name = "Português" },
            new Language { Id = 5, IsoCode = "de", Name = "Deutsch" },
            new Language { Id = 6, IsoCode = "it", Name = "Italiano" },
            new Language { Id = 7, IsoCode = "zh", Name = "中文" },
            new Language { Id = 8, IsoCode = "ja", Name = "日本語" }
        );
    }
}

public class MediaTypeConfiguration : IEntityTypeConfiguration<MediaType>
{
    public void Configure(EntityTypeBuilder<MediaType> builder)
    {
        builder.ToTable("MediaType");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Name).HasMaxLength(20).IsRequired();
        builder.HasData(
            new MediaType { Id = 1, Name = "image" },
            new MediaType { Id = 2, Name = "video" },
            new MediaType { Id = 3, Name = "document" }
        );
    }
}

public class BookingStatusConfiguration : IEntityTypeConfiguration<BookingStatus>
{
    public void Configure(EntityTypeBuilder<BookingStatus> builder)
    {
        builder.ToTable("BookingStatus");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).HasMaxLength(20).IsRequired();
        builder.HasData(
            new BookingStatus { Id = 1, Name = "Pending" },
            new BookingStatus { Id = 2, Name = "Confirmed" },
            new BookingStatus { Id = 3, Name = "Completed" },
            new BookingStatus { Id = 4, Name = "Cancelled" },
            new BookingStatus { Id = 5, Name = "NoShow" }
        );
    }
}

public class PaymentMethodTypeConfiguration : IEntityTypeConfiguration<PaymentMethodType>
{
    public void Configure(EntityTypeBuilder<PaymentMethodType> builder)
    {
        builder.ToTable("PaymentMethodType");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).HasMaxLength(30).IsRequired();
        builder.HasData(
            new PaymentMethodType { Id = 1, Name = "Card" },
            new PaymentMethodType { Id = 2, Name = "Transfer" },
            new PaymentMethodType { Id = 3, Name = "Cash" },
            new PaymentMethodType { Id = 4, Name = "PayPal" },
            new PaymentMethodType { Id = 5, Name = "Crypto" }
        );
    }
}

public class PaymentStatusTypeConfiguration : IEntityTypeConfiguration<PaymentStatusType>
{
    public void Configure(EntityTypeBuilder<PaymentStatusType> builder)
    {
        builder.ToTable("PaymentStatusType");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).HasMaxLength(20).IsRequired();
        builder.HasData(
            new PaymentStatusType { Id = 1, Name = "Pending" },
            new PaymentStatusType { Id = 2, Name = "Succeeded" },
            new PaymentStatusType { Id = 3, Name = "Failed" },
            new PaymentStatusType { Id = 4, Name = "Refunded" },
            new PaymentStatusType { Id = 5, Name = "Disputed" }
        );
    }
}

public class ReviewCriteriaConfiguration : IEntityTypeConfiguration<ReviewCriteria>
{
    public void Configure(EntityTypeBuilder<ReviewCriteria> builder)
    {
        builder.ToTable("ReviewCriteria");
        builder.HasKey(rc => rc.Id);
        builder.Property(rc => rc.Name).HasMaxLength(50).IsRequired();
        builder.HasData(
            new ReviewCriteria { Id = 1, Name = "Guide" },
            new ReviewCriteria { Id = 2, Name = "Punctuality" },
            new ReviewCriteria { Id = 3, Name = "ValueForMoney" },
            new ReviewCriteria { Id = 4, Name = "Safety" },
            new ReviewCriteria { Id = 5, Name = "Cleanliness" },
            new ReviewCriteria { Id = 6, Name = "Organization" }
        );
    }
}

public class TicketCategoryConfiguration : IEntityTypeConfiguration<TicketCategory>
{
    public void Configure(EntityTypeBuilder<TicketCategory> builder)
    {
        builder.ToTable("TicketCategory");
        builder.HasKey(tc => tc.Id);
        builder.Property(tc => tc.Name).HasMaxLength(50).IsRequired();
        builder.Property(tc => tc.NameEn).HasMaxLength(50);

        builder.HasData(
            new TicketCategory { Id = Guid.Parse("A1B2C3D4-E5F6-4A1B-8C9D-0E1F2A3B4C5D"), Name = "Adulto", NameEn = "Adult", AgeRangeMin = 13, SortOrder = 1 },
            new TicketCategory { Id = Guid.Parse("B2C3D4E5-F6A1-4B2C-9D0E-1F2A3B4C5D6E"), Name = "Niño", NameEn = "Child", AgeRangeMin = 5, AgeRangeMax = 12, SortOrder = 2 },
            new TicketCategory { Id = Guid.Parse("C3D4E5F6-A1B2-4C3D-0E1F-2A3B4C5D6E7F"), Name = "Bebé", NameEn = "Infant", AgeRangeMax = 4, SortOrder = 3 },
            new TicketCategory { Id = Guid.Parse("D4E5F6A1-B2C3-4D4E-1F2A-3B4C5D6E7F8A"), Name = "Senior", NameEn = "Senior", AgeRangeMin = 65, SortOrder = 4 }
        );
    }
}

// ── Tablas N-M ─────────────────────────────────────────

public class AttractionTagConfiguration : IEntityTypeConfiguration<AttractionTag>
{
    public void Configure(EntityTypeBuilder<AttractionTag> builder)
    {
        builder.ToTable("AttractionTag");
        builder.HasKey(at => new { at.AttractionId, at.TagId });
    }
}

public class AttractionInclusionConfiguration : IEntityTypeConfiguration<AttractionInclusion>
{
    public void Configure(EntityTypeBuilder<AttractionInclusion> builder)
    {
        builder.ToTable("AttractionInclusion", t => t.HasCheckConstraint("CK_AttrIncl_Type", "type IN ('included','not_included','optional','bring')"));
        builder.HasKey(ai => new { ai.AttractionId, ai.InclusionItemId });
        builder.Property(ai => ai.Type).HasMaxLength(20).IsRequired();
    }
}

public class ProductInclusionConfiguration : IEntityTypeConfiguration<ProductInclusion>
{
    public void Configure(EntityTypeBuilder<ProductInclusion> builder)
    {
        builder.ToTable("ProductInclusion", t => t.HasCheckConstraint("CK_ProdIncl_Type", "type IN ('included','not_included','optional','bring')"));
        builder.HasKey(pi => new { pi.ProductId, pi.InclusionItemId });
        builder.Property(pi => pi.Type).HasMaxLength(20).IsRequired();
    }
}

// ── Configuraciones simples ─────────────────────────────

public class AttractionTranslationConfiguration : IEntityTypeConfiguration<AttractionTranslation>
{
    public void Configure(EntityTypeBuilder<AttractionTranslation> builder)
    {
        builder.ToTable("AttractionTranslation");
        builder.HasKey(t => t.Id);
        builder.HasIndex(t => new { t.AttractionId, t.LanguageId }).IsUnique();
        builder.Property(t => t.Name).HasMaxLength(150).IsRequired();
        builder.Property(t => t.DescriptionShort).HasMaxLength(255);
    }
}

public class AttractionLanguageConfiguration : IEntityTypeConfiguration<AttractionLanguage>
{
    public void Configure(EntityTypeBuilder<AttractionLanguage> builder)
    {
        builder.ToTable("AttractionLanguage", t => t.HasCheckConstraint("CK_AttrLang_GuideType",
            "guide_type IN ('live','audio','written','app')"));
        builder.HasKey(al => al.Id);
        builder.HasIndex(al => new { al.AttractionId, al.LanguageId, al.GuideType }).IsUnique();
        builder.Property(al => al.GuideType).HasMaxLength(20).IsRequired();
    }
}

public class AvailabilitySlotConfiguration : IEntityTypeConfiguration<AvailabilitySlot>
{
    public void Configure(EntityTypeBuilder<AvailabilitySlot> builder)
    {
        builder.ToTable("AvailabilitySlot", t => t.HasCheckConstraint("CK_AvailSlot_Capacity",
            "capacity_available <= capacity_total AND capacity_total > 0 AND capacity_available >= 0"));
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => new { s.ProductId, s.SlotDate, s.StartTime }).IsUnique();

        builder.Property(s => s.IsActive).HasDefaultValue(true);
        builder.Property(s => s.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        builder.Property(s => s.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
    }
}

public class PriceTierConfiguration : IEntityTypeConfiguration<PriceTier>
{
    public void Configure(EntityTypeBuilder<PriceTier> builder)
    {
        builder.ToTable("PriceTier", t => t.HasCheckConstraint("CK_PriceTier_Price", "price >= 0"));
        builder.HasKey(pt => pt.Id);
        builder.Property(pt => pt.Price).HasPrecision(12, 2);
        builder.Property(pt => pt.CurrencyCode).HasMaxLength(3).HasDefaultValue("USD");
        builder.Property(pt => pt.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(pt => pt.TicketCategory)
               .WithMany(tc => tc.PriceTiers)
               .HasForeignKey(pt => pt.TicketCategoryId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class AudioGuideConfiguration : IEntityTypeConfiguration<AudioGuide>
{
    public void Configure(EntityTypeBuilder<AudioGuide> builder)
    {
        builder.ToTable("AudioGuide");
        builder.HasKey(g => g.Id);
        builder.HasIndex(g => new { g.AttractionId, g.LanguageId }).IsUnique();
        builder.Property(g => g.Title).HasMaxLength(150).IsRequired();
        builder.Property(g => g.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        builder.HasMany(g => g.Stops)
               .WithOne(s => s.AudioGuide)
               .HasForeignKey(s => s.AudioGuideId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class AudioGuideStopConfiguration : IEntityTypeConfiguration<AudioGuideStop>
{
    public void Configure(EntityTypeBuilder<AudioGuideStop> builder)
    {
        builder.ToTable("AudioGuideStop");
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => new { s.AudioGuideId, s.StopNumber }).IsUnique();
        builder.Property(s => s.Title).HasMaxLength(150).IsRequired();
        builder.Property(s => s.AudioUrl).IsRequired();
        builder.Property(s => s.Latitude).HasPrecision(9, 6);
        builder.Property(s => s.Longitude).HasPrecision(9, 6);
    }
}

public class TourItineraryConfiguration : IEntityTypeConfiguration<TourItinerary>
{
    public void Configure(EntityTypeBuilder<TourItinerary> builder)
    {
        builder.ToTable("TourItinerary");
        builder.HasKey(ti => ti.Id);
        builder.HasIndex(ti => new { ti.AttractionId, ti.LanguageId }).IsUnique();
        builder.Property(ti => ti.Title).HasMaxLength(150).IsRequired();
        builder.Property(ti => ti.TotalDistanceKm).HasPrecision(6, 2);
        builder.Property(ti => ti.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        builder.HasMany(ti => ti.Stops)
               .WithOne(s => s.Itinerary)
               .HasForeignKey(s => s.ItineraryId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class TourStopConfiguration : IEntityTypeConfiguration<TourStop>
{
    public void Configure(EntityTypeBuilder<TourStop> builder)
    {
        builder.ToTable("TourStop", t => t.HasCheckConstraint("CK_TourStop_Admission",
            "admission_type IS NULL OR admission_type IN ('included','optional','excluded','bring')"));
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => new { s.ItineraryId, s.StopNumber }).IsUnique();
        builder.Property(s => s.Name).HasMaxLength(150).IsRequired();
        builder.Property(s => s.AdmissionType).HasMaxLength(20);
        builder.Property(s => s.Latitude).HasPrecision(9, 6);
        builder.Property(s => s.Longitude).HasPrecision(9, 6);
    }
}

public class ProductOptionConfiguration : IEntityTypeConfiguration<ProductOption>
{
    public void Configure(EntityTypeBuilder<ProductOption> builder)
    {
        builder.ToTable("ProductOption");
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => new { p.AttractionId, p.Slug }).IsUnique();
        builder.Property(p => p.Title).HasMaxLength(150).IsRequired();
        builder.Property(p => p.Slug).HasMaxLength(150).IsRequired();
        builder.Property(p => p.DurationDescription).HasMaxLength(100);
        builder.Property(p => p.CancelPolicyHours).HasDefaultValue(24);
        builder.Property(p => p.MinParticipants).HasDefaultValue((short)1);
        builder.Property(p => p.IsActive).HasDefaultValue(true);
        builder.Property(p => p.IsPrivate).HasDefaultValue(false);
        builder.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        builder.HasMany(p => p.Translations)
               .WithOne(t => t.Product)
               .HasForeignKey(t => t.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.AvailabilitySlots)
               .WithOne(s => s.ProductOption)
               .HasForeignKey(s => s.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.PriceTiers)
               .WithOne(pt => pt.ProductOption)
               .HasForeignKey(pt => pt.ProductId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLog", t => t.HasCheckConstraint("CK_AuditLog_Action",
            "action IN ('INSERT','UPDATE','DELETE')"));
        builder.HasKey(a => a.Id);
        builder.Property(a => a.TableName).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Action).HasMaxLength(10).IsRequired();
        builder.Property(a => a.ChangedBy).HasMaxLength(256);
        builder.Property(a => a.IpAddress).HasMaxLength(45);
        builder.Property(a => a.UserAgent).HasMaxLength(500);
        builder.Property(a => a.Endpoint).HasMaxLength(255);
        builder.Property(a => a.ChangedAt).HasDefaultValueSql("GETUTCDATE()");
    }
}
