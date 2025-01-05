namespace TheXDS.Triton.Diagnostics.Middleware;

/// <summary>
/// Implements a <see cref="TextJournal"/> that allows writing log entries to the 
/// standard output of the application.
/// </summary>
public class StdoutJournal() : StreamJournal(Console.OpenStandardOutput)
{
}