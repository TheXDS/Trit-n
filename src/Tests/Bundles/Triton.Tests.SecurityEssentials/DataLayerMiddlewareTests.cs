#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Component;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Models;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.SecurityEssentials;

public class DataLayerMiddlewareTests
{
    private class TestSecurityActorProvider : ISecurityActorProvider
    {
        public SecurityObject? SecurityObject { get; set; }
        public SecurityObject? GetActor()
        {
            return SecurityObject;
        }
    }

    [Test]
    public void GetModelContextString_generic_Test()
    {
        var a = typeof(CrudAction).FullName;
        var b = CrudAction.Create.ToString();
        var c = typeof(LoginCredential).CSharpName();

        Assert.AreEqual($"{a}.{b};{c}", DataLayerSecurityMiddleware.GetModelContextString<LoginCredential>(CrudAction.Create));
    }

    [Test]
    public void GetModelContextString_Test()
    {
        var a = typeof(CrudAction).FullName;
        var b = CrudAction.Create.ToString();
        var c = typeof(LoginCredential).CSharpName();

        Assert.AreEqual($"{a}.{b};{c}", DataLayerSecurityMiddleware.GetModelContextString(CrudAction.Create, typeof(LoginCredential)));
    }

    [Test]
    public async Task Middleware_Test()
    {
        static async Task<LoginCredential> GetCredential(string credential, IUserService userService)
        {
            var cred = (await userService.GetCredential(credential)).ReturnValue;
            Assert.IsNotNull(cred);
            return cred!;
        }

        TestSecurityActorProvider prov = new();
        TestUserService svc = new();
        ITransactionMiddleware middleware = new DataLayerSecurityMiddleware(prov, svc);
        var root = await GetCredential("root", svc);
        var disabled = await GetCredential("disabled", svc);

        prov.SecurityObject = null;
        Assert.AreEqual(FailureReason.Tamper, middleware.PrologAction(CrudAction.Create, new LoginCredential())!.Reason);
        Assert.AreEqual(FailureReason.Tamper, middleware.PrologAction(CrudAction.Create, new LoginCredential())!.Reason);

        prov.SecurityObject =  root;
        Assert.IsNull(middleware.PrologAction(CrudAction.Commit, null));
        Assert.IsNull(middleware.PrologAction(CrudAction.Create, new LoginCredential()));

        prov.SecurityObject = disabled;
        Assert.IsNull(middleware.PrologAction(CrudAction.Commit, null));
        Assert.AreEqual(FailureReason.Forbidden, middleware.PrologAction(CrudAction.Create, new LoginCredential())!.Reason);
    }
}
