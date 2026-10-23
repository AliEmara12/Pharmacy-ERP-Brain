namespace PharmacyERP.Domain.Entities.Catalog;
public class ProductUom
{
	public Guid OrganizationId { get; set; }
	public Guid ProductId { get; set; }
	public Guid ProductUomId { get; set; }
	public string BaseUomCode { get; set; } = string.Empty;
	public decimal ConversionToBase { get; set; } = 1m;
	public bool IsBase { get; set; } = true;
}