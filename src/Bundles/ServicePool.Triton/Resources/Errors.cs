using System;
using Ers = TheXDS.ServicePool.Triton.Resources.Strings.Errors;

namespace TheXDS.ServicePool.Triton.Resources
{
    internal static class Errors
    {
        public static ArgumentException TypeMustImplDbContext(string argName)
        {
            return new(Ers.TypeMustImplementDbContext, argName);
        }
    }
}
