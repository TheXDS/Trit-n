#pragma warning disable CS1591

using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TheXDS.Triton.Models;
using Triton.EfContextBuilder;

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
            var b = ContextBuilder.Build(new[] { typeof(Comment), typeof(Post), typeof(User) }, ConfigTest).New();
            Assert.IsInstanceOf<DbContext>(b);
            TestContextSets(b);
        }

        private static void TestContextSets(DbContext context)
        {
            Assert.IsInstanceOf<DbSet<User>>(context.Set<User>());
            Assert.IsInstanceOf<DbSet<Comment>>(context.Set<Comment>());
            Assert.IsInstanceOf<DbSet<Post>>(context.Set<Post>());
        }
    }
}