namespace PharmacyERP.Domain.Entities.Catalog;

public class Product
{
    public Guid OrganizationId { get; set; }
    public Guid ProductId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool LotTrackingRequired { get; set; }
}
