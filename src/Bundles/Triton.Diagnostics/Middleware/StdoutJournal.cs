namespace TheXDS.Triton.Diagnostics.Middleware;

/// <summary>
/// Implementa un <see cref="TextJournal"/> que permite escribir las
/// entradas de bitácora en la salida estándar de la aplicación.
/// </summary>
public class StdoutJournal() : StreamJournal(Console.OpenStandardOutput)
{
}
