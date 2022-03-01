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
            Factory = new TypeFactory($"{ReflectionHelpers.GetEntryPoint()?.DeclaringType?.Namespace ?? ReflectionHelpers.GetCallingMethod()?.DeclaringType?.Namespace ?? typeof(ContextBuilder).Namespace ?? nameof(ContextBuilder)}._generated", false);
        }

        /// <summary>
        /// Construye un nuevo contexto de datos utilizando los modelos
        /// especificados.
        /// </summary>
        /// <param name="models">
        /// Modelos de datos a incorporar como parte del contexto de datos a
        /// generar.
        /// </param>
        /// <param name="configurationCallback">
        /// Método a invocar para configurar externamente el contexto de datos
        /// generado.
        /// </param>
        /// <returns>
        /// Un <see cref="TypeBuilder{T}"/> con el que se puede instanciar un
        /// nuevo contexto de datos.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Se produce si algún elemento de la colección
        /// <paramref name="models"/> no hereda del tipo base
        /// <see cref="Model"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Se produce si el método especificado en <paramref name="configurationCallback"/> no es un método estático.
        /// </exception>
        public static TypeBuilder<DbContext> Build(Type[] models, Action<DbContextOptionsBuilder>? configurationCallback)
        {
            return Build($"DynamicDbContext_{Guid.NewGuid()}", models, configurationCallback);
        }

        /// <summary>
        /// Construye un nuevo contexto de datos utilizando los modelos
        /// especificados.
        /// </summary>
        /// <param name="name">Nombre del nuevo contexto de datos.</param>
        /// <param name="models">
        /// Modelos de datos a incorporar como parte del contexto de datos a
        /// generar.
        /// </param>
        /// <param name="configurationCallback">
        /// Método a invocar para configurar externamente el contexto de datos
        /// generado. Debe ser un método estático.
        /// </param>
        /// <returns>
        /// Un <see cref="TypeBuilder{T}"/> con el que se puede instanciar un
        /// nuevo contexto de datos.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Se produce si <paramref name="name"/> es una cadena vacía o
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Se produce si algún elemento de la colección
        /// <paramref name="models"/> no hereda del tipo base
        /// <see cref="Model"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Se produce si el método especificado en <paramref name="configurationCallback"/> no es un método estático.
        /// </exception>
        public static TypeBuilder<DbContext> Build(string name, Type[] models, Action<DbContextOptionsBuilder>? configurationCallback)
        {
            if (models.Any(p => !p.Implements<Model>())) throw new ArgumentException(null, nameof(models));
            var t = Factory.NewType<DbContext>(name);
            foreach (var j in models)
            {
                t.Builder.AddAutoProperty($"{j.Name}{(j.Name.EndsWith("s") ? "es" : "s")}", typeof(DbSet<>).MakeGenericType(j));
            }
            if (configurationCallback is { Method: MethodInfo callback })
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
        /// <exception cref="ArgumentException">
        /// Se produce si algún elemento de la colección
        /// <paramref name="models"/> no hereda del tipo base
        /// <see cref="Model"/>.
        /// </exception>
        public static TypeBuilder<DbContext> Build(Type[] models) => Build(models, null);

        /// <summary>
        /// Construye un nuevo contexto de datos utilizando los modelos
        /// especificados.
        /// </summary>
        /// <param name="name">Nombre del nuevo contexto de datos.</param>
        /// <param name="models">
        /// Modelos de datos a incorporar como parte del contexto de datos a
        /// generar.
        /// </param>
        /// <returns>
        /// Un <see cref="TypeBuilder{T}"/> con el que se puede instanciar un
        /// nuevo contexto de datos.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Se produce si <paramref name="name"/> es una cadena vacía o
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Se produce si algún elemento de la colección
        /// <paramref name="models"/> no hereda del tipo base
        /// <see cref="Model"/>.
        /// </exception>
        public static TypeBuilder<DbContext> Build(string name, Type[] models) => Build(name, models, null);

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
        /// <param name="name">Nombre del nuevo contexto de datos.</param>
        /// <returns>
        /// Un <see cref="TypeBuilder{T}"/> con el que se puede instanciar un
        /// nuevo contexto de datos.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Se produce si <paramref name="name"/> es una cadena vacía o
        /// <see langword="null"/>.
        /// </exception>
        public static TypeBuilder<DbContext> Build(string name) => Build(name, (Action<DbContextOptionsBuilder>?)null);

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
        /// <exception cref="InvalidOperationException">
        /// Se produce si el método especificado en <paramref name="configurationCallback"/> no es un método estático.
        /// </exception>
        public static TypeBuilder<DbContext> Build(Action<DbContextOptionsBuilder>? configurationCallback)
        {
            return Build(Objects.GetTypes<Model>().Where(p => !p.IsAbstract).ToArray(), configurationCallback);
        }

        /// <summary>
        /// Construye un nuevo contexto de datos, descubriendo todos los
        /// modelos de datos disponibles en la aplicación.
        /// </summary>
        /// <param name="name">Nombre del nuevo contexto de datos.</param>
        /// <param name="configurationCallback">
        /// Método a invocar para configurar externamente el contexto de datos
        /// generado.
        /// </param>
        /// <returns>
        /// Un <see cref="TypeBuilder{T}"/> con el que se puede instanciar un
        /// nuevo contexto de datos.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Se produce si <paramref name="name"/> es una cadena vacía o
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Se produce si el método especificado en <paramref name="configurationCallback"/> no es un método estático.
        /// </exception>
        public static TypeBuilder<DbContext> Build(string name, Action<DbContextOptionsBuilder>? configurationCallback)
        {
            return Build(name, Objects.GetTypes<Model>().Where(p => !p.IsAbstract).ToArray(), configurationCallback);
        }

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
