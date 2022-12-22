#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Component;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.SecurityEssentials;

public class AuthenticatedServiceTests
{
    private class TestAuthService : AuthenticatedService
    {
        public TestAuthService() : base(new TestUserService())
        {
        }
    }

    [Test]
    public void Class_exposes_broker_test()
    {
        var service = new TestAuthService();
        Assert.IsNotNull(service.AuthenticationBroker);
        Assert.IsTrue(service.AuthenticationBroker.GetType().Implements<IAuthenticationBroker>());
    }
}
