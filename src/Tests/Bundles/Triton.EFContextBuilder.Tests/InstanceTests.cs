using TheXDS.Triton.Tests.Models;

#pragma warning disable CS1591
#pragma warning disable CA1822

namespace TheXDS.Triton.Tests.EFContextBuilder;
using System;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TheXDS.Triton.EfContextBuilder;

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
        Assert.IsInstanceOf<DbContext>(context);
        Assert.IsInstanceOf<DbSet<User>>(context.Set<User>());
        Assert.IsInstanceOf<DbSet<Comment>>(context.Set<Comment>());
        Assert.IsInstanceOf<DbSet<Post>>(context.Set<Post>());
    }
}