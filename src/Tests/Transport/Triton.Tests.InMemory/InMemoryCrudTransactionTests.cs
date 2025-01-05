#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.Triton.InMemory.Services;
using TheXDS.Triton.Tests.Models;
using TheXDS.Triton.Models.Base;
using Moq;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Services;
namespace TheXDS.Triton.Tests.InMemory;

public class InMemoryCrudTransactionTests
{
    [Test]
    public void Read_reads_entities()
    {
        User user = new() { Id = "abc123" };
        using var transaction = new InMemoryCrudTransaction([user]);
        var result = transaction.Read(typeof(User), "abc123");
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.SameAs(user));
    }

    [Test]
    public async Task Create_creates_new_entities()
    {
        var newUser = new User("CreateTest", "Test user");
        ICollection<Model> store = [];
        using var transaction = new InMemoryCrudTransaction(store);
        Assert.That(transaction.Create(newUser).Success, Is.True);
        Assert.That((await transaction.CommitAsync()).Success, Is.True);
        Assert.That(store.Single(), Is.SameAs(newUser));
    }

    [Test]
    public async Task Create_fails_on_existing_entity()
    {
        var existingUser = new User("abc123", "Existing user");
        var newUser = new User("abc123", "New user");
        ICollection<Model> store = [existingUser];
        using var transaction = new InMemoryCrudTransaction(store);
        var result = transaction.Create(newUser);
        Assert.That(result.Success, Is.False);
        Assert.That(result.Reason, Is.EqualTo(FailureReason.EntityDuplication));
        Assert.That((await transaction.CommitAsync()).Success, Is.True);
        Assert.That(store.Single(), Is.SameAs(existingUser));
    }

    [Test]
    public async Task CreateOrUpdate_creates_new_entities()
    {
        var newUser = new User("CreateTest", "Test user");
        ICollection<Model> store = [];
        using var transaction = new InMemoryCrudTransaction(store);
        Assert.That(transaction.CreateOrUpdate(newUser).Success, Is.True);
        Assert.That((await transaction.CommitAsync()).Success, Is.True);
        Assert.That(store.Single(), Is.SameAs(newUser));
    }

    [Test]
    public async Task CreateOrUpdate_updates_entities()
    {
        var existingUser = new User("abc123", "Existing user");
        var newUserData = new User("abc123", "Modified user");
        ICollection<Model> store = [existingUser];
        using var transaction = new InMemoryCrudTransaction(store);
        var result = transaction.CreateOrUpdate(newUserData);
        Assert.That(result.Success, Is.True);
        Assert.That((await transaction.CommitAsync()).Success, Is.True);
        Assert.That(((User)store.Single()).PublicName, Is.EqualTo("Modified user"));
    }

    [Test]
    public async Task Update_updates_entities()
    {
        var existingUser = new User("abc123", "Existing user");
        var newUserData = new User("abc123", "Modified user");
        ICollection<Model> store = [existingUser];
        using var transaction = new InMemoryCrudTransaction(store);
        var result = transaction.Update(newUserData);
        Assert.That(result.Success, Is.True);
        Assert.That((await transaction.CommitAsync()).Success, Is.True);
        Assert.That(((User)store.Single()).PublicName, Is.EqualTo("Modified user"));
    }

    [Test]
    public async Task Update_fails_on_not_found()
    {
        var newUserData = new User("abc123", "Modified user");
        ICollection<Model> store = [];
        using var transaction = new InMemoryCrudTransaction(store);
        var result = transaction.Update(newUserData);
        Assert.That(result.Success, Is.False);
        Assert.That(result.Reason, Is.EqualTo(FailureReason.NotFound));
        Assert.That((await transaction.CommitAsync()).Success, Is.True);
        Assert.That(store, Is.Empty);
    }

    [Test]
    public async Task Delete_with_entity_fails_on_not_found()
    {
        var newUserData = new User("abc123", "Modified user");
        ICollection<Model> store = [];
        using var transaction = new InMemoryCrudTransaction(store);
        var result = transaction.Delete(newUserData);
        Assert.That(result.Success, Is.False);
        Assert.That(result.Reason, Is.EqualTo(FailureReason.NotFound));
        Assert.That((await transaction.CommitAsync()).Success, Is.True);
        Assert.That(store, Is.Empty);
    }

    [Test]
    public async Task Delete_with_string_key_fails_on_not_found()
    {
        ICollection<Model> store = [];
        using var transaction = new InMemoryCrudTransaction(store);
        var result = transaction.Delete<User>("abc123");
        Assert.That(result.Success, Is.False);
        Assert.That(result.Reason, Is.EqualTo(FailureReason.NotFound));
        Assert.That((await transaction.CommitAsync()).Success, Is.True);
        Assert.That(store, Is.Empty);
    }

    [Test]
    public async Task Delete_with_TKey_fails_on_not_found()
    {
        ICollection<Model> store = [];
        using var transaction = new InMemoryCrudTransaction(store);
        var result = transaction.Delete<User, string>("abc123");
        Assert.That(result.Success, Is.False);
        Assert.That(result.Reason, Is.EqualTo(FailureReason.NotFound));
        Assert.That((await transaction.CommitAsync()).Success, Is.True);
        Assert.That(store, Is.Empty);
    }

    [Test]
    public async Task Delete_with_TKey_deletes_entity()
    {
        ICollection<Model> store = [new User("abc123", "abc 123")];
        using var transaction = new InMemoryCrudTransaction(store);
        var result = transaction.Delete<User, string>("abc123");
        Assert.That(result.Success, Is.True);
        Assert.That((await transaction.CommitAsync()).Success, Is.True);
        Assert.That(store, Is.Empty);
    }

    [Test]
    public async Task Delete_with_string_key_deletes_entity()
    {
        ICollection<Model> store = [new User("abc123", "abc 123")];
        using var transaction = new InMemoryCrudTransaction(store);
        var result = transaction.Delete<User>("abc123");
        Assert.That(result.Success, Is.True);
        Assert.That((await transaction.CommitAsync()).Success, Is.True);
        Assert.That(store, Is.Empty);
    }
}