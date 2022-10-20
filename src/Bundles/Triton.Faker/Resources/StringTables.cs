using System.Runtime.CompilerServices;
using TheXDS.MCART.Resources;

namespace TheXDS.Triton.Faker.Resources;

internal static class StringTables
{
    private static readonly StringUnpacker _u = new(typeof(StringTables).Assembly, typeof(StringTables).FullName!);

    private static string[]? _femaleNames = null;
    private static string[]? _maleNames = null;
    private static string[]? _surnames = null;
    private static string[]? _lorem = null;

    /// <summary>
    /// Obtiene un arreglo de nombres femeninos conocidos.
    /// </summary>
    public static string[] FemaleNames => Unpack(ref _femaleNames);

    /// <summary>
    /// Obtiene un arreglo de nombres masculinos conocidos.
    /// </summary>
    public static string[] MaleNames => Unpack(ref _maleNames);

    /// <summary>
    /// Obtiene un arreglo de apellidos conocidos.
    /// </summary>
    public static string[] Surnames => Unpack(ref _surnames);

    /// <summary>
    /// Obtiene un arreglo de palabas en latín para la generación de texto
    /// de tipo Lorem Ipsum.
    /// </summary>
    public static string[] Lorem => Unpack(ref _lorem);

    private static string[] Unpack(ref string[]? array, [CallerMemberName]string id = null!)
    {
        return array ??= _u.Unpack(id.ToLower(), new DeflateGetter()).Split(',');
    }
}
