using System.Collections.Generic;
using System.IO;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Triton.Middleware;

/// <summary>
/// Implementa un <see cref="TextJournal"/> que permite escribir las
/// entradas de bitácora en un archivo en el sistema de archivos.
/// </summary>
public class TextFileJournal : TextJournal
{
    private string? _path;

    /// <summary>
    /// Ruta del archivo a escribir.
    /// </summary>
    /// <value>
    /// Una ruta de archivo válida, o <see langword="null"/> para
    /// deshabilitar este escritor de bitácora.
    /// </value>
    /// <remarks>
    /// Esta propiedad se establecerá en <see langword="null"/> si ocurre
    /// un error al intentar escribir en el archivo especificado.
    /// </remarks>
    public string? Path
    {
        get => _path;
        set
        {
            if (value is not null)
            {
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