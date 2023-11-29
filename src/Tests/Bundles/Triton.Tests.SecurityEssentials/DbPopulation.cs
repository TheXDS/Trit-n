#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.MCART.Security;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Models;
using TheXDS.Triton.Services;
using TheXDS.Triton.Tests.Services;

namespace TheXDS.Triton.Tests.SecurityEssentials;

[SetUpFixture]
public class DbPopulation
{
    [OneTimeSetUp]
    public void PopulateDb()
    {
        var svc = new TestUserService();
        using var t = svc.GetTransaction();
        if (!t.All<LoginCredential>().Any())
        {
            LoginCredential lc;
            var creds = new LoginCredential[] {
                new()
                {
                    Granted = PermissionFlags.All,
                    Revoked = PermissionFlags.None,
                    Id = Guid.NewGuid(),
                    Enabled = true,
                    Username = "root",
                    PasswordHash = ((IUserService)svc).HashPassword("root".ToSecureString()),
                },
                new()
                {
                    Granted = PermissionFlags.None,
                    Revoked = PermissionFlags.All,
                    Id = Guid.NewGuid(),
                    Enabled = false,
                    Username = "disabled",
                    PasswordHash = ((IUserService)svc).HashPassword("test".ToSecureString()),
                },
                new()
                {
                    Granted = PermissionFlags.Elevate,
                    Revoked = PermissionFlags.All,
                    Id = Guid.NewGuid(),
                    Enabled = false,
                    Username = "elevatable",
                    PasswordHash = ((IUserService)svc).HashPassword("test".ToSecureString()),
                },
                lc = new LoginCredential()
                {
                    Granted = PermissionFlags.View,
                    Revoked = PermissionFlags.Read,
                    Id = Guid.NewGuid(),
                    Enabled = true,
                    Username = "test",
                    PasswordHash = ((IUserService)svc).HashPassword("test".ToSecureString()),
                }
            };

            var g = new UserGroup()
            {
                Granted = PermissionFlags.Create,
                Revoked = PermissionFlags.Update,
                DisplayName = "test group",
                Id = Guid.NewGuid(),
            };
            t.Create(new Session()
            {
                Credential = lc,
                Id = Guid.NewGuid(),
                Timestamp = new DateTime(2022, 1, 1),
                TtlSeconds = int.MaxValue,
                Token = "abcd1234"
            }.PushInto(lc.Sessions));
            t.Create(new UserGroupMembership()
            {
                SecurityObject = lc,
                Group = g,
                Id = Guid.NewGuid(),
            }.PushInto(lc.Membership).PushInto(g.Membership));
            t.Create(
                new SecurityDescriptor()
                {
                    Id = Guid.NewGuid(),
                    ContextId = "testViewContext",
                    Granted = PermissionFlags.Delete,
                    Revoked = PermissionFlags.Export
                }.PushInto(lc.Descriptors)
            );
            t.Create(creds);
            t.Create(lc);
            t.Create(g);
        }
    }

    [OneTimeTearDown]
    public void Teardown()
    {
        TestCrudTransaction.Wipe();
    }
}