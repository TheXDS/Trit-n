using Microsoft.EntityFrameworkCore;
using TheXDS.MCART.Helpers;
using TheXDS.Triton.EfContextBuilder;
using TheXDS.Triton.Models.Base;

namespace TheXDS.ServicePool.Triton.EfContextBuilder;

/// <summary>
/// Contains extension methods that allow configuring Tritón for use with 
/// the <see cref="ServicePool"/>.
/// </summary>
public static class ServicePoolEfContextBuilderExtensions
{
    /// <summary>
    /// Adds a dynamically generated data service to the collection of hosted
    /// services within a <see cref="Pool"/>, automatically discovering the
    /// models to include in the context.
    /// </summary>
    /// <param name="configurable">
    /// An instance of Tritón configuration to use for registering a new
    /// dynamic context.
    /// </param>
    /// <param name="optionsCallback">
    /// A configuration callback to use when the dynamic context requests
    /// configuration. Can be omitted or set to <see langword="null"/> if not
    /// required to invalidate the method
    /// <see cref="DbContext.OnConfiguring(DbContextOptionsBuilder)"/>.
    /// </param>
    /// <returns>
    /// The same instance of the object used for configuring Tritón.
    /// </returns>
    public static ITritonConfigurable UseDynamicContext(this ITritonConfigurable configurable, Action<DbContextOptionsBuilder>? optionsCallback = null)
    {
        var t = ContextBuilder.Build(optionsCallback);
        configurable.UseContext(t.Builder.CreateType()!);
        return configurable;
    }

    /// <summary>
    /// Adds a dynamically generated data service to the collection of hosted
    /// services within a <see cref="Pool"/>, explicitly specifying the models
    /// to include in the context.
    /// </summary>
    /// <param name="configurable">
    /// An instance of Tritón configuration to use for registering a new
    /// dynamic context.
    /// </param>
    /// <param name="models">
    /// An array of types to include in the dynamically generated context.
    /// </param>
    /// <param name="optionsCallback">
    /// A configuration callback to use when the dynamic context requests
    /// configuration. Can be omitted or set to
    /// <see langword="null"/> if not required to invalidate the method
    /// <see cref="DbContext.OnConfiguring(DbContextOptionsBuilder)"/>.
    /// </param>
    /// <returns>
    /// The same instance of the object used for configuring Tritón.
    /// </returns>
    public static ITritonConfigurable UseDynamicContext(this ITritonConfigurable configurable, Type[] models, Action<DbContextOptionsBuilder>? optionsCallback = null)
    {
        var t = ContextBuilder.Build(models, optionsCallback);
        configurable.UseContext(t.Builder.CreateType()!);
        return configurable;
    }

    /// <summary>
    /// Adds a dynamically generated data service to the collection of hosted
    /// services within a <see cref="Pool"/>, filtering the models to include
    /// in the context.
    /// </summary>
    /// <param name="configurable">
    /// An instance of Tritón configuration to use for registering a new 
    /// dynamic context.
    /// </param>
    /// <param name="modelFilter">
    /// A function to filter which data models to include in the dynamically 
    /// generated context.
    /// </param>
    /// <param name="optionsCallback">
    /// A configuration callback to use when the dynamic context requests 
    /// configuration. Can be omitted or set to <see langword="null"/> if not 
    /// required to invalidate the method
    /// <see cref="DbContext.OnConfiguring(DbContextOptionsBuilder)"/>.
    /// </param>
    /// <returns>
    /// The same instance of the object used for configuring Tritón.
    /// </returns>
    public static ITritonConfigurable UseDynamicContext(this ITritonConfigurable configurable, Func<Type, bool> modelFilter, Action<DbContextOptionsBuilder>? optionsCallback = null)
    {
        return UseDynamicContext(configurable, ReflectionHelpers.PublicTypes<Model>().Where(modelFilter).ToArray(), optionsCallback);
    }
}