namespace PharmacyERP.Domain.Entities.Identity;

public class Branch
{
	public Guid OrganizationId { get; set; }
	public Guid BranchId { get; set; }
	public string TimeZoneId { get; set; } = "UTC";
}
