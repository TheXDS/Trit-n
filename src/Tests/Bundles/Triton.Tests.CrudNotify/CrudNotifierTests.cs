#pragma warning disable CS1591

using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;
using TheXDS.Triton.Tests.Models;
using TheXDS.Triton.Tests.Services;
using NUnit.Framework;
using TheXDS.Triton.CrudNotify;

namespace TheXDS.Triton.Tests.CrudNotify;

public class CrudNotifierTests
{
    private static CrudAction Action { get; set; }
    
    private static IEnumerable<Model>? Entities { get; set; }
    
    private class TestNotifier : ICrudNotifier
    {
        public ServiceResult NotifyPeers(CrudAction action, IEnumerable<Model>? entities)
        {
            Action = action;
            Entities = entities;
            return ServiceResult.Ok;
        }
    }
    
    [Test]
    public async Task Crud_transaction_triggers_notifications_Test()
    {
        TritonService srv = new(new TestTransFactory());
        srv.Configuration.AddNotifyService<TestNotifier>();
        await using (var t = srv.GetTransaction())
        {
            User u = new("cntest", "CrudNotify user");
            t.Create(u);
            Assert.AreEqual(CrudAction.Create, Action);
            Assert.AreSame(u, Entities!.ToArray()[0]);
        }
        Assert.AreEqual(CrudAction.Commit, Action);
        Assert.IsNull(Entities);
    }
}