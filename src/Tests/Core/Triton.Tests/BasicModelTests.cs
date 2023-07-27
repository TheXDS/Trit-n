#pragma warning disable CS1591

using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Tests;

public class BasicModelTests
{
    private class ConcurrentTestModel : ConcurrentModel<int>
    {
    }

    private class TestModel : Model<string>
    {
        public TestModel()
        {
        }

        public TestModel(string id): base(id)
        {
        }
    }

    [Test]
    public void ConcurrentModel_T_includes_RowVersion()
    {
        var t = typeof(ConcurrentTestModel);
        Assert.NotNull(t.GetProperties().SingleOrDefault(p => p.IsReadWrite() && p.PropertyType == typeof(byte[]) && p.HasAttribute<TimestampAttribute>()));
        var x = new ConcurrentTestModel();
        Assert.AreEqual(default(bool[]), x.RowVersion);
        var a = RandomNumberGenerator.GetBytes(16);
        x.RowVersion = a;
        Assert.AreEqual(a, x.RowVersion);
    }

    [Test]
    public void IdAsString_is_not_null()
    {
        var t = new TestModel();
        Assert.Null(t.Id);
        Assert.NotNull(t.IdAsString);

        var u = new ConcurrentTestModel();
        Assert.Zero(u.Id);
        Assert.AreEqual("0", u.IdAsString);
    }

    [Test]
    public void Model_ctor_throws_on_null_id()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new TestModel(null!));
    }

    [Test]
    public void Model_ctor_initializes_id()
    {
        var t = new TestModel("xabc1234");
        Assert.AreEqual("xabc1234", t.Id);
        Assert.AreEqual("xabc1234", t.IdAsString);
    }
}