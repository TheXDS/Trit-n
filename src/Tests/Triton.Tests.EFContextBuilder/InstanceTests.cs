#pragma warning disable CS1591

using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TheXDS.Triton.Models;
using TheXDS.Triton;

namespace Triton.Tests.EFContextBuilder
{
    [TestFixture]
    public class InstanceTests
    {
        public static void ConfigTest(DbContextOptionsBuilder options)
        {
            options.UseInMemoryDatabase("TestDb");
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

        private static void TestContext(DbContext context)
        {
            Assert.IsInstanceOf<DbContext>(context);
            Assert.IsInstanceOf<DbSet<User>>(context.Set<User>());
            Assert.IsInstanceOf<DbSet<Comment>>(context.Set<Comment>());
            Assert.IsInstanceOf<DbSet<Post>>(context.Set<Post>());
        }
    }
}