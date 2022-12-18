using System.Text;
using TheXDS.MCART.Math;
using TheXDS.MCART.Resources;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Faker.Resources;
using static TheXDS.Triton.Faker.Globals;

namespace TheXDS.Triton.Faker;

/// <summary>
/// Contiene funciones auxiliares de generación de texto aleatorio.
/// </summary>
public static class Text
{
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
        Lorem_Contract(words, wordsPerSentence, sentencesPerParagraph, delta);

        StringBuilder text = new();
        double wps = wordsPerSentence;
        double spp = sentencesPerParagraph;
        int twc = 0; // Cuenta de palabras en total.
        int swc = 0; // Cuenta de palabras por oración.
        int psc = 0; // Cuenta de oraciones por párrafo.
        do
        {
            var word = StringTables.Lorem.Pick();
            text.Append(swc != 0 ? word : Capitalize(word));
            twc++;
            swc++;
            if (swc > wps.Variate(delta))
            {
                TerminateSentence(text, ref psc, ref swc, spp, delta);
            }
            else
            {
                text.Append(' ');
            }
        } while (twc < words);
        return swc != 0 ? $"{text.ToString().TrimEnd()}." : text.ToString();
    }

    private static void Lorem_Contract(in int words, in int wordsPerSentence, in int sentencesPerParagraph, in double delta)
    {
        if (words < 1) throw Errors.ValueOutOfRange(nameof(words), 1, int.MaxValue);
        if (wordsPerSentence < 1) throw Errors.ValueOutOfRange(nameof(wordsPerSentence), 1, int.MaxValue);
        if (sentencesPerParagraph < 1) throw Errors.ValueOutOfRange(nameof(sentencesPerParagraph), 1, int.MaxValue);
        if (!delta.IsValid()) throw Errors.InvalidValue(nameof(delta));
    }

    private static void TerminateSentence(StringBuilder text, ref int psc, ref int swc, in double spp, in double delta)
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
}
