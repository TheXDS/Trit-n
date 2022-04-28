#pragma warning disable CS1591

using NUnit.Framework;
using System;
using System.Threading.Tasks;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Models;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.SecurityEssentials
{
    public class UserServiceTests
    {
        private readonly IUserService _srv = new TestUserService();

        [Test]
        public async Task Authenticate_valid_user_Test()
        {
            var t = DateTime.UtcNow;
            var epsilon = TimeSpan.FromSeconds(30);
            var r = await _srv.Authenticate("test", "test".ToSecureString());
            Assert.IsTrue(r.Success);
            Assert.IsInstanceOf<Session>(r.ReturnValue);
            Assert.AreEqual("test", r.ReturnValue!.Credential.Username);
            Assert.IsTrue(r.ReturnValue!.Timestamp.IsBetween(t - epsilon, t + epsilon));
        }

        [Test]
        public async Task Authenticate_creates_session_in_db_Test()
        {
            var r = await _srv.Authenticate("test", "test".ToSecureString());
            using var t = _srv.GetReadTransaction();
            Assert.NotNull(await t.ReadAsync<Session, Guid>(r.ReturnValue!.Id));
        }

        [Test]
        public async Task Authenticate_with_invalid_user_Test()
        {
            var r = await _srv.Authenticate("nonExistentUser", "test".ToSecureString());
            Assert.IsFalse(r.Success);
            Assert.IsNull(r.ReturnValue);
            Assert.AreEqual(FailureReason.Forbidden, r.Reason);
        }

        [Test]
        public async Task Authenticate_with_invalid_password_Test()
        {
            var r = await _srv.Authenticate("test", "wrong".ToSecureString());
            Assert.IsFalse(r.Success);
            Assert.IsNull(r.ReturnValue);
            Assert.AreEqual(FailureReason.Forbidden, r.Reason);
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
            Assert.AreEqual(result, (await _srv.CheckAccess("test", context, flags)).ReturnValue);
        }

        [Test]
        public async Task CheckAccess_returns_null_on_svc_error_Test()
        {
            var svc = new TestUserService();
            ((InMemory.Services.TestTransFactory)svc.Factory).InjectFailure = true;
            var r = await ((IUserService)svc).CheckAccess("test", "testViewContext", PermissionFlags.View);
            Assert.IsFalse(r.Success);
            Assert.IsNull(r.ReturnValue);
        }

        [Test]
        public async Task Authenticate_returns_null_on_svc_error_Test()
        {
            var svc = new TestUserService();
            ((InMemory.Services.TestTransFactory)svc.Factory).InjectFailure = true;
            var r = await ((IUserService)svc).Authenticate("test", "test".ToSecureString());
            Assert.IsFalse(r.Success);
            Assert.IsNull(r.ReturnValue);
            Assert.AreEqual(FailureReason.ServiceFailure, r.Reason);
        }
    }
}