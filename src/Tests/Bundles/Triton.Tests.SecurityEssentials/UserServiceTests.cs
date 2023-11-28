#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Models;
using TheXDS.Triton.Services;
using TheXDS.Triton.Tests.Services;

namespace TheXDS.Triton.Tests.SecurityEssentials;

public class UserServiceTests
{
    private readonly IUserService _svc = new TestUserService();
    private readonly IUserService _badSvc = CreateBadService();

    private static IUserService CreateBadService()
    {
        var svc = new TestUserService();
        ((TestTransFactory)svc.Factory).InjectFailure = true;
        return svc;
    }

    [Test]
    public async Task Authenticate_valid_user_Test()
    {
        var t = DateTime.UtcNow;
        var epsilon = TimeSpan.FromSeconds(30);
        var r = await _svc.Authenticate("test", "test".ToSecureString());
        Assert.That(r.Success, Is.True);
        Assert.That(r.ReturnValue, Is.AssignableFrom<Session>());
        Assert.That(r.ReturnValue!.Credential.Username, Is.EqualTo("test"));
        Assert.That(r.ReturnValue!.Timestamp.IsBetween(t - epsilon, t + epsilon), Is.True);
    }
    
    [Test]
    public async Task Authenticate_creates_session_in_db_Test()
    {
        var r = await _svc.Authenticate("test", "test".ToSecureString());
        await using var t = _svc.GetReadTransaction();
        Assert.That(await t.ReadAsync<Session, Guid>(r.ReturnValue!.Id), Is.Not.Null);
    }

    [Test]
    public async Task Authenticate_with_invalid_user_Test()
    {
        var r = await _svc.Authenticate("nonExistentUser", "test".ToSecureString());
        Assert.That(r.Success, Is.False);
        Assert.That(r.ReturnValue, Is.Null);
        Assert.That(r.Reason, Is.EqualTo(FailureReason.Forbidden));
    }
        
    [Test]
    public async Task Authenticate_with_disabled_user_Test()
    {
        var r = await _svc.Authenticate("disabled", "test".ToSecureString());
        Assert.That(r.Success, Is.False);
        Assert.That(r.ReturnValue, Is.Null);
        Assert.That(r.Reason, Is.EqualTo(FailureReason.Forbidden));
    }

    [Test]
    public async Task Authenticate_with_invalid_password_Test()
    {
        var r = await _svc.Authenticate("test", "wrong".ToSecureString());
        Assert.That(r.Success, Is.False);
        Assert.That(r.ReturnValue, Is.Null);
        Assert.That(r.Reason, Is.EqualTo(FailureReason.Forbidden));
    }

    [TestCase("testViewContext", PermissionFlags.Delete, true)]
    [TestCase("nonRegisteredContext", PermissionFlags.View, true)]
    [TestCase("nonRegisteredContext", PermissionFlags.Create, true)]
    [TestCase("testViewContext", PermissionFlags.Export, false)]
    [TestCase("nonRegisteredContext", PermissionFlags.Read, false)]
    [TestCase("nonRegisteredContext", PermissionFlags.Update, false)]
    [TestCase("testViewContext", PermissionFlags.Lock, null)]
    [TestCase("nonRegisteredContext", PermissionFlags.Elevate, null)]
    public async Task CheckAccess_permissions_Test(string context, PermissionFlags flags, bool? result)
    {
        Assert.That((await _svc.CheckAccess("test", context, flags)).ReturnValue, Is.EqualTo(result));
    }

    [Test]
    public async Task CheckAccess_returns_null_on_svc_error_Test()
    {
        var r = await _badSvc.CheckAccess("test", "testViewContext", PermissionFlags.View);
        Assert.That(r.Success, Is.False);
        Assert.That(r.ReturnValue, Is.Null);
    }

    [Test]
    public async Task Authenticate_returns_null_on_svc_error_Test()
    {
        var r = await _badSvc.Authenticate("test", "test".ToSecureString());
        Assert.That(r.Success, Is.False);
        Assert.That(r.ReturnValue, Is.Null);
        Assert.That(r.Reason, Is.EqualTo(FailureReason.ServiceFailure));
    }

    [Test]
    public async Task AddNewLoginCredential_Test()
    {
        Assert.That((await _svc.AddNewLoginCredential("newUser", "newPassword".ToSecureString())).Success, Is.True);
        var u = await _svc.GetCredential("newUser");
        Assert.That(u.Success, Is.True);
        Assert.That(u.ReturnValue, Is.Not.Null);
        Assert.That(u.ReturnValue!.PasswordHash.Any(), Is.True);
        Assert.That((await _svc.VerifyPassword("newUser", "newPassword".ToSecureString())).ReturnValue?.Valid, Is.True);
    }

    [Test]
    public async Task AddNewLoginCredential_on_Service_error_Test()
    {
        var r = await _badSvc.AddNewLoginCredential("newUser", "newPassword".ToSecureString());
        Assert.That(r.Success, Is.False);
        Assert.That(r.Reason, Is.EqualTo(FailureReason.ServiceFailure));
    }

    [Test]
    public async Task AddNewLoginCredential_on_dup_entity_Test()
    {
        var r = await _svc.AddNewLoginCredential("test", "newPassword".ToSecureString());
        Assert.That(r.Success, Is.False);
        Assert.That(r.Reason, Is.EqualTo(FailureReason.EntityDuplication));
    }
}