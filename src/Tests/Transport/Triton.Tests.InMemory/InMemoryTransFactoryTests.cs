#pragma warning disable CS1591

using NUnit.Framework;
using System.Transactions;
using TheXDS.Triton.InMemory.Services;
using TheXDS.Triton.Services;
using TheXDS.Triton.Tests.Models;

namespace TheXDS.Triton.Tests.InMemory;

public class InMemoryTransFactoryTests : TransactionMiddlewareExecutionTests<InMemoryTransFactory>
{
    [Test]
    public void Wipe_clears_store()
    {
        TransactionConfiguration transactionConfig = new();
        InMemoryTransFactory transFactory = new();
        TritonService service = new(transactionConfig, transFactory);
        using (var t = service.GetWriteTransaction())
        {
            t.Create(new User() { Id = "XYZ789" });
            t.Commit();
        }
        using (var t = service.GetReadTransaction())
        {
            Assert.That(t.Read<User, string>("XYZ789").Result, Is.Not.Null);
        }
        InMemoryTransFactory.Wipe();
        using (var t = service.GetReadTransaction())
        {
            Assert.That(t.Read<User, string>("XYZ789").Result, Is.Null);
        }
    }
}
