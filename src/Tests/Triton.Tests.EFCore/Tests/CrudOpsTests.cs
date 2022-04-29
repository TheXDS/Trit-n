using TheXDS.Triton.Tests.Models;

#pragma warning disable CS1591

namespace TheXDS.Triton.Tests.EFCore.Tests;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

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

        Assert.AreEqual(1, q.Count);
        Assert.AreEqual("user1", q[0].Key.Id);
        Assert.AreEqual(1, q[0].Count());
    }

    [Test]
    public void SimpleReadTransactionTest()
    {
        using var t = _srv.GetReadTransaction();

        Post? post = t.Read<Post, long>(1L);
        Assert.IsInstanceOf<Post>(post);
        Assert.AreEqual("Test", post!.Title);

        Comment? comment = t.Read<Comment>(1L);
        Assert.IsInstanceOf<Comment>(comment);
        Assert.AreEqual("It works!", comment!.Content);
    }

    [Test]
    public async Task FullyAsyncReadTransactionTest()
    {
        await using var t = _srv.GetReadTransaction();

        Post? post = await t.ReadAsync<Post, long>(1L);
        Assert.IsInstanceOf<Post>(post);
        Assert.AreEqual("Test", post!.Title);

        Comment? comment = await t.ReadAsync<Comment>(1L);
        Assert.IsInstanceOf<Comment>(comment);
        Assert.AreEqual("It works!", comment!.Content);
    }
}