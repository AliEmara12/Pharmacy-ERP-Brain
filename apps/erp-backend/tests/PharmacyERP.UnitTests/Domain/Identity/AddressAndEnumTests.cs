using PharmacyERP.Domain.Enums;
using PharmacyERP.Domain.Exceptions;
using PharmacyERP.Domain.ValueObjects;

namespace PharmacyERP.UnitTests.Domain.Identity;

public class AddressAndEnumTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Address_WithoutLine1_ThrowsInvalidAddressException(string? line1)
    {
        Assert.Throws<InvalidAddressException>(() => new Address(line1!, null, "Zagazig", null, null, "EG"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Address_WithoutCity_ThrowsInvalidAddressException(string? city)
    {
        Assert.Throws<InvalidAddressException>(() => new Address("12 Main Street", null, city!, null, null, "EG"));
    }

    [Theory]
    [InlineData("EGY")]
    [InlineData("E")]
    [InlineData("")]
    [InlineData("   ")]
    public void Address_WithCountryCodeNotExactlyTwoCharacters_ThrowsInvalidAddressException(string countryCode)
    {
        Assert.Throws<InvalidAddressException>(() => new Address("12 Main Street", null, "Zagazig", null, null, countryCode));
    }

    [Fact]
    public void Address_WithValidData_NormalizesCountryCodeToUppercase()
    {
        var address = new Address("12 Main Street", "Apartment 2", "Zagazig", "Sharqia", "44511", " eg ");

        Assert.Equal("12 Main Street", address.Line1);
        Assert.Equal("Apartment 2", address.Line2);
        Assert.Equal("Zagazig", address.City);
        Assert.Equal("EG", address.CountryCode);
    }

    [Fact]
    public void UserRole_UsesSpecifiedNumericValues()
    {
        Assert.Equal(1, (int)UserRole.Admin);
        Assert.Equal(2, (int)UserRole.Pharmacist);
        Assert.Equal(3, (int)UserRole.Cashier);
    }

    [Fact]
    public void BranchScope_UsesSpecifiedNumericValues()
    {
        Assert.Equal(1, (int)BranchScope.ALL);
        Assert.Equal(2, (int)BranchScope.ASSIGNED);
    }
}
