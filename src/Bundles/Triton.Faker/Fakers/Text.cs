using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheXDS.MCART.Math;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Resources;
using static TheXDS.MCART.Common;
using static TheXDS.Triton.Fakers.Globals;

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
            l.Add(_rnd.CoinFlip() ? StringTables.Surnames.Pick() : new[] { "1st", "2nd", "3rd" }.Concat(Sequence(4, 100).Select(p => $"{p}th")).Pick());
            l.Add(new[] { "Ave.", "Road", "Street", "Highway" }.Pick());
            return string.Join(' ', l);
        }

        /// <summary>
        /// Obtiene un texto aleatorio de tipo Lorem con la cantidad de
        /// palabras especificadas.
        /// </summary>
        /// <param name="words">Cantidad de palabras a generar.</param>
        /// <returns>
        /// Un texto aleatorio de tipo Lorem Ipsum.
        /// </returns>
        public static string Lorem(in int words)
        {
            const double delta = 0.3;  // Delta de variación, en %
            const double wps = 8;      // Palabras por oración
            const double spp = 6;      // Oraciones por párrafo.

            var text = new StringBuilder();
            
            var twc = 0; // Cuenta de palabras en total.
            var swc = 0; // Cuenta de palabras por oración.
            var psc = 0; // Cuenta de oraciones por párrafo.

            do
            {
                var word = StringTables.Lorem.Pick();
                text.Append(swc != 0 ? word : Capitalize(word));
                twc++;
                swc++;

                if (swc > wps.Variate(delta))
                {
                    text.Append(". ");
                    swc = 0;
                    psc++;
                }
                if (psc > spp.Variate(delta))
                {
                    text.AppendLine(".");
                    psc = 0;
                    swc = 0;
                }
            } while (twc < words);

            return text.ToString();
        }

        private static string Capitalize(string value)
        {
            return value.Substring(0, 1).ToUpper() + value.Substring(1).ToLower();
        }
    }
}
