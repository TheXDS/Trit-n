#pragma warning disable CS1591

using NUnit.Framework;
using System.Linq;

namespace TheXDS.Triton.Tests
{
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
    }
}
