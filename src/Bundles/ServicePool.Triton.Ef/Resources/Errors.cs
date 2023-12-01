using Ers = TheXDS.ServicePool.Triton.Ef.Resources.Strings.Errors;

namespace TheXDS.ServicePool.Triton.Resources;

internal static class Errors
{
    public static ArgumentException TypeMustImplDbContext(string argName)
    {
        return new(Ers.TypeMustImplementDbContext, argName);
    }
}
