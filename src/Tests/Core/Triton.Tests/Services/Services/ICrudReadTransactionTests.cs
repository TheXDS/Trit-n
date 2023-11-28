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
        Model? u = t.Read(typeof(User), "user1").ReturnValue;
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
        Model u = (await t.ReadAsync(typeof(User), "user1")).ReturnValue!;
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
        Assert.That(q.ReturnValue, Is.Not.Null);
        Assert.That(q.ReturnValue, Is.Not.Empty);
        Assert.That(q.ReturnValue![0].IdAsString, Is.EqualTo("user1"));
    }
}

public class ICrudWriteTransactionTests : TransactionTestBase
{
    [Test]
    public void CreateOrUpdate_TModel_creates_new_entities()
    {
        var id = Guid.NewGuid().ToString();
        using (var t = GetTransaction())
        {
            var u = new User() { Id = id, PublicName = "Name" };
            Assert.That(t.CreateOrUpdate(u).Success, Is.True);
            Assert.That(t.Commit().Success, Is.True);
        }
        using (var t = GetTransaction())
        {
            var r = t.Read<User>(id);
            Assert.That(r.Success, Is.True);
            Assert.That(r.ReturnValue, Is.Not.Null);
            Assert.That(r.ReturnValue!.IdAsString, Is.EqualTo(id));
            Assert.That(r.ReturnValue.PublicName, Is.EqualTo("Name"));
        }
    }
    [Test]
    public void CreateOrUpdate_creates_new_entities()
    {
        var id = Guid.NewGuid().ToString();
        using (var t = GetTransaction())
        {
            Model u = new User() { Id = id, PublicName = "Name" };
            Assert.That(t.CreateOrUpdate(u).Success, Is.True);
            Assert.That(t.Commit().Success, Is.True);
        }
        using (var t = GetTransaction())
        {
            var r = t.Read<User>(id);
            Assert.That(r.Success, Is.True);
            Assert.That(r.ReturnValue, Is.Not.Null);
            Assert.That(r.ReturnValue!.IdAsString, Is.EqualTo(id));
            Assert.That(r.ReturnValue.PublicName, Is.EqualTo("Name"));
        }
    }

    [Test]
    public void CreateOrUpdate_TModel_updates_old_entities()
    {
        var id = Guid.NewGuid().ToString();
        using (var t = GetTransaction())
        {
            t.Create(new User() { Id = id, PublicName = "oldName" });
            t.Commit();
        }
        using (var t = GetTransaction())
        {
            var u = new User() { Id = id, PublicName = "newName" };
            Assert.That(t.CreateOrUpdate(u).Success, Is.True);
            Assert.That(t.Commit().Success, Is.True);
        }
        using (var t = GetTransaction())
        {
            var r = t.Read<User>(id);
            Assert.That(r.Success, Is.True);
            Assert.That(r.ReturnValue, Is.Not.Null);
            Assert.That(r.ReturnValue!.IdAsString, Is.EqualTo(id));
            Assert.That(r.ReturnValue.PublicName, Is.EqualTo("newName"));
        }
    }

    [Test]
    public void CreateOrUpdate_updates_old_entities()
    {
        var id = Guid.NewGuid().ToString();
        using (var t = GetTransaction())
        {
            t.Create(new User() { Id = id, PublicName = "oldName" });
            t.Commit();
        }
        using (var t = GetTransaction())
        {
            Model u = new User() { Id = id, PublicName = "newName" };
            Assert.That(t.CreateOrUpdate(u).Success, Is.True);
            Assert.That(t.Commit().Success, Is.True);
        }
        using (var t = GetTransaction())
        {
            var r = t.Read<User>(id);
            Assert.That(r.Success, Is.True);
            Assert.That(r.ReturnValue, Is.Not.Null);
            Assert.That(r.ReturnValue!.IdAsString, Is.EqualTo(id));
            Assert.That(r.ReturnValue.PublicName, Is.EqualTo("newName"));
        }
    }

    [Test]
    public void Update_updates_old_entities()
    {
        var id = Guid.NewGuid().ToString();
        using (var t = GetTransaction())
        {
            t.Create(new User() { Id = id, PublicName = "oldName" });
            t.Commit();
        }
        using (var t = GetTransaction())
        {
            Model u = new User() { Id = id, PublicName = "newName" };
            Assert.That(t.Update(u).Success, Is.True);
            Assert.That(t.Commit().Success, Is.True);
        }
        using (var t = GetTransaction())
        {
            var r = t.Read<User>(id);
            Assert.That(r.Success, Is.True);
            Assert.That(r.ReturnValue, Is.Not.Null);
            Assert.That(r.ReturnValue!.IdAsString, Is.EqualTo(id));
            Assert.That(r.ReturnValue.PublicName, Is.EqualTo("newName"));
        }
    }

    [Test]
    public void Update_TModel_updates_old_entities()
    {
        var id = Guid.NewGuid().ToString();
        using (var t = GetTransaction())
        {
            t.Create(new User() { Id = id, PublicName = "oldName" });
            t.Commit();
        }
        using (var t = GetTransaction())
        {
            var u = new User() { Id = id, PublicName = "newName" };
            Assert.That(t.Update(u).Success, Is.True);
            Assert.That(t.Commit().Success, Is.True);
        }
        using (var t = GetTransaction())
        {
            var r = t.Read<User>(id);
            Assert.That(r.Success, Is.True);
            Assert.That(r.ReturnValue, Is.Not.Null);
            Assert.That(r.ReturnValue!.IdAsString, Is.EqualTo(id));
            Assert.That(r.ReturnValue.PublicName, Is.EqualTo("newName"));
        }
    }

    [Test]
    public void Delete_TModel_with_entity_deletes_entities()
    {
        var id = Guid.NewGuid().ToString();
        using (var t = GetTransaction())
        {
            t.Create(new User() { Id = id, PublicName = "Name" });
            t.Commit();
        }
        using (var t = GetTransaction())
        {
            User u = t.Read<User>(id).ReturnValue!;
            Assert.That(t.Delete(u).Success, Is.True);
            Assert.That(t.Commit().Success, Is.True);
        }
        using (var t = GetTransaction())
        {
            var r = t.Read<User>(id);
            Assert.That(r.Success, Is.False);
            Assert.That(r.Reason, Is.EqualTo(FailureReason.NotFound));
        }
    }

    [Test]
    public void Delete_with_entity_deletes_entities()
    {
        var id = Guid.NewGuid().ToString();
        using (var t = GetTransaction())
        {
            t.Create(new User() { Id = id, PublicName = "Name" });
            t.Commit();
        }
        using (var t = GetTransaction())
        {
            Model u = t.Read<User>(id).ReturnValue!;
            Assert.That(t.Delete(u).Success, Is.True);
            Assert.That(t.Commit().Success, Is.True);
        }
        using (var t = GetTransaction())
        {
            var r = t.Read<User>(id);
            Assert.That(r.Success, Is.False);
            Assert.That(r.Reason, Is.EqualTo(FailureReason.NotFound));
        }
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
    public void Delete_failing_returns_failure_result(FailureReason reason)
    {
        var id = Guid.NewGuid().ToString();
        using var t = GetTransaction(reason);
        Model u = new User(id, id);
        var r = t.Delete(u);
        Assert.That(r.Success, Is.False);
        Assert.That(r.Reason, Is.EqualTo(reason));
    }
}