#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Component;
using TheXDS.Triton.Models;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.SecurityEssentials;

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
        Assert.That(expectedCredential, Is.SameAs(broker.Credential));
        Assert.That(expectedElevatedValue, Is.EqualTo(broker.Elevated));
        Assert.That(expectedCanElevateValue, Is.EqualTo(broker.CanElevate()));
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
        Assert.That(elevatable, Is.SameAs(broker.GetActor()));

        var elevationResult = await broker.Elevate("root", "root".ToSecureString());
        Assert.That(elevationResult.Success, Is.True);
        CheckState(broker, elevatable, true, true);
        Assert.That(elevatable, Is.Not.SameAs(broker.GetActor()));
        Assert.That(elevationResult.ReturnValue!.Credential, Is.SameAs(broker.GetActor()));
    }

    [Test]
    public async Task Elevation_failure_test()
    {
        IAuthenticationBroker broker = GetNewBroker();

        var elevatable = (await _svc.GetCredential("disabled")).ReturnValue!;
        broker.Authenticate(elevatable);
        CheckState(broker, elevatable, false, false);
        Assert.That(elevatable, Is.SameAs(broker.GetActor()));

        var elevationResult = await broker.Elevate("root", "root".ToSecureString());
        Assert.That(elevationResult.Success, Is.False);
        CheckState(broker, elevatable, false, false);
        Assert.That(elevatable, Is.SameAs(broker.GetActor()));
        Assert.That(elevationResult.Reason, Is.EqualTo(FailureReason.Forbidden));
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