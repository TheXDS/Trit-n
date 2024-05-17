using System.IO.Compression;

namespace TheXDS.StringPacker;

internal class Program
{
    private static readonly string[] separators = [",", ";"];

    static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine(@$"Parameter(s) missing.

Usage:
    {typeof(Program).Assembly.GetName().Name}.exe <output file> <strings>");
            return;
        }

        FileInfo outFile = new(args[0]);
        if (outFile.Exists) { outFile.Delete(); }
        using var fs = outFile.OpenWrite();
        using var ds = new DeflateStream(fs, CompressionMode.Compress);
        using var sw = new StreamWriter(ds);
        sw.Write(args[1..].SelectMany(p => p.Split(separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)).ToArray());
    }
}