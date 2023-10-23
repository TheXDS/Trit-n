#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.Triton.InMemory.Services;
using TheXDS.Triton.Services;
using TheXDS.Triton.Tests.Models;
using TheXDS.Triton.Models.Base;
namespace TheXDS.Triton.Tests.InMemory;

public class InMemoryCrudTransactionTests
{
    [Test]
    public async Task Create_creates_new_entities()
    {
        var c = new List<Model>();
        using var t = new InMemoryCrudTransaction(new TransactionConfiguration(), c);
        var u = new User("CreateTest", "Test user");
        Assert.That(t.Create(u).Success, Is.True);
        Assert.That((await t.CommitAsync()).Success, Is.True);
        var readResult = await t.ReadAsync<User, string>("CreateTest");
        Assert.That(readResult.Success, Is.True);
        Assert.That(readResult.ReturnValue, Is.InstanceOf<User>());
    }

    [Test]
    public async Task CreateOrUpdate_creates_new_entities()
    {
        var c = new List<Model>();
        using var t = new InMemoryCrudTransaction(new TransactionConfiguration(), c);
        var u = new User("CreateOrUpdateNewTest", "Test user");
        Assert.That(t.CreateOrUpdate(u).Success, Is.True);
        Assert.That((await t.CommitAsync()).Success, Is.True);
        var readResult = await t.ReadAsync<User, string>("CreateOrUpdateNewTest");
        Assert.That(readResult.Success, Is.True);
        Assert.That(readResult.ReturnValue, Is.InstanceOf<User>());
    }

    [Test]
    public async Task CreateOrUpdate_updates_entities()
    {
        var c = new List<Model>();
        using var t = new InMemoryCrudTransaction(new TransactionConfiguration(), c);
        var u = new User("CreateOrUpdateExistingTest", "AAA");
        Assert.That(t.Create(u).Success, Is.True);
        Assert.That((await t.CommitAsync()).Success, Is.True);

        var updatedU = new User("CreateOrUpdateExistingTest", "BBB");
        Assert.That(t.CreateOrUpdate(updatedU).Success, Is.True);
        Assert.That((await t.CommitAsync()).Success, Is.True);
        var readResult = await t.ReadAsync<User, string>("CreateOrUpdateExistingTest");
        Assert.That(readResult.Success, Is.True);
        Assert.That(readResult.ReturnValue, Is.InstanceOf<User>());
        Assert.That(readResult.ReturnValue!.PublicName, Is.EqualTo("BBB"));
    }

    [Test]
    public async Task Update_updates_entities()
    {
        var c = new List<Model>();
        using var t = new InMemoryCrudTransaction(new TransactionConfiguration(), c);
        var u = new User("UpdateExistingTest", "AAA");
        Assert.That(t.Create(u).Success, Is.True);
        Assert.That((await t.CommitAsync()).Success, Is.True);

        var updatedU = new User("UpdateExistingTest", "BBB");
        Assert.That(t.Update(updatedU).Success, Is.True);
        Assert.That((await t.CommitAsync()).Success, Is.True);
        var readResult = await t.ReadAsync<User, string>("UpdateExistingTest");
        Assert.That(readResult.Success, Is.True);
        Assert.That(readResult.ReturnValue, Is.InstanceOf<User>());
        Assert.That(readResult.ReturnValue!.PublicName, Is.EqualTo("BBB"));
    }
}