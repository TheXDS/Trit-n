#pragma warning disable 1591

using NUnit.Framework;
using TheXDS.Triton.Services;
using TheXDS.Triton.Tests.EFCore.Models;
using TheXDS.Triton.Tests.Models;

namespace TheXDS.Triton.Tests.EFCore.Services;

public class CrudTransactionTests
{
    private static CrudTransaction<BlogContext> GetTestTransaction() => new(new TransactionConfiguration());

    [Test]
    public void CrudTransaction_class_contains_Context_property()
    {
        var t = GetTestTransaction();

        Assert.IsNotNull(t.Context);
        Assert.IsInstanceOf<BlogContext>(t.Context);
    }

    [Test]
    public async Task CrudTransaction_async_ops_commit_automatically_test()
    {
        await using (var t = GetTestTransaction())
        {
            var r = t.Create(new User("user0123", "User 0-1-2-3"));
            Assert.IsTrue(r.Success);
        }

        await using (var t = GetTestTransaction())
        {
            var r = t.Read<User, string>("user0123", out var u);
            Assert.IsTrue(r.Success);
            Assert.IsNotNull(u);
        }
    }

    [Test]
    public void CrudTransaction_can_read_data_test_1()
    {
        using var t = GetTestTransaction();
        var r = t.Read<User, string>("user1");
        Assert.IsTrue(r.Success);
        Assert.IsNotNull(r.ReturnValue);
    }

    [Test]
    public async Task CrudTransaction_can_read_data_async_test_1()
    {
        await using var t = GetTestTransaction();
        var r = t.Read<User, string>("user1");
        Assert.IsTrue(r.Success);
        Assert.IsNotNull(r.ReturnValue);
    }

    [Test]
    public void CrudTransaction_can_read_data_test_2()
    {
        using var t = GetTestTransaction();
        var r = t.Read<User, string>("user1", out var u);
        Assert.IsTrue(r.Success);
        Assert.IsNotNull(u);
    }

    [Test]
    public async Task CrudTransaction_can_read_data_async_test_2()
    {
        await using var t = GetTestTransaction();
        var r = t.Read<User, string>("user1", out var u);
        Assert.IsTrue(r.Success);
        Assert.IsNotNull(u);
    }

    [Test]
    public void CrudTransaction_write_new_data_and_verify_test()
    {
        using (var t = GetTestTransaction())
        {
            var r = t.Create(new User("user123", "User 1-2-3"));
            Assert.IsTrue(r.Success);
        }

        using (var t = GetTestTransaction())
        {
            var r = t.Read<User, string>("user123", out var u);
            Assert.IsTrue(r.Success);
            Assert.IsNotNull(u);
        }
    }

    [Test]
    public async Task CrudTransaction_write_new_data_and_verify_async_test()
    {
        await using (var t = GetTestTransaction())
        {
            var r = t.Create(new User("user123", "User 1-2-3"));
            Assert.IsTrue(r.Success);
        }

        await using (var t = GetTestTransaction())
        {
            var r = t.Read<User, string>("user123", out var u);
            Assert.IsTrue(r.Success);
            Assert.IsNotNull(u);
        }
    }

    [Test]
    public void CrudTransaction_write_data_delete_and_verify_test_1()
    {
        using (var t = GetTestTransaction())
        {
            t.Create(new User("user456", "User 4-5-6"));
        }

        using (var t = GetTestTransaction())
        {
            var r = t.Delete<User, string>("user456");
            Assert.IsTrue(r.Success);
        }

        using (var t = GetTestTransaction())
        {
            var r = t.Read<User, string>("user456", out var u);
            Assert.AreEqual(FailureReason.NotFound, r.Reason);
            Assert.IsNull(u);
        }
    }

    [Test]
    public async Task CrudTransaction_write_data_delete_and_verify_async_test_1()
    {
        await using (var t = GetTestTransaction())
        {
            t.Create(new User("user456", "User 4-5-6"));
            await t.CommitAsync();
        }

        await using (var t = GetTestTransaction())
        {
            var r = t.Delete<User, string>("user456");
            Assert.IsTrue(r.Success);
            await t.CommitAsync();
        }

        await using (var t = GetTestTransaction())
        {
            var r = t.Read<User, string>("user456", out var u);
            Assert.AreEqual(FailureReason.NotFound, r.Reason);
            Assert.IsNull(u);
        }
    }

    [Test]
    public void CrudTransaction_write_data_delete_and_verify_test_2()
    {
        using (var t = GetTestTransaction())
        {
            t.Create(new User("user456", "User 4-5-6"));
        }

        using (var t = GetTestTransaction())
        {
            var u = t.Read<User, string>("user456").ReturnValue!;
            var r = t.Delete(u);
            Assert.IsTrue(r.Success);
        }

        using (var t = GetTestTransaction())
        {
            var r = t.Read<User, string>("user456", out var u);
            Assert.AreEqual(FailureReason.NotFound, r.Reason);
            Assert.IsNull(u);
        }
    }

    [Test]
    public async Task CrudTransaction_write_data_delete_and_verify_async_test_2()
    {
        await using (var t = GetTestTransaction())
        {
            t.Create(new User("user456", "User 4-5-6"));
            await t.CommitAsync();
        }

        await using (var t = GetTestTransaction())
        {
            var u = t.Read<User, string>("user456").ReturnValue!;
            var r = t.Delete(u);
            Assert.IsTrue(r.Success);
            await t.CommitAsync();
        }

        await using (var t = GetTestTransaction())
        {
            var r = t.Read<User, string>("user456", out var u);
            Assert.AreEqual(FailureReason.NotFound, r.Reason);
            Assert.IsNull(u);
        }
    }

    [Test]
    public void CrudTransaction_update_and_verify_test()
    {
        using (var t = GetTestTransaction())
        {
            var r = t.Create(new User("user123", "User 1-2"));
            Assert.IsTrue(r.Success);
        }

        using (var t = GetTestTransaction())
        {
            t.Read<User, string>("user123", out var u);
            u!.PublicName = "User 1-2-3";
            var r = t.Update(u);
            Assert.IsTrue(r.Success);
        }

        using (var t = GetTestTransaction())
        {
            var r = t.Read<User, string>("user123", out var u);
            Assert.IsTrue(r.Success);
            Assert.IsNotNull(u);
            Assert.AreEqual("User 1-2-3", u!.PublicName);
        }
    }

    [Test]
    public async Task CrudTransaction_update_and_verify_async_test()
    {
        await using (var t = GetTestTransaction())
        {
            var r = t.Create(new User("user789", "User 1-2"));
            Assert.IsTrue(r.Success);
            Assert.IsTrue((await t.CommitAsync()).Success);
        }

        await using (var t = GetTestTransaction())
        {
            var u = (await t.ReadAsync<User, string>("user789")).ReturnValue;
            u!.PublicName = "User 1-2-3";
            Assert.IsTrue(t.Update(u).Success);
            Assert.IsTrue((await t.CommitAsync()).Success);
        }

        await using (var t = GetTestTransaction())
        {
            var r = t.Read<User, string>("user789", out var u);
            Assert.IsTrue(r.Success);
            Assert.IsNotNull(u);
            Assert.AreEqual("User 1-2-3", u!.PublicName);
        }
    }

    [Test]
    public async Task CrudTransaction_ReadAsync_test()
    {
        await using (var t = GetTestTransaction())
        {
            var r = t.Create(new User("user987", "User 9-8-7"));
            Assert.IsTrue(r.Success);
        }

        await using (var t = GetTestTransaction())
        {
            var r = await t.ReadAsync<User, string>("user987");
            Assert.IsTrue(r.Success);
            Assert.IsNotNull(r.ReturnValue);
        }
    }

    [Test]
    public async Task SearchAsync_test()
    {
        await using var t = GetTestTransaction();
        var r = (await t.SearchAsync<User>(p => p.PublicName != null)).ReturnValue!;
        Assert.IsNotNull(r);
        Assert.NotZero(r.Length);
    }
}