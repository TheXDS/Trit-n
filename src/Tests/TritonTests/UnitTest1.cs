#pragma warning disable CS1591

using NUnit.Framework;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

namespace TheXDS.Triton.Tests
{
    public class Tests
    {
        /* COSAS A PROBAR
         * ==============
         * - Carga de AppDomain en Net Core 3
         *
         */

        [SetUp]
        public void Setup()
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [Test]
        public void AppDomainCreationTest()
        {
            const string p = @"C:\Users\xds_x\source\repos\TheXDS\MCART\Build\bin\Consoleer\Debug\netcoreapp3.0" + @"\Consoleer.dll";
            var x = MCART.Resources.RtInfo.RtSupport(this.GetType().Assembly);
            var alc = new TestLoadContext(p);
            var asm = new WeakReference(alc.LoadFromAssemblyPath(p), false);
            Assert.True(asm.IsAlive);
            Assert.NotNull(asm.Target);
            ((Assembly)asm.Target).EntryPoint.Invoke(null, new object[] { new string[] { "--Detail:alot" } });
            alc.Unload();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private class TestLoadContext: AssemblyLoadContext
        {
            private AssemblyDependencyResolver _resolver;

            public TestLoadContext(string path) : base(true)
            {
                _resolver = new AssemblyDependencyResolver(path);
            }

            protected override Assembly? Load(AssemblyName assemblyName)
            {
                return (_resolver.ResolveAssemblyToPath(assemblyName) is string path)
                    ? LoadFromAssemblyPath(path)
                    : null;
            }
        }
    }
}