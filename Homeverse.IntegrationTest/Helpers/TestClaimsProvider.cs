using Homeverse.Domain.Enums;
using System.Runtime.CompilerServices;
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

    public static TestClaimsProvider WithAdminClaims(string id = "1")
    {
        var provider = new TestClaimsProvider();
        provider.Claims.Add(new Claim(ClaimTypes.NameIdentifier, id));
        provider.Claims.Add(new Claim(ClaimTypes.Name, "Admin user"));
        provider.Claims.Add(new Claim(ClaimTypes.Role, RoleEnum.Admin.ToString()));

        return provider;
    }

    public static TestClaimsProvider WithLandlordClaims(string id = "1")
    {
        var provider = new TestClaimsProvider();
        provider.Claims.Add(new Claim(ClaimTypes.NameIdentifier, id));
        provider.Claims.Add(new Claim(ClaimTypes.Name, "Landlord user"));
        provider.Claims.Add(new Claim(ClaimTypes.Name, RoleEnum.Landlord.ToString()));

        return provider;
    }
}
