

namespace PharmacyERP.Application.Common.Interfaces
{
    public interface IDateTimeProvider
    {
        DateTimeOffset UtcNow { get; }
        DateOnly GetOrganizationBusinessDate(string ianaTimeZoneId);
    }
}
