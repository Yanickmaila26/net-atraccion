namespace Servicio.Atraccion.DataAccess.Entities;

/// <summary>
/// Catálogo de idiomas ISO 639-1.
/// Tabla de referencia — PK SMALLINT por rendimiento.
/// </summary>
public class Language
{
    public short Id { get; set; }
    public string IsoCode { get; set; } = string.Empty;   // "es", "en", "fr"
    public string Name { get; set; } = string.Empty;       // "Español", "English"
    public bool IsActive { get; set; } = true;

    // Navegación
    public virtual ICollection<AttractionTranslation> AttractionTranslations { get; set; } = [];
    public virtual ICollection<AttractionLanguage> AttractionLanguages { get; set; } = [];
    public virtual ICollection<AudioGuide> AudioGuides { get; set; } = [];
    public virtual ICollection<TourItinerary> TourItineraries { get; set; } = [];
    public virtual ICollection<ProductTranslation> ProductTranslations { get; set; } = [];
    public virtual ICollection<Client> Clients { get; set; } = [];
}
