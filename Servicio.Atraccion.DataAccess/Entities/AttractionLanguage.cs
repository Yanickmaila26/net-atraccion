namespace Servicio.Atraccion.DataAccess.Entities;

/// <summary>
/// Indica en qué idiomas está disponible la guía de la atracción
/// y de qué tipo es (en vivo, audio, escrita, app).
/// </summary>
public class AttractionLanguage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AttractionId { get; set; }
    public short LanguageId { get; set; }

    /// <summary>"live" | "audio" | "written" | "app"</summary>
    public string GuideType { get; set; } = string.Empty;

    public virtual Attraction Attraction { get; set; } = null!;
    public virtual Language Language { get; set; } = null!;
}
