using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheXDS.MCART.Math;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Faker.Resources;
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
        public static Address GetAddress()
        {
            static string RndAddress()
            {
                var l = new List<string>();
                if (_rnd.CoinFlip()) l.Add(_rnd.Next(1, 300).ToString());
                if (_rnd.CoinFlip()) l.Add(new[] { "N", "NE", "E", "SE", "S", "SW", "W", "NW" }.Pick());
                l.Add(_rnd.CoinFlip() ? Capitalize(StringTables.Surnames.Pick()) : GetOrdinal(_rnd.Next(1, 130)));
                l.Add(new[] { "Ave.", "Road", "Street", "Highway" }.Pick());
                return string.Join(' ', l);
            }
            static string? RndLine2() => _rnd.CoinFlip() ? $"{new[] { "#", "Apt.", "House" }.Pick()} {_rnd.Next(1, 9999)}" : null;
            static string RndCity() => string.Join(' ', new string?[] { Capitalize(StringTables.Surnames.Pick()), _rnd.CoinFlip() ? "City" : null }.NotNull());
            static string RndCountry() => new System.Globalization.RegionInfo(System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.SpecificCultures).Pick().Name).EnglishName;
            return new(RndAddress(), RndLine2(), RndCity(), RndCountry(), (ushort)_rnd.Next(10001, 99999));
        }

        private static string GetOrdinal(int value)
        {
            var l = value.ToString().PadLeft(2, '0')[..2];
            return value.ToString().Last() switch
            {
                '1' when l != "11" => $"{value}st",
                '2' when l != "12" => $"{value}nd",
                '3' when l != "13" => $"{value}rd",
                _ => $"{value}th"
            };
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
            return Lorem(words, 7, 4);
        }

        /// <summary>
        /// Obtiene un texto aleatorio de tipo Lorem con la cantidad de
        /// palabras especificadas.
        /// </summary>
        /// <param name="words">Cantidad de palabras a generar.</param>
        /// <param name="wordsPerSentence">Palabras por oración.</param>
        /// <param name="sentencesPerParagraph">Oraciones por párrafo.</param>
        /// <returns>
        /// Un texto aleatorio de tipo Lorem Ipsum.
        /// </returns>
        public static string Lorem(in int words, in int wordsPerSentence, in int sentencesPerParagraph)
        {
            return Lorem(words, wordsPerSentence, sentencesPerParagraph, 0.3);
        }

        /// <summary>
        /// Obtiene un texto aleatorio de tipo Lorem con la cantidad de
        /// palabras especificadas.
        /// </summary>
        /// <param name="words">Cantidad de palabras a generar.</param>
        /// <param name="wordsPerSentence">Palabras por oración.</param>
        /// <param name="sentencesPerParagraph">Oraciones por párrafo.</param>
        /// <param name="delta">Delta de variación, en %</param>
        /// <returns>
        /// Un texto aleatorio de tipo Lorem Ipsum.
        /// </returns>
        public static string Lorem(in int words, in int wordsPerSentence, in int sentencesPerParagraph, in double delta)
        {
            double wps = wordsPerSentence;
            double spp = sentencesPerParagraph;
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
                    if (_rnd.CoinFlip())
                    {
                        text.Append('.');
                        psc++;
                        swc = 0;
                        if (psc > spp.Variate(delta))
                        {
                            text.AppendLine();
                            psc = 0;
                            swc = 0;
                        }
                        else
                        {
                            text.Append(' ');
                        }
                    }
                    else
                    {
                        text.Append(", ");
                        swc = 1;
                    }
                }
                else
                {
                    text.Append(' ');
                }
            } while (twc < words);
            return swc != 0 ? $"{text.ToString().TrimEnd()}." : text.ToString();
        }
    }
}
