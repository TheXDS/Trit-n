#pragma warning disable CS1591

using Moq;
using NUnit.Framework;
using TheXDS.Triton.Models;
using TheXDS.Triton.Services;
using static TheXDS.Triton.Models.PermissionFlags;

namespace TheXDS.Triton.Tests.SecurityEssentials.IUserServiceTests;

public class CheckAccess
{
    private static LoginCredential GetTestSecurityObject()
    {
        return new()
        {
            Granted = Read,
            Revoked = Create,
            Membership =
            {
                new()
                {
                    Group = new()
                    {
                        Granted = Update,
                        Revoked = Delete,
                        Descriptors =
                        {
                            new()
                            {
                                ContextId = "TestC",
                                Granted = Elevate,
                                Revoked = None,
                            },
                            new()
                            {
                                ContextId = "TestD",
                                Granted = None,
                                Revoked = Lock,
                            }
                        }
                    }
                }
            },
            Descriptors =
            {
                new()
                {
                    ContextId = "TestA",
                    Granted = View,
                    Revoked = None,
                },
                new()
                {
                    ContextId = "TestB",
                    Granted = None,
                    Revoked = Export,
                }
            }
        };
    }

    [Test]
    public void CheckAccess_returns_true_for_default_granted_permissions()
    {
        var svc = new Mock<IUserService>() { CallBase = true }.Object;
        var usr = GetTestSecurityObject();
        Assert.That(svc.CheckAccess(usr, "XXX", Read).Result, Is.True);
    }

    [Test]
    public void CheckAccess_returns_false_for_default_revoked_permissions()
    {
        var svc = new Mock<IUserService>() { CallBase = true }.Object;
        var usr = GetTestSecurityObject();
        Assert.That(svc.CheckAccess(usr, "XXX", Create).Result, Is.False);
    }

    [Test]
    public void CheckAccess_returns_null_for_default_unset_permissions()
    {
        var svc = new Mock<IUserService>() { CallBase = true }.Object;
        var usr = new LoginCredential();
        Assert.That(svc.CheckAccess(usr, "XXX", Create).Result, Is.Null);        
    }

    [Test]
    public void CheckAccess_returns_true_for_descriptor_granted_permissions()
    {
        var svc = new Mock<IUserService>() { CallBase = true }.Object;
        var usr = GetTestSecurityObject();
        Assert.That(svc.CheckAccess(usr, "TestA", View).Result, Is.True);
    }

    [Test]
    public void CheckAccess_returns_false_for_descriptor_revoked_permissions()
    {
        var svc = new Mock<IUserService>() { CallBase = true }.Object;
        var usr = GetTestSecurityObject();
        Assert.That(svc.CheckAccess(usr, "TestB", Export).Result, Is.False);
    }

    [Test]
    public void CheckAccess_returns_null_for_descriptor_unset_permissions()
    {
        var svc = new Mock<IUserService>() { CallBase = true }.Object;
        var usr = GetTestSecurityObject();
        Assert.That(svc.CheckAccess(usr, "TestA", Lock).Result, Is.Null);
    }

    [Test]
    public void CheckAccess_returns_true_for_group_granted_permissions()
    {
        var svc = new Mock<IUserService>() { CallBase = true }.Object;
        var usr = GetTestSecurityObject();
        Assert.That(svc.CheckAccess(usr, "TestC", Elevate).Result, Is.True);
    }

    [Test]
    public void CheckAccess_returns_false_for_group_revoked_permissions()
    {
        var svc = new Mock<IUserService>() { CallBase = true }.Object;
        var usr = GetTestSecurityObject();
        Assert.That(svc.CheckAccess(usr, "TestD", Lock).Result, Is.False);
    }

    [Test]
    public void CheckAccess_returns_null_for_group_unset_permissions()
    {
        var svc = new Mock<IUserService>() { CallBase = true }.Object;
        var usr = GetTestSecurityObject();
        Assert.That(svc.CheckAccess(usr, "TestD", View).Result, Is.Null);
    }

    [Test]
    public async Task CheckAccess_with_username_resolves_credential_and_checks()
    {
        var cred = GetTestSecurityObject();
        var svcMock = new Mock<IUserService>() { CallBase = true };
        svcMock.Setup(p => p.CheckAccess(cred, "XXX", Read))
            .Returns(new ServiceResult<bool?>(true))
            .Verifiable(Times.Once);
        svcMock.Setup(p => p.GetCredential("test"))
            .ReturnsAsync(cred)
            .Verifiable(Times.Once);
        Assert.That((await svcMock.Object.CheckAccess("test", "XXX", Read)).Result, Is.True);
        svcMock.Verify();
    }
}
