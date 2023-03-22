namespace TheXDS.Triton.Diagnostics.Middleware;

/// <summary>
/// Implementa un <see cref="TextJournal"/> que permite escribir las
/// entradas de bitácora en la salida estándar de la aplicación.
/// </summary>
public class StdoutJournal : TextJournal
{
    /// <inheritdoc/>
    protected override void WriteText(IEnumerable<string> lines)
    {
        using var stdout = Console.OpenStandardOutput();
        using var writer = new StreamWriter(stdout);
        foreach (var j in lines)
        {
            writer.WriteLine(j);
        }
    }
}