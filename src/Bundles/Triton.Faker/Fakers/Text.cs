using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Resources;
using static TheXDS.Triton.Fakers.Globals;
using static TheXDS.MCART.Common;

namespace TheXDS.Triton.Fakers
{
    /// <summary>
    /// Contiene funciones auxiliares de generación de texto aleatorio.
    /// </summary>
    public static class Text
    {
        /// <summary>
        /// Genera una dirección física aleatoria.
        /// </summary>
        /// <returns>Una dirección física aleatoria.</returns>
        public static string FakeAddress()
        {
            var l = new List<string>();
            if (_rnd.CoinFlip()) l.Add(_rnd.Next(1, 300).ToString());
            if (_rnd.CoinFlip()) l.Add(new[] { "N", "NE", "E", "SE", "S", "SW", "W", "NW" }.Pick());
            l.Add(_rnd.CoinFlip() ? StringTables.Lorem.Pick() : new[] { "1st", "2nd", "3rd" }.Concat(Sequence(4, 100).Select(p => $"{p}th")).Pick());
            l.Add(new[] { "Ave.", "Road", "Street", "Highway" }.Pick());
            return string.Join(' ', l);
        }

        public static string Lorem(in int words)
        {
            const double delta = 0.3;  // Delta de variación, en %
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
