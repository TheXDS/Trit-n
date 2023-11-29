#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;
using TheXDS.Triton.Tests.Models;

namespace TheXDS.Triton.Tests.Services.Services;

public class ICrudReadTransactionTests : TransactionTestBase
{
    [Test]
    public void Read_with_type_and_key_as_object()
    {
        using var t = GetTransaction();
        Model? u = t.Read(typeof(User), "user1").Result;
        Assert.That(u, Is.Not.Null);
        Assert.That(u, Is.InstanceOf<User>());
        Assert.That(u!.IdAsString, Is.EqualTo("user1"));
    }

    [Test]
    public void Read_contract_test()
    {
        using var t = GetTransaction();
        Assert.That(() => _ = t.Read(typeof(User), null!), Throws.ArgumentNullException);
        Assert.That(() => _ = t.Read(null!, "user1"), Throws.ArgumentNullException);
    }

    [Test]
    public async Task ReadAsync_with_type_and_key_as_object()
    {
        using var t = GetTransaction();
        Model u = (await t.ReadAsync(typeof(User), "user1")).Result!;
        Assert.That(u, Is.Not.Null);
        Assert.That(u, Is.InstanceOf<User>());
        Assert.That(u.IdAsString, Is.EqualTo("user1"));
    }
    [Test]
    public void All_with_type_returns_data()
    {
        using var t = GetTransaction();
        var q = t.All(typeof(User)).ToArray();
        Assert.That(q, Is.Not.Null);
        Assert.That(q, Is.Not.Empty);
    }

    [TestCase(FailureReason.Unknown)]
    [TestCase(FailureReason.Tamper)]
    [TestCase(FailureReason.Forbidden)]
    [TestCase(FailureReason.ServiceFailure)]
    [TestCase(FailureReason.NetworkFailure)]
    [TestCase(FailureReason.DbFailure)]
    [TestCase(FailureReason.ValidationError)]
    [TestCase(FailureReason.ConcurrencyFailure)]
    [TestCase(FailureReason.NotFound)]
    [TestCase(FailureReason.EntityDuplication)]
    [TestCase(FailureReason.BadQuery)]
    [TestCase(FailureReason.QueryOverLimit)]
    public void All_with_type_returns_failureReason_on_failure(FailureReason reason)
    {
        using var t = GetTransaction(reason);
        var q = t.All(typeof(User));
        Assert.That(q.Success, Is.False);
        Assert.That(q.Reason, Is.EqualTo(reason));
    }

    [Test]
    public async Task SearchAsync_with_predicate()
    {
        using var t = GetTransaction();
        var q = await t.SearchAsync<User>(u => u.Id == "user1");
        Assert.That(q, Is.Not.Null);
        Assert.That(q.Success, Is.True);
        Assert.That(q.Result, Is.Not.Null);
        Assert.That(q.Result, Is.Not.Empty);
        Assert.That(q.Result![0].IdAsString, Is.EqualTo("user1"));
    }
}
