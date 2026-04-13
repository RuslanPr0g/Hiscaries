using FsCheck;
using FsCheck.Fluent;
using Hiscary.IntegrationTesting.Security;

namespace Hiscary.IntegrationTesting.PropertyBased;

public static class SecurityArbitraries
{
    public static Arbitrary<EndpointDescriptor> ProtectedEndpoints()
    {
        var gen = Gen.Elements(SecurityEndpointCatalog.ProtectedEndpoints);
        return gen.ToArbitrary();
    }

    public static Arbitrary<EndpointDescriptor> PublisherOnlyEndpoints()
    {
        var gen = Gen.Elements(SecurityEndpointCatalog.PublisherOnlyEndpoints);
        return gen.ToArbitrary();
    }
}
