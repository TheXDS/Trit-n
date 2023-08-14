#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.Triton.Services;
using TheXDS.Triton.Tests.Models;
using TheXDS.Triton.Tests.Services;

namespace TheXDS.Triton.Tests;

[SetUpFixture]
public class DbPopulation
{
    [OneTimeSetUp]
    public void PopulateDb()
    {
        using var t = new TritonService(new TestTransFactory()).GetTransaction();
        if (!t.All<User>().Any())
        {
            User u1, u2, u3;
            Post post;

            t.Create(
                u1 = new("user1", "User #1", new DateTime(2001, 1, 1)),
                u2 = new("user2", "User #2", new DateTime(2009, 3, 4)),
                u3 = new("user3", "User #3", new DateTime(2004, 9, 11))
            );
            t.Create(post = new Post("Test", "This is a test.", u1, new DateTime(2016, 12, 31)) { Published = true, Id = 1L });
            t.Create(
                new Comment(u2, post, "It works!", new DateTime(2017, 1, 1)) { Id = 1L },
                new Comment(u3, post, "Yay! c:", new DateTime(2017, 1, 2)) { Id = 2L },
                new Comment(u1, post, "Shuddap >:(", new DateTime(2017, 1, 3)) { Id = 3L },
                new Comment(u3, post, "ok :c", new DateTime(2017, 1, 4)) { Id = 4L }
            );
        }
    }

    [OneTimeTearDown]
    public void Teardown()
    {
        TestCrudTransaction.Wipe();
    }
}