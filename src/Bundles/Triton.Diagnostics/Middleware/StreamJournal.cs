namespace TheXDS.Triton.Diagnostics.Middleware;

/// <summary>
/// Implementa un <see cref="TextJournal"/> que permite escribir las
/// entradas de bitácora en un <see cref="Stream"/>.
/// </summary>
/// <param name="getStream">
/// Función a utilizar para obtener un <see cref="Stream"/>.
/// </param>
public class StreamJournal(Func<Stream> getStream) : TextJournal
{
    private readonly Func<Stream> _getStream = getStream;

    /// <inheritdoc/>
    protected override void WriteText(IEnumerable<string> lines)
    {
        using var stream = _getStream.Invoke();
        using var writer = new StreamWriter(stream);
        foreach (var j in lines)
        {
            writer.WriteLine(j);
        }
    }
}