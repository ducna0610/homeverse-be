using Homeverse.Domain.Enums;
using System.Security.Claims;

namespace Homeverse.IntegrationTest.Helpers;

public class TestClaimsProvider
{
    public IList<Claim> Claims { get; }

    public TestClaimsProvider(IList<Claim> claims)
    {
        Claims = claims;
    }

    public TestClaimsProvider()
    {
        Claims = new List<Claim>();
    }

    public static TestClaimsProvider WithAdminClaims()
    {
        var provider = new TestClaimsProvider();
        provider.Claims.Add(new Claim(ClaimTypes.NameIdentifier, "1"));
        provider.Claims.Add(new Claim(ClaimTypes.Name, "Admin user"));
        provider.Claims.Add(new Claim(ClaimTypes.Role, RoleEnum.Admin.ToString()));

        return provider;
    }

    public static TestClaimsProvider WithLandlordClaims()
    {
        var provider = new TestClaimsProvider();
        provider.Claims.Add(new Claim(ClaimTypes.NameIdentifier, "2"));
        provider.Claims.Add(new Claim(ClaimTypes.Name, RoleEnum.Landlord.ToString()));

        return provider;
    }
}
