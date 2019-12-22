using System.Runtime.CompilerServices;
using TheXDS.MCART.Resources;

namespace TheXDS.Triton.Resources
{
    internal static class StringTables
    {
        private static readonly StringUnpacker _u = new StringUnpacker(typeof(StringTables).Assembly, typeof(StringTables).FullName!);

        private static string[]? _femaleNames;
        private static string[]? _maleNames;
        private static string[]? _surnames;
        private static string[]? _lorem;

        public static string[] FemaleNames => Unpack(ref _femaleNames);
        public static string[] MaleNames => Unpack(ref _maleNames);
        public static string[] Surnames => Unpack(ref _surnames);
        public static string[] Lorem => Unpack(ref _lorem);

        private static string[] Unpack(ref string[]? array, [CallerMemberName]string id = null!)
        {
            return array ??= _u.Unpack(id.ToLower()).Split(',');
        }
    }
}
