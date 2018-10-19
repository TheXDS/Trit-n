using System;
using System.Reflection;
using System.Reflection.Emit;

namespace TheXDS.Triton.ViewModel
{
    internal static class Builders
    {
        private const string Namespace = "TheXDS.Triton.ViewModel._Generated";
        public static readonly ModuleBuilder MBuilder = 
            AppDomain.CurrentDomain.DefineDynamicAssembly(
                    new AssemblyName(Namespace), 
                    AssemblyBuilderAccess.Run)
                .DefineDynamicModule(Namespace);
    }
}