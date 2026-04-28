namespace Servicio.Atraccion.DataAccess.Entities;

public class ProductInclusion
{
    public Guid ProductId { get; set; }
    public Guid InclusionItemId { get; set; }
    public string Type { get; set; } = string.Empty;

    public virtual ProductOption Product { get; set; } = null!;
    public virtual InclusionItem InclusionItem { get; set; } = null!;
}
