#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Component;
using TheXDS.Triton.Models;
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

public class AuthenticationBrokerTests
{
    private readonly IUserService _svc = new TestUserService();

    private IAuthenticationBroker GetNewBroker()
    {
        IMiddlewareConfigurator tc = new TransactionConfiguration();
        return new AuthenticationBroker(tc, _svc);
    }

    private static void CheckState(
        IAuthenticationBroker broker,
        SecurityObject? expectedCredential,
        bool expectedElevatedValue,
        bool expectedCanElevateValue)
    {
        Assert.AreSame(expectedCredential, broker.Credential);
        Assert.AreEqual(expectedElevatedValue, broker.Elevated);
        Assert.AreEqual(expectedCanElevateValue, broker.CanElevate());
    }


    [Test]
    public void New_instance_state_test()
    {
        IAuthenticationBroker broker = GetNewBroker();
        CheckState(broker, null, false, false);
    }

    [TestCase("root", true)]
    [TestCase("disabled", false)]
    [TestCase("elevatable", true)]
    public async Task Authenticate_test(string user, bool canElevate)
    {
        IAuthenticationBroker broker = GetNewBroker();
        var credential = (await _svc.GetCredential(user)).ReturnValue!;
        broker.Authenticate(credential);
        CheckState(broker, credential, false, canElevate);
    }

    [Test]
    public async Task Elevation_test()
    {
        IAuthenticationBroker broker = GetNewBroker();

        var elevatable = (await _svc.GetCredential("elevatable")).ReturnValue!;
        broker.Authenticate(elevatable);
        CheckState(broker, elevatable, false, true);
        Assert.AreSame(elevatable, broker.GetActor());

        var elevationResult = await broker.Elevate("root", "root".ToSecureString());
        Assert.IsTrue(elevationResult.Success);
        CheckState(broker, elevatable, true, true);
        Assert.AreNotSame(elevatable, broker.GetActor());
        Assert.AreSame(elevationResult.ReturnValue!.Credential, broker.GetActor());

    }

    [Test]
    public async Task Elevation_failure_test()
    {
        IAuthenticationBroker broker = GetNewBroker();

        var elevatable = (await _svc.GetCredential("disabled")).ReturnValue!;
        broker.Authenticate(elevatable);
        CheckState(broker, elevatable, false, false);
        Assert.AreSame(elevatable, broker.GetActor());

        var elevationResult = await broker.Elevate("root", "root".ToSecureString());
        Assert.IsFalse(elevationResult.Success);
        CheckState(broker, elevatable, false, false);
        Assert.AreSame(elevatable, broker.GetActor());
        Assert.AreEqual(FailureReason.Forbidden, elevationResult.Reason);
    }

    [Test]
    public async Task Revoke_elevation_test()
    {
        IAuthenticationBroker broker = GetNewBroker();

        var elevatable = (await _svc.GetCredential("elevatable")).ReturnValue!;
        broker.Authenticate(elevatable);
        await broker.Elevate("root", "root".ToSecureString());
        CheckState(broker, elevatable, true, true);

        broker.RevokeElevation();
        CheckState(broker, elevatable, false, true);
    }
}