using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton
{
    /// <summary>
    /// Contiene métodos para gestionar la generación dinámica de contextos de
    /// datos para Entity Framework Core.
    /// </summary>
    public static class ContextBuilder
    {
        private static readonly TypeFactory Factory;

        /// <summary>
        /// Inicializa la clase <see cref="ContextBuilder"/>
        /// </summary>
        static ContextBuilder()
        {
            Factory = new TypeFactory($"{ReflectionHelpers.GetEntryPoint()?.DeclaringType?.Namespace ?? ReflectionHelpers.GetCallingMethod()?.DeclaringType?.Namespace ?? typeof(ContextBuilder).Namespace ?? nameof(ContextBuilder)}._generated", true);
        }

        /// <summary>
        /// Construye un nuevo contexto de datos utilizando los modelos
        /// especificados.
        /// </summary>
        /// <param name="models">
        /// Modelos de datos a incorporar como parte del contexto de datos a
        /// generar.
        /// </param>
        /// <param name="setupCallback">
        /// Método a invocar para configurar externamente el contexto de datos
        /// generado.
        /// </param>
        /// <returns>
        /// Un <see cref="TypeBuilder{T}"/> con el que se puede instanciar un
        /// nuevo contexto de datos.
        /// </returns>
        public static TypeBuilder<DbContext> Build(Type[] models, Action<DbContextOptionsBuilder>? setupCallback)
        {
            if (models.Any(p => !p.Implements<Model>())) throw new ArgumentException(null, nameof(models));
            var t = Factory.NewType<DbContext>($"DynamicDbContext_{models.Aggregate(0, (a, j) => a ^ j.GetHashCode())}");
            foreach (var j in models)
            {
                t.Builder.AddAutoProperty($"{j.Name}{(j.Name.EndsWith("s") ? "es" : "s")}", typeof(DbSet<>).MakeGenericType(j));
            }
            if (setupCallback is { Method: MethodInfo callback })
            {
                if (!callback.IsStatic) throw new InvalidOperationException();
                if (typeof(DbContext).GetMethod("OnConfiguring", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(DbContextOptionsBuilder) }, null) is { } oc)
                {
                    t.Builder.AddOverride(oc).Il.LoadArg1().Call(callback).Return();
                }
            }
            return t;
        }

        /// <summary>
        /// Construye un nuevo contexto de datos utilizando los modelos
        /// especificados.
        /// </summary>
        /// <param name="models">
        /// Modelos de datos a incorporar como parte del contexto de datos a
        /// generar.
        /// </param>
        /// <returns>
        /// Un <see cref="TypeBuilder{T}"/> con el que se puede instanciar un
        /// nuevo contexto de datos.
        /// </returns>
        public static TypeBuilder<DbContext> Build(Type[] models) => Build(models, null);

        /// <summary>
        /// Construye un nuevo contexto de datos, descubriendo todos los
        /// modelos de datos disponibles en la aplicación.
        /// </summary>
        /// <returns>
        /// Un <see cref="TypeBuilder{T}"/> con el que se puede instanciar un
        /// nuevo contexto de datos.
        /// </returns>
        public static TypeBuilder<DbContext> Build() => Build((Action<DbContextOptionsBuilder>?)null);

        /// <summary>
        /// Construye un nuevo contexto de datos, descubriendo todos los
        /// modelos de datos disponibles en la aplicación.
        /// </summary>
        /// <param name="configurationCallback">
        /// Método a invocar para configurar externamente el contexto de datos
        /// generado.
        /// </param>
        /// <returns>
        /// Un <see cref="TypeBuilder{T}"/> con el que se puede instanciar un
        /// nuevo contexto de datos.
        /// </returns>
        public static TypeBuilder<DbContext> Build(Action<DbContextOptionsBuilder>? configurationCallback) => Build(Objects.GetTypes<Model>().Where(p => !p.IsAbstract).ToArray(), configurationCallback);

        /// <summary>
        /// Obtiene un <see cref="ITransactionFactory"/> conectado a un
        /// contexto de datos de Entity Framework Core generado dinámicamente.
        /// </summary>
        /// <param name="type">
        /// Constructor de tipos utilizado para generar un contexto de datos de
        /// Entity Framework Core.
        /// </param>
        /// <returns>
        /// Un <see cref="ITransactionFactory"/> conectado a un contexto de
        /// datos de Entity Framework Core generado dinámicamente.
        /// </returns>
        public static ITransactionFactory GetEfTransFactory(this TypeBuilder<DbContext> type)
        {
            return typeof(EfCoreTransFactory<>).MakeGenericType(type.Builder.CreateType()!).New<ITransactionFactory>();
        }
    }
}
