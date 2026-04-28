using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Servicio.Atraccion.DataAccess.Common;
using Servicio.Atraccion.DataAccess.Entities;
using System.Text.Json;

namespace Servicio.Atraccion.DataAccess.Context;

public class AtraccionDbContext : DbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public AtraccionDbContext(
        DbContextOptions<AtraccionDbContext> options,
        IHttpContextAccessor? httpContextAccessor = null)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // ══════════════════════════════════════════════════
    // 1. MÓDULO GEOGRÁFICO
    // ══════════════════════════════════════════════════
    public DbSet<Location> Locations { get; set; }

    // ══════════════════════════════════════════════════
    // 2. IDIOMAS
    // ══════════════════════════════════════════════════
    public DbSet<Language> Languages { get; set; }

    // ══════════════════════════════════════════════════
    // 3. RBAC
    // ══════════════════════════════════════════════════
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Client> Clients { get; set; }

    // ══════════════════════════════════════════════════
    // 4. CATÁLOGO
    // ══════════════════════════════════════════════════
    public DbSet<Category> Categories { get; set; }
    public DbSet<CategoryTranslation> CategoryTranslations { get; set; }
    public DbSet<Subcategory> Subcategories { get; set; }
    public DbSet<SubcategoryTranslation> SubcategoryTranslations { get; set; }
    public DbSet<Tag> Tags { get; set; }

    // ══════════════════════════════════════════════════
    // 5. ATRACCIONES
    // ══════════════════════════════════════════════════
    public DbSet<Attraction> Attractions { get; set; }
    public DbSet<AttractionTranslation> AttractionTranslations { get; set; }
    public DbSet<AttractionLanguage> AttractionLanguages { get; set; }
    public DbSet<AttractionTag> AttractionTags { get; set; }
    public DbSet<AttractionMedia> AttractionMedias { get; set; }
    public DbSet<AttractionInclusion> AttractionInclusions { get; set; }

    // ══════════════════════════════════════════════════
    // 6. AUDIOGUÍA E ITINERARIO
    // ══════════════════════════════════════════════════
    public DbSet<AudioGuide> AudioGuides { get; set; }
    public DbSet<AudioGuideStop> AudioGuideStops { get; set; }
    public DbSet<TourItinerary> TourItineraries { get; set; }
    public DbSet<TourStop> TourStops { get; set; }
    public DbSet<TourStopMedia> TourStopMedias { get; set; }

    // ══════════════════════════════════════════════════
    // 7. INCLUSIONES
    // ══════════════════════════════════════════════════
    public DbSet<InclusionItem> InclusionItems { get; set; }
    public DbSet<MediaType> MediaTypes { get; set; }

    // ══════════════════════════════════════════════════
    // 8. INVENTARIO Y PRECIOS
    // ══════════════════════════════════════════════════
    public DbSet<ProductOption> ProductOptions { get; set; }
    public DbSet<ProductTranslation> ProductTranslations { get; set; }
    public DbSet<ProductInclusion> ProductInclusions { get; set; }
    public DbSet<AvailabilitySlot> AvailabilitySlots { get; set; }
    public DbSet<PriceTier> PriceTiers { get; set; }
    public DbSet<TicketCategory> TicketCategories { get; set; }
    public DbSet<ProductScheduleTemplate> ScheduleTemplates { get; set; }
    public DbSet<ProductScheduleTime> ScheduleTimes { get; set; }

    // ══════════════════════════════════════════════════
    // 9. RESERVAS Y PAGOS
    // ══════════════════════════════════════════════════
    public DbSet<BookingStatus> BookingStatuses { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingDetail> BookingDetails { get; set; }
    public DbSet<PaymentMethodType> PaymentMethodTypes { get; set; }
    public DbSet<PaymentStatusType> PaymentStatusTypes { get; set; }
    public DbSet<Payment> Payments { get; set; }

    // ══════════════════════════════════════════════════
    // 10. RESEÑAS
    // ══════════════════════════════════════════════════
    public DbSet<Review> Reviews { get; set; }
    public DbSet<ReviewCriteria> ReviewCriterias { get; set; }
    public DbSet<ReviewRating> ReviewRatings { get; set; }
    public DbSet<ReviewMedia> ReviewMedias { get; set; }

    // ══════════════════════════════════════════════════
    // 11. AUDITORÍA
    // ══════════════════════════════════════════════════
    public DbSet<AuditLog> AuditLogs { get; set; }

    // ══════════════════════════════════════════════════
    // MODEL CREATING
    // ══════════════════════════════════════════════════
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Aplicar configuraciones explícitas de los archivos de configuración
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AtraccionDbContext).Assembly);

        // 2. Convención Global para PostgreSQL (snake_case)
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Nombre de la tabla
            var tableName = entity.GetTableName();
            if (!string.IsNullOrEmpty(tableName))
            {
                entity.SetTableName(ToSnakeCase(tableName));
            }

            // Nombre de las columnas y Default Values
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.Name));

                var defaultValueSql = property.GetDefaultValueSql();
                if (defaultValueSql != null && defaultValueSql.Contains("GETUTCDATE()", StringComparison.OrdinalIgnoreCase))
                {
                    property.SetDefaultValueSql("now()");
                }
            }

            // Claves primarias
            foreach (var key in entity.GetKeys())
            {
                var keyName = key.GetName();
                if (!string.IsNullOrEmpty(keyName)) key.SetName(ToSnakeCase(keyName));
            }

            // Claves foráneas
            foreach (var fk in entity.GetForeignKeys())
            {
                var constraintName = fk.GetConstraintName();
                if (!string.IsNullOrEmpty(constraintName)) fk.SetConstraintName(ToSnakeCase(constraintName));
            }

            // Índices
            foreach (var index in entity.GetIndexes())
            {
                var indexName = index.GetDatabaseName();
                if (!string.IsNullOrEmpty(indexName)) index.SetDatabaseName(ToSnakeCase(indexName));
            }
        }
    }

    private string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        var startUnderscore = input.StartsWith("_");
        var res = System.Text.RegularExpressions.Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        
        return startUnderscore ? "_" + res : res;
    }

    // ══════════════════════════════════════════════════
    // SAVE WITH AUDIT
    // ══════════════════════════════════════════════════
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 1. Estampar timestamps y usuario en entidades BaseEntity
        StampAuditFields();

        // 2. Capturar audit entries ANTES del save
        var auditEntries = BuildAuditEntries();

        // 3. Persistir cambios principales
        var result = await base.SaveChangesAsync(cancellationToken);

        // 4. Persistir audit log (segundo guardado — no vuelve a auditarse)
        if (auditEntries.Count > 0)
        {
            AuditLogs.AddRange(auditEntries);
            await base.SaveChangesAsync(cancellationToken);
        }

        return result;
    }

    // ──────────────────────────────────────────────────
    private void StampAuditFields()
    {
        var currentUser = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "system";
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    // Evitar sobrescribir campos de creación
                    entry.Property(e => e.CreatedAt).IsModified = false;
                    break;

                case EntityState.Deleted:
                    // Dejamos el EF manejar el borrado
                    break;
            }
        }
    }

    // ──────────────────────────────────────────────────
    private List<AuditLog> BuildAuditEntries()
    {
        ChangeTracker.DetectChanges();

        var currentUser = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "system";
        var ipAddress   = _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        var endpoint    = _httpContextAccessor?.HttpContext?.Request?.Path.ToString();
        var userAgent   = _httpContextAccessor?.HttpContext?.Request?.Headers["User-Agent"].ToString();

        var entries = new List<AuditLog>();

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is AuditLog) continue;
            if (entry.State is EntityState.Detached or EntityState.Unchanged) continue;

            // Determinar el RecordId — se busca la propiedad "Id" de tipo Guid
            var idProp = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "Id");
            if (idProp?.CurrentValue is not Guid recordId) continue;

            var action = entry.State switch
            {
                EntityState.Added    => "INSERT",
                EntityState.Modified => "UPDATE",
                EntityState.Deleted  => "DELETE",
                _                    => null
            };

            if (action is null) continue;

            var auditEntry = new AuditLog
            {
                TableName  = entry.Metadata.GetTableName() ?? entry.Entity.GetType().Name,
                RecordId   = recordId,
                Action     = action,
                ChangedBy  = currentUser,
                ChangedAt  = DateTime.UtcNow,
                IpAddress  = ipAddress,
                UserAgent  = userAgent,
                Endpoint   = endpoint,
                OldValues  = action is "UPDATE" or "DELETE"
                    ? JsonSerializer.Serialize(
                        entry.Properties.ToDictionary(
                            p => p.Metadata.Name,
                            p => p.OriginalValue is DateOnly or TimeOnly ? p.OriginalValue.ToString() : p.OriginalValue))
                    : null,
                NewValues  = action is "INSERT" or "UPDATE"
                    ? JsonSerializer.Serialize(
                        entry.Properties.ToDictionary(
                            p => p.Metadata.Name,
                            p => p.CurrentValue is DateOnly or TimeOnly ? p.CurrentValue.ToString() : p.CurrentValue))
                    : null
            };

            entries.Add(auditEntry);
        }

        return entries;
    }
}