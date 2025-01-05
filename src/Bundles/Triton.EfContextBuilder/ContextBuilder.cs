using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.EfContextBuilder;

/// <summary>
/// Contains methods for dynamically managing data contexts for Entity Framework Core.
/// </summary>
public static class ContextBuilder
{
    private static readonly TypeFactory Factory;

    /// <summary>
    /// Initializes the <see cref="ContextBuilder"/> class.
    /// </summary>
    static ContextBuilder()
    {
        Factory = new TypeFactory($"{ReflectionHelpers.GetEntryPoint()?.DeclaringType?.Namespace
            ?? ReflectionHelpers.GetCallingMethod()?.DeclaringType?.Namespace
            ?? typeof(ContextBuilder).Namespace
            ?? nameof(ContextBuilder)}._generated", false);
    }

    /// <summary>
    /// Builds a new data context using the specified models.
    /// </summary>
    /// <param name="models">
    /// Data models to incorporate as part of the data context to be generated.
    /// </param>
    /// <param name="configurationCallback">
    /// Method to invoke externally to configure the generated data context.
    /// </param>
    /// <returns>
    /// A <see cref="TypeBuilder{T}"/> with which a new data context can be instantiated.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if any element in the collection
    /// <paramref name="models"/> does not inherit from the base type
    /// <see cref="Model"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the method specified in <paramref name="configurationCallback"/> is not a static method.
    /// </exception>
    public static ITypeBuilder<DbContext> Build(Type[] models, Action<DbContextOptionsBuilder>? configurationCallback)
    {
        return Build($"DynamicDbContext_{Guid.NewGuid()}", models, configurationCallback);
    }

    /// <summary>
    /// Builds a new data context using the specified models.
    /// </summary>
    /// <param name="name">Name of the new data context.</param>
    /// <param name="models">
    /// Data models to incorporate as part of the data context to be generated.
    /// </param>
    /// <param name="configurationCallback">
    /// Method to invoke externally to configure the generated data context.
    /// Must be a static method.
    /// </param>
    /// <returns>
    /// A <see cref="TypeBuilder{T}"/> with which a new data context can be
    /// instantiated.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="name"/> is an empty string or
    /// <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if any element in the collection
    /// <paramref name="models"/> does not inherit from the base type
    /// <see cref="Model"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the method specified in
    /// <paramref name="configurationCallback"/> is not a static method.
    /// </exception>
    public static ITypeBuilder<DbContext> Build(string name, Type[] models, Action<DbContextOptionsBuilder>? configurationCallback)
    {
        if (models.Any(p => !p.Implements<Model>())) throw new ArgumentException(null, nameof(models));
        var t = Factory.NewType<DbContext>(name);
        foreach (var j in models)
        {
            t.Builder.AddAutoProperty($"{j.Name}{(j.Name.EndsWith("s") ? "es" : "s")}", typeof(DbSet<>).MakeGenericType(j));
        }
        if (configurationCallback is { Method: { } callback })
        {
            if (!callback.IsStatic) throw new InvalidOperationException();
            if (typeof(DbContext).GetMethod("OnConfiguring", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(DbContextOptionsBuilder) }, null) is { } oc)
            {
                t.Builder.AddOverride(oc).Il.LoadArg1().Call(callback).Return();
            }
        }
        return t;
    }

    /// <summary>
    /// Builds a new data context using the specified models.
    /// </summary>
    /// <param name="models">
    /// Data models to incorporate as part of the data context to be generated.
    /// </param>
    /// <returns>
    /// A <see cref="TypeBuilder{T}"/> with which a new data context can be
    /// instantiated.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if any element in the collection
    /// <paramref name="models"/> does not inherit from the base type
    /// <see cref="Model"/>.
    /// </exception>
    public static ITypeBuilder<DbContext> Build(Type[] models) => Build(models, null);

    /// <summary>
    /// Builds a new data context using the specified models.
    /// </summary>
    /// <param name="name">Name of the new data context.</param>
    /// <param name="models">
    /// Data models to incorporate as part of the data context to be generated.
    /// </param>
    /// <returns>
    /// A <see cref="TypeBuilder{T}"/> with which a new data context can be
    /// instantiated.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="name"/> is an empty string or
    /// <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if any element in the collection
    /// <paramref name="models"/> does not inherit from the base type
    /// <see cref="Model"/>.
    /// </exception>
    public static ITypeBuilder<DbContext> Build(string name, Type[] models) => Build(name, models, null);

    /// <summary>
    /// Builds a new data context, discovering all available data models in the
    /// application.
    /// </summary>
    /// <returns>
    /// A <see cref="TypeBuilder{T}"/> with which a new data context can be
    /// instantiated.
    /// </returns>
    public static ITypeBuilder<DbContext> Build() => Build((Action<DbContextOptionsBuilder>?)null);

    /// <summary>
    /// Builds a new data context, discovering all available data models in the
    /// application.
    /// </summary>
    /// <param name="name">Name of the new data context.</param>
    /// <returns>
    /// A <see cref="TypeBuilder{T}"/> with which a new data context can be
    /// instantiated.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="name"/> is an empty string or
    /// <see langword="null"/>.
    /// </exception>
    public static ITypeBuilder<DbContext> Build(string name) => Build(name, (Action<DbContextOptionsBuilder>?)null);

    /// <summary>
    /// Builds a new data context, discovering all available data models in the
    /// application.
    /// </summary>
    /// <param name="configurationCallback">
    /// The method to invoke for externally configuring the generated data
    /// context.
    /// </param>
    /// <returns>
    /// A <see cref="TypeBuilder{T}"/> with which a new data context can be
    /// instantiated.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the specified method in 
    /// <paramref name="configurationCallback"/> is not static.
    /// </exception>
    public static ITypeBuilder<DbContext> Build(Action<DbContextOptionsBuilder>? configurationCallback)
    {
        return Build(ReflectionHelpers.GetTypes<Model>().Where(p => !p.IsAbstract).ToArray(), configurationCallback);
    }

    /// <summary>
    /// Builds a new data context, discovering all available data models in the
    /// application.
    /// </summary>
    /// <param name="name">Name of the new data context.</param>
    /// <param name="configurationCallback">
    /// The method to invoke for externally configuring the generated data
    /// context.
    /// </param>
    /// <returns>
    /// A <see cref="TypeBuilder{T}"/> with which a new data context can be
    /// instantiated.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="name"/> is an empty string or null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the method specified in
    /// <paramref name="configurationCallback"/> is not static.
    /// </exception>
    public static ITypeBuilder<DbContext> Build(string name, Action<DbContextOptionsBuilder>? configurationCallback)
    {
        return Build(name, ReflectionHelpers.GetTypes<Model>().Where(p => !p.IsAbstract).ToArray(), configurationCallback);
    }

    /// <summary>
    /// Gets a <see cref="ITransactionFactory"/> connected to an Entity
    /// Framework Core data context generated dynamically.
    /// </summary>
    /// <param name="type">
    /// The type builder used to generate an Entity Framework Core data
    /// context.
    /// </param>
    /// <returns>
    /// A <see cref="ITransactionFactory"/> connected to an Entity Framework
    /// Core data context generated dynamically.
    /// </returns>
    public static ITransactionFactory GetEfTransFactory(this ITypeBuilder<DbContext> type)
    {
        return typeof(EfCoreTransFactory<>).MakeGenericType(type.Builder.CreateType()!).New<ITransactionFactory>();
    }
}
