using NUnit.Framework;
using System.Globalization;
using TheXDS.Triton.Resources.Strings;

namespace TheXDS.Triton.Tests.Resources
{
    internal class StringsTests
    {
        [Test]
        public void String_resources_test()
        {
            foreach (var j in typeof(Common).GetProperties().Where(p => p.PropertyType == typeof(string)))
            {
                var neutralString = j.GetValue(null);
                Assert.IsInstanceOf<string>(neutralString);
                Assert.IsNotEmpty(neutralString!.ToString());
            }
        }

        [TestCase("en-US")]
        public void Translation_test(string culture)
        {
            var c = CultureInfo.CreateSpecificCulture(culture);
            foreach (var j in typeof(Common).GetProperties().Where(p => p.PropertyType == typeof(string)))
            {
                Common.Culture = null!;
                var neutralString = j.GetValue(null);

                Common.Culture = c;
                var translated = j.GetValue(null);

                Assert.IsInstanceOf<string>(translated);
                Assert.IsNotEmpty(translated!.ToString());

                Assert.AreNotEqual(neutralString!.ToString(), translated!.ToString());
            }
        }
    }
}
