using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TheXDS.MCART.Exceptions;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Extensions;
using TheXDS.ServicePool.Triton.Resources;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;

namespace TheXDS.ServicePool.Triton;

/// <summary>
/// Incluye métodos de extensión para configurar un <see cref="ServicePool"/>
/// con servicios de Tritón conectados a un contexto de datos de Entity
/// Framework.
/// </summary>
public static class TritonConfigurableExtensions
{
    /// <summary>
    /// Agrega un <see cref="DbContext"/> a la colección de servicios
    /// hosteados dentro de un
    /// <see cref="ServicePool"/>, envolviendolo en un 
    /// <see cref="TritonService"/>.
    /// </summary>
    /// <param name="configurable">Instancia a configurar.</param>
    /// <param name="contextType">Tipo de contexto a registrar.</param>
    /// <param name="options">
    /// Instancia de opciones de configuración de contexto a utilizar par
    /// a configurar el contexto subyacente.
    /// </param>
    /// <param name="configurator">
    /// Configuración de Middleware a utilizar al generar transacciones.
    /// </param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    public static ITritonConfigurable UseContext(this ITritonConfigurable configurable, Type contextType, DbContextOptions options, IMiddlewareConfigurator? configurator = null)
    {
        return configurable.UseService(contextType, (m, f) => new TritonService(m, f), options, configurator);
    }

    /// <summary>
    /// Agrega un <see cref="DbContext"/> a la colección de servicios
    /// hosteados dentro de un
    /// <see cref="ServicePool"/>, envolviendolo en un 
    /// <see cref="TritonService"/>.
    /// </summary>
    /// <param name="configurable">Instancia a configurar.</param>
    /// <param name="contextType">Tipo de contexto a registrar.</param>
    /// <param name="builder">
    /// Método de configuración a llamar para configurar el contexto
    /// subyacente.
    /// </param>
    /// <param name="configurator">
    /// Configuración de Middleware a utilizar al generar transacciones.
    /// </param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    public static ITritonConfigurable UseContext(this ITritonConfigurable configurable, Type contextType, Action<DbContextOptionsBuilder> builder, IMiddlewareConfigurator? configurator = null)
    {
        return configurable.UseService(contextType, (m, f) => new TritonService(m, f), builder, configurator);
    }

    /// <summary>
    /// Agrega un <see cref="DbContext"/> a la colección de servicios
    /// hosteados dentro de un
    /// <see cref="ServicePool"/>, envolviendolo en un 
    /// <see cref="TritonService"/>.
    /// </summary>
    /// <param name="configurable">Instancia a configurar.</param>
    /// <param name="contextType">Tipo de contexto a registrar.</param>
    /// <param name="configurator">
    /// Configuración de Middleware a utilizar al generar transacciones.
    /// </param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    public static ITritonConfigurable UseContext(this ITritonConfigurable configurable, Type contextType, IMiddlewareConfigurator? configurator = null)
    {
        return configurable.UseService(contextType, (m, f) => new TritonService(m, f), DbContextOptionsSource.None, configurator);
    }

    /// <summary>
    /// Agrega un <see cref="DbContext"/> a la colección de servicios
    /// hosteados dentro de un
    /// <see cref="ServicePool"/>, envolviendolo en un 
    /// <see cref="TritonService"/>.
    /// </summary>
    /// <param name="configurable">Instancia a configurar.</param>
    /// <typeparam name="T">Tipo de contexto a registrar.</typeparam>
    /// <param name="configurator">
    /// Configuración de Middleware a utilizar al generar transacciones.
    /// </param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    public static ITritonConfigurable UseContext<T>(this ITritonConfigurable configurable, IMiddlewareConfigurator? configurator = null) where T : DbContext
    {
        return configurable.UseService<TritonService, T>((m, f) => new TritonService(m, f), configurator);
    }

    /// <summary>
    /// Agrega un <see cref="DbContext"/> a la colección de servicios
    /// hosteados dentro de un
    /// <see cref="ServicePool"/>, envolviendolo en un 
    /// <see cref="TritonService"/>.
    /// </summary>
    /// <typeparam name="T">Tipo de contexto a registrar.</typeparam>
    /// <param name="configurable">Instancia a configurar.</param>
    /// <param name="options">
    /// Instancia de opciones de configuración de contexto a utilizar par
    /// a configurar el contexto subyacente.
    /// </param>
    /// <param name="configurator">
    /// Configuración de Middleware a utilizar al generar transacciones.
    /// </param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    public static ITritonConfigurable UseContext<T>(this ITritonConfigurable configurable, DbContextOptions<T> options, IMiddlewareConfigurator? configurator = null) where T : DbContext
    {
        return configurable.UseService((m, f) => new TritonService(m, f), options, configurator);
    }

    /// <summary>
    /// Agrega un <see cref="DbContext"/> a la colección de servicios
    /// hosteados dentro de un
    /// <see cref="ServicePool"/>, envolviendolo en un 
    /// <see cref="TritonService"/>.
    /// </summary>
    /// <typeparam name="T">Tipo de contexto a registrar.</typeparam>
    /// <param name="configurable">Instancia a configurar.</param>
    /// <param name="builder">
    /// Método de configuración a llamar para configurar el contexto
    /// subyacente.
    /// </param>
    /// <param name="configurator">
    /// Configuración de Middleware a utilizar al generar transacciones.
    /// </param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    public static ITritonConfigurable UseContext<T>(this ITritonConfigurable configurable, Action<DbContextOptionsBuilder<T>> builder, IMiddlewareConfigurator? configurator = null) where T : DbContext
    {
        return configurable.UseService((m, f) => new TritonService(m, f), builder, configurator);
    }

    /// <summary>
    /// Agrega un <see cref="DbContext"/> a la colección de servicios
    /// hosteados dentro de un
    /// <see cref="ServicePool"/>, envolviendolo en un 
    /// <see cref="TritonService"/>.
    /// </summary>
    /// <typeparam name="T">Tipo de contexto a registrar.</typeparam>
    /// <param name="configurable">Instancia a configurar.</param>
    /// <param name="optionsSource">
    /// Objeto a utilizar para obtener la configuración de contexto a utilizar
    /// al generar transacciones.
    /// </param>
    /// <param name="configurator">
    /// Configuración de Middleware a utilizar al generar transacciones.
    /// </param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    public static ITritonConfigurable UseContext<T>(this ITritonConfigurable configurable, DbContextOptionsSource<T>? optionsSource, IMiddlewareConfigurator? configurator = null) where T : DbContext
    {
        return configurable.UseService((m, f) => new TritonService(m, f), optionsSource, configurator);
    }

    /// <summary>
    /// Agrega un <see cref="DbContext"/> a la colección de servicios
    /// hosteados dentro de un
    /// <see cref="ServicePool"/>, envolviendolo en un 
    /// <see cref="TritonService"/>.
    /// </summary>
    /// <param name="configurable">Instancia a configurar.</param>
    /// <param name="contextType">Tipo de contexto a registrar.</param>
    /// <param name="optionsSource">
    /// Objeto a utilizar para obtener la configuración de contexto a utilizar
    /// al generar transacciones.
    /// </param>
    /// <param name="configurator">
    /// Configuración de Middleware a utilizar al generar transacciones.
    /// </param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    public static ITritonConfigurable UseContext(this ITritonConfigurable configurable, Type contextType, IDbContextOptionsSource? optionsSource, IMiddlewareConfigurator? configurator = null)
    {
        return configurable.UseService(contextType, (m, f) => new TritonService(m, f), optionsSource, configurator);
    }

    /// <summary>
    /// Registra un servicio para acceder a datos.
    /// </summary>
    /// <typeparam name="TService">Tipo de servicio a registrar.</typeparam>
    /// <typeparam name="TContext">Tipo de contexto de datos a registrar.</typeparam>
    /// <param name="configurable">Instancia a configurar.</param>
    /// <param name="factoryCallback">Método a llamar para crear el servicio.</param>
    /// <param name="configurator">
    /// Configuración de Middleware a utilizar al generar transacciones.
    /// </param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    public static ITritonConfigurable UseService<TService, TContext>(this ITritonConfigurable configurable, Func<IMiddlewareConfigurator, EfCoreTransFactory<TContext>, TService> factoryCallback, IMiddlewareConfigurator? configurator = null)
        where TService : ITritonService where TContext : DbContext
    {
        return configurable.UseService(factoryCallback, (DbContextOptionsSource<TContext>?)null, configurator);
    }

    /// <summary>
    /// Registra un servicio para acceder a datos.
    /// </summary>
    /// <typeparam name="TService">Tipo de servicio a registrar.</typeparam>
    /// <typeparam name="TContext">Tipo de contexto de datos a registrar.</typeparam>
    /// <param name="configurable">Instancia a configurar.</param>
    /// <param name="factoryCallback">Método a llamar para crear el servicio.</param>
    /// <param name="options">
    /// Instancia de opciones de configuración de contexto a utilizar par
    /// a configurar el contexto subyacente.
    /// </param>
    /// <param name="configurator">
    /// Configuración de Middleware a utilizar al generar transacciones.
    /// </param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    public static ITritonConfigurable UseService<TService, TContext>(this ITritonConfigurable configurable, Func<IMiddlewareConfigurator, EfCoreTransFactory<TContext>, TService> factoryCallback, DbContextOptions<TContext> options, IMiddlewareConfigurator? configurator = null)
        where TService : ITritonService where TContext : DbContext
    {
        return configurable.UseService(factoryCallback, new DbContextOptionsSource<TContext>(options), configurator);
    }

    /// <summary>
    /// Registra un servicio para acceder a datos.
    /// </summary>
    /// <typeparam name="TService">Tipo de servicio a registrar.</typeparam>
    /// <typeparam name="TContext">Tipo de contexto de datos a registrar.</typeparam>
    /// <param name="configurable">Instancia a configurar.</param>
    /// <param name="factoryCallback">Método a llamar para crear el servicio.</param>
    /// <param name="configCallback">
    /// Método de configuración a llamar para configurar el contexto
    /// subyacente.
    /// </param>
    /// <param name="configurator">
    /// Configuración de Middleware a utilizar al generar transacciones.
    /// </param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    public static ITritonConfigurable UseService<TService, TContext>(this ITritonConfigurable configurable, Func<IMiddlewareConfigurator, EfCoreTransFactory<TContext>, TService> factoryCallback, Action<DbContextOptionsBuilder<TContext>> configCallback, IMiddlewareConfigurator? configurator = null)
        where TService : ITritonService where TContext : DbContext
    {
        return configurable.UseService(factoryCallback, new DbContextOptionsSource<TContext>(configCallback), configurator);
    }

    /// <summary>
    /// Registra un servicio para acceder a datos.
    /// </summary>
    /// <typeparam name="TService">Tipo de servicio a registrar.</typeparam>
    /// <typeparam name="TContext">Tipo de contexto de datos a registrar.</typeparam>
    /// <param name="configurable">Instancia a configurar.</param>
    /// <param name="factoryCallback">Método a llamar para crear el servicio.</param>
    /// <param name="optionsSource">
    /// Objeto a utilizar para obtener la configuración de contexto a utilizar
    /// al generar transacciones.
    /// </param>
    /// <param name="configurator">
    /// Configuración de Middleware a utilizar al generar transacciones.
    /// </param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    public static ITritonConfigurable UseService<TService, TContext>(this ITritonConfigurable configurable, Func<IMiddlewareConfigurator, EfCoreTransFactory<TContext>, TService> factoryCallback, DbContextOptionsSource<TContext>? optionsSource, IMiddlewareConfigurator? configurator = null)
        where TService : ITritonService
        where TContext : DbContext
    {
        configurable.Pool.Register(() => factoryCallback.Invoke(configurator ?? configurable.Pool.Resolve<IMiddlewareConfigurator>() ?? new TransactionConfiguration(), new EfCoreTransFactory<TContext>(optionsSource ?? DbContextOptionsSource.None)));
        return configurable;
    }

    /// <summary>
    /// Registra un servicio para acceder a datos.
    /// </summary>
    /// <param name="configurable">Instancia a configurar.</param>
    /// <param name="contextType">Tipo de contexto de datos a registrar.</param>
    /// <param name="factoryCallback">Método a llamar para crear el servicio.</param>
    /// <param name="options">
    /// Instancia de opciones de configuración de contexto a utilizar par
    /// a configurar el contexto subyacente.
    /// </param>
    /// <param name="configurator">
    /// Configuración de Middleware a utilizar al generar transacciones.
    /// </param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    public static ITritonConfigurable UseService(this ITritonConfigurable configurable, Type contextType, Func<IMiddlewareConfigurator, ITransactionFactory, ITritonService> factoryCallback, DbContextOptions options, IMiddlewareConfigurator? configurator = null)
    {
        return configurable.UseService(contextType, factoryCallback, new DbContextOptionsSource(options), configurator);
    }

    /// <summary>
    /// Registra un servicio para acceder a datos.
    /// </summary>
    /// <param name="configurable">Instancia a configurar.</param>
    /// <param name="contextType">Tipo de contexto de datos a registrar.</param>
    /// <param name="factoryCallback">Método a llamar para crear el servicio.</param>
    /// <param name="configCallback">
    /// Método de configuración a llamar para configurar el contexto
    /// subyacente.
    /// </param>
    /// <param name="configurator">
    /// Configuración de Middleware a utilizar al generar transacciones.
    /// </param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    public static ITritonConfigurable UseService(this ITritonConfigurable configurable, Type contextType, Func<IMiddlewareConfigurator, ITransactionFactory, ITritonService> factoryCallback, Action<DbContextOptionsBuilder> configCallback, IMiddlewareConfigurator? configurator = null)
    {
        return configurable.UseService(contextType, factoryCallback, new DbContextOptionsSource(configCallback), configurator);
    }

    /// <summary>
    /// Registra un servicio para acceder a datos.
    /// </summary>
    /// <param name="configurable">Instancia a configurar.</param>
    /// <param name="contextType">Tipo de contexto de datos a registrar.</param>
    /// <param name="factoryCallback">Método a llamar para crear el servicio.</param>
    /// <param name="optionsSource">
    /// Objeto a utilizar para obtener la configuración de contexto a utilizar
    /// al generar transacciones.
    /// </param>
    /// <param name="configurator">
    /// Configuración de Middleware a utilizar al generar transacciones.
    /// </param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    public static ITritonConfigurable UseService(this ITritonConfigurable configurable, Type contextType, Func<IMiddlewareConfigurator, ITransactionFactory, ITritonService> factoryCallback, IDbContextOptionsSource? optionsSource, IMiddlewareConfigurator? configurator = null)
    {
        CheckContextType(contextType, optionsSource);
        return configurable.UseService(factoryCallback, () => CreateEfFactory(contextType, optionsSource), configurator);
    }

    /// <summary>
    /// Descubre automáticamente todos los servicios y contextos de datos a
    /// exponer por medio de <see cref="ServicePool"/>.
    /// </summary>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    public static ITritonConfigurable DiscoverContexts(this ITritonConfigurable configurable)
    {
        foreach (var j in ReflectionHelpers.GetTypes<DbContext>(true).Where(p => p.GetConstructor(Type.EmptyTypes) is not null))
        {
            configurable.UseContext(j);
        }
        return configurable;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerNonUserCode]
    private static void CheckContextType(Type contextType, IDbContextOptionsSource? optionsSource)
    {
        if (!contextType.Implements<DbContext>())
        {
            throw Errors.TypeMustImplDbContext(nameof(contextType));
        }
        var options  = (optionsSource ?? DbContextOptionsSource.None).GetOptions();
        if ((options is null && !contextType.IsInstantiable([])) || (options is not null && !contextType.IsInstantiable([typeof(DbContextOptions)])))
        {
            throw new ClassNotInstantiableException();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerNonUserCode]
    private static ITransactionFactory CreateEfFactory(Type contextType, IDbContextOptionsSource? optionsSource)
    {
        CheckContextType(contextType, optionsSource);
        return typeof(EfCoreTransFactory<>).MakeGenericType(contextType).New<ITransactionFactory>(optionsSource ?? DbContextOptionsSource.None);
    }
}