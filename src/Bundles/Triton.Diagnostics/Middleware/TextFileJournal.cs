using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Triton.Diagnostics.Middleware;

/// <summary>
/// Implements a <see cref="TextJournal"/> that allows writing log entries to a
/// file in the file system.
/// </summary>
public class TextFileJournal : TextJournal
{
    private string? _path;

    /// <summary>
    /// Path of the file to write to.
    /// </summary>
    /// <value>
    /// A valid file path, or <see langword="null"/> to disable this log
    /// writer.
    /// </value>
    /// <remarks>
    /// This property will be set to <see langword="null"/> if an error occurs
    /// while trying to write to the specified file.
    /// </remarks>
    public string? Path
    {
        get => _path;
        set
        {
            if (value is not null)
            {
                // HACK: Simple path validation.
                _ = new FileInfo(value);
            }
            _path = value;
        }
    }

    /// <inheritdoc/>
    protected override void WriteText(IEnumerable<string> lines)
    {
        try
        {
            if (!Path.IsEmpty()) File.AppendAllLines(Path, lines);
        }
        catch
        {
            Path = null;
            throw;
        }
    }
}