#pragma warning disable CS1591

using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Tests
{
    public class BasicModelTests
    {
        private class ConcurrentTestModel : ConcurrentModel<int>
        {
        }

        [Test]
        public void ConcurrentModel_T_includes_RowVersion()
        {
            var t = typeof(ConcurrentTestModel);
            Assert.NotNull(t.GetProperties().SingleOrDefault(p => p.IsReadWrite() && p.PropertyType == typeof(byte[]) && p.HasAttr<TimestampAttribute>()));
            var x = new ConcurrentTestModel();
            Assert.AreEqual(default(bool[]), x.RowVersion);
            var a = RandomNumberGeneratorExtensions.GetBytes(16);
            x.RowVersion = a;
            Assert.AreEqual(a, x.RowVersion);

        }
    }
}