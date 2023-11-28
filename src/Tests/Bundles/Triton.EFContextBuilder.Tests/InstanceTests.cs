#pragma warning disable CS1591
#pragma warning disable CA1822

using TheXDS.Triton.Tests.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TheXDS.Triton.EfContextBuilder;

namespace TheXDS.Triton.Tests.EFContextBuilder;

[TestFixture]
public class InstanceTests
{
    public static void ConfigTest(DbContextOptionsBuilder options)
    {
        options.UseInMemoryDatabase("TestDb");
    }

    public void BrokenConfigTest(DbContextOptionsBuilder options)
    {            
    }

    [Test]
    public void ParametricInstancingBuilderTest()
    {
        TestContext(ContextBuilder.Build(new[] { typeof(Comment), typeof(Post), typeof(User) }, ConfigTest).New());
    }

    [Test]
    public void AutomaticInstancingBuilderTest()
    {
        TestContext(ContextBuilder.Build(ConfigTest).New());
    }

    [Test]
    public void Instancing_contracts_test()
    {
        Assert.Throws<ArgumentException>(() => ContextBuilder.Build(new[] { typeof(Comment), typeof(Exception) }));
        Assert.Throws<InvalidOperationException>(() => ContextBuilder.Build(BrokenConfigTest));
    }

    private static void TestContext(DbContext context)
    {
        Assert.That(context, Is.InstanceOf<DbContext>());
        Assert.That(context.Set<User>(), Is.AssignableTo<DbSet<User>>());
        Assert.That(context.Set<Comment>(), Is.AssignableTo<DbSet<Comment>>());
        Assert.That(context.Set<Post>(), Is.AssignableTo<DbSet<Post>>());
    }
}