namespace TheXDS.Triton.Middleware
{
    /// <summary>
    /// Implementa un <see cref="TextJournal"/> que permite escribir las
    /// entradas de bitácora en un archivo en el sistema de archivos.
    /// </summary>
    public class StdoutJournal : TextJournal
    {
        /// <inheritdoc/>
        protected override void WriteText(System.Collections.Generic.IEnumerable<string> lines)
        {
            using var stdout = System.Console.OpenStandardOutput();
            using var writer = new System.IO.StreamWriter(stdout);
            foreach (var j in lines)
            {
                writer.WriteLine(j);
            }
        }
    }
}