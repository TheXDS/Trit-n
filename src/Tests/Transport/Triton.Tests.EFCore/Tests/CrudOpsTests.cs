#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.Triton.Tests.Models;

namespace TheXDS.Triton.Tests.EFCore.Tests;

public partial class CrudOpsTests : TritonEfTestClass
{
    [Test]
    public void RelatedDataEagerLoadingTest()
    {
        var q = _srv.GetAllUsersFirst3Posts().ToList();

        /* -= Según la base de datos de prueba: =-
         * Existen 3 usuarios, y únicamente el primer usuario debe tener un
         * Post. Los demás usuarios no deben tener ninguno.
         * 
         * Por la forma en que el Query está construido, solo se debe
         * obtener al primer usuario y a su correspondiente post.
         */

        Assert.That(q.Count, Is.EqualTo(1));
        Assert.That(q[0].Key.Id, Is.EqualTo("user1"));
        Assert.That(q[0].Count(), Is.EqualTo(1));
    }

    [Test]
    public void SimpleReadTransactionTest()
    {
        using var t = _srv.GetReadTransaction();

        Post? post = t.Read<Post, long>(1L);
        Assert.That(post, Is.InstanceOf<Post>());
        Assert.That(post!.Title, Is.EqualTo("Test"));

        Comment? comment = t.Read<Comment>(1L);
        Assert.That(comment, Is.InstanceOf<Comment>());
        Assert.That(comment!.Content, Is.EqualTo("It works!"));
    }

    [Test]
    public async Task FullyAsyncReadTransactionTest()
    {
        await using var t = _srv.GetReadTransaction();

        Post? post = await t.ReadAsync<Post, long>(1L);
        Assert.That(post, Is.InstanceOf<Post>());
        Assert.That(post!.Title, Is.EqualTo("Test"));

        Comment? comment = await t.ReadAsync<Comment>(1L);
        Assert.That(comment, Is.InstanceOf<Comment>());
        Assert.That(comment!.Content, Is.EqualTo("It works!"));
    }
}