using System.Text;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Resources;
using static TheXDS.Triton.Fakers.Globals;

namespace TheXDS.Triton.Fakers
{
    public static class Text
    {

        public static string Lorem(in int words)
        {
            const double delta = 0.3; // Delta de variación, en %
            const double wps = 6;      // Palabras por oración
            const double spp = 5;      // Oraciones por párrafo.

            var text = new StringBuilder();
            
            var wc = 0; // Cuenta de palabras.
            do
            {
                text.Append(StringTables.Lorem.Pick());
                wc++;
                var wpts = Variate(wps, delta);
                var swc = 0;
                do
                {
                    if (wc == words) break;



                } while (swc < wps);
                text.AppendLine(".");
            } while (wc < words);

            return text.ToString();
        }

        private static string Capitalize(string value)
        {
            return value.Substring(0, 1).ToUpper() + value.Substring(1).ToLower();
        }

        private static double Variate(in double value, in double delta)
        {
            var d = value * delta;
            var d2 = (d * 2) - d;
            return value + _rnd.NextDouble() * d2;
        }
    }
}
