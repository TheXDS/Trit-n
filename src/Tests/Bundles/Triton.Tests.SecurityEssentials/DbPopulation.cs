#pragma warning disable CS1591

using NUnit.Framework;
using System.Linq;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.InMemory.Services;
using TheXDS.Triton.Models;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests
{
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
                var lc = new LoginCredential()
                {
                    Granted = PermissionFlags.View,
                    Revoked = PermissionFlags.Read,
                    Id = System.Guid.NewGuid(),
                    Enabled = true,
                    Username = "test",
                    PasswordHash = ((IUserService)svc).HashPassword("test".ToSecureString()),  
                };
                var g = new UserGroup()
                {
                    Granted = PermissionFlags.Create,
                    Revoked = PermissionFlags.Update,
                    DisplayName = "test group",
                    Id = System.Guid.NewGuid(),
                };
                t.Create(new Session()
                {
                    Credential = lc,
                    Id = System.Guid.NewGuid(),
                    Timestamp = new System.DateTime(2022, 1, 1),
                    TtlHours = int.MaxValue,
                    Token = "abcd1234"
                }.PushInto(lc.Sessions));
                t.Create(new UserGroupMembership() {
                    User = lc,
                    Group = g,
                    Id = System.Guid.NewGuid(),
                }.PushInto(lc.Membership).PushInto(g.Membership));
                t.CreateMany(
                    new SecurityDescriptor()
                    {
                        Id = System.Guid.NewGuid(),
                        ContextId = "testViewContext",
                        Granted = PermissionFlags.Delete,
                        Revoked = PermissionFlags.Export
                    }.PushInto(lc.Descriptors)                    
                    );
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
}