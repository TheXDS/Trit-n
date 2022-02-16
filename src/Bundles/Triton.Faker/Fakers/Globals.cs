using System;

namespace TheXDS.Triton.Fakers
{
    internal static class Globals
    {
        public static readonly Random _rnd = new();

        public static string Capitalize(string value)
        {
            return value[..1].ToUpper() + value[1..].ToLower();
        }
    }
}
