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
            Assert.That(CrudAction.Create, Is.EqualTo(Action));
            Assert.That(u, Is.SameAs(Entities!.ToArray()[0]));
        }
        Assert.That(CrudAction.Commit, Is.EqualTo(Action));
        Assert.That(Entities, Is.Null);
    }
}