namespace TheXDS.Triton.Diagnostics.Middleware;

/// <summary>
/// Implements a <see cref="TextJournal"/> that allows writing log entries to a <see cref="Stream"/>.
/// </summary>
/// <param name="getStream">Function used to obtain a <see cref="Stream"/>.</param>
public class StreamJournal(Func<Stream> getStream) : TextJournal
{
    private readonly Func<Stream> _getStream = getStream;

    /// <inheritdoc/>
    protected override void WriteText(IEnumerable<string> lines)
    {
        using var stream = _getStream.Invoke();
        using var writer = new StreamWriter(stream);
        foreach (var line in lines)
        {
            writer.WriteLine(line);
        }
    }
}