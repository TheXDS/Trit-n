#pragma warning disable CS1591

using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Tests.Models;

public class BasicModelTests
{
    private class ConcurrentTestModel : ConcurrentModel<int>;

    private class CatalogModelTest : CatalogModel<int>;

    private class TestModel : Model<string>
    {
        public TestModel()
        {
        }

        public TestModel(string id) : base(id)
        {
        }
    }

    [Test]
    public void CatalogModel_T_includes_description()
    {
        var t = typeof(CatalogModelTest);
        Assert.That(t.GetProperties().SingleOrDefault(p => p.IsReadWrite() && p.PropertyType == typeof(string) && p.Name == nameof(CatalogModel<int>.Description)), Is.Not.Null);
        var x = new CatalogModelTest();
        Assert.That(x.Description, Is.Null);
        x.Description = "Test";
        Assert.That(x.Description, Is.EqualTo("Test"));
    }

    [Test]
    public void ConcurrentModel_T_includes_RowVersion()
    {
        var t = typeof(ConcurrentTestModel);
        Assert.That(t.GetProperties().SingleOrDefault(p => p.IsReadWrite() && p.PropertyType == typeof(byte[]) && p.HasAttribute<TimestampAttribute>()), Is.Not.Null);
        var x = new ConcurrentTestModel();
        Assert.That(x.RowVersion, Is.EqualTo(default(bool[])));
        var a = RandomNumberGenerator.GetBytes(16);
        x.RowVersion = a;
        Assert.That(x.RowVersion, Is.EqualTo(a));
    }

    [Test]
    public void IdAsString_is_not_null()
    {
        var t = new TestModel();
        Assert.That(t.Id, Is.Null);
        Assert.That(t.IdAsString, Is.Not.Null);

        var u = new ConcurrentTestModel();
        Assert.That(u.Id, Is.Zero);
        Assert.That(u.IdAsString, Is.EqualTo("0"));
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
        Assert.That(t.Id, Is.EqualTo("xabc1234"));
        Assert.That(t.IdAsString, Is.EqualTo("xabc1234"));
    }
}