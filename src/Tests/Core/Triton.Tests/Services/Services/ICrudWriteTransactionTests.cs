#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;
using TheXDS.Triton.Tests.Models;

namespace TheXDS.Triton.Tests.Services.Services;

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