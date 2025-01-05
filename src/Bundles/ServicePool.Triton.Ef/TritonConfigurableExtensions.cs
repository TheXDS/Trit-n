using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TheXDS.MCART.Exceptions;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Extensions;
using TheXDS.ServicePool.Triton.Resources;
using TheXDS.Triton.Services;

namespace TheXDS.ServicePool.Triton;

/// <summary>
/// Contains extension methods for configuring a <see cref="Pool"/> with 
/// services of Tritón connected to an Entity Framework data context.
/// </summary>
public static class TritonConfigurableExtensions
{
    /// <summary>
    /// Adds a <see cref="DbContext"/> to the collection of services hosted within 
    /// a <see cref="Pool"/>, wrapping it in a <see cref="TritonService"/>.
    /// </summary>
    /// <param name="configurable">The instance to configure.</param>
    /// <param name="contextType">The type of context to register.</param>
    /// <param name="options">
    /// An instance of DbContextOptions to use when configuring the underlying context.
    /// </param>
    /// <param name="configurator">
    /// The middleware configuration to use when generating transactions.
    /// </param>
    /// <returns>
    /// The same instance of the object used to configure Tritón.
    /// </returns>
    public static ITritonConfigurable UseContext(this ITritonConfigurable configurable, Type contextType, DbContextOptions options, IMiddlewareConfigurator? configurator = null)
    {
        return configurable.UseService(contextType, (m, f) => new TritonService(m, f), options, configurator);
    }

    /// <summary>
    /// Adds a <see cref="DbContext"/> to the collection of services hosted within 
    /// a <see cref="Pool"/>, wrapping it in a <see cref="TritonService"/>.
    /// </summary>
    /// <param name="configurable">The instance to configure.</param>
    /// <param name="contextType">The type of context to register.</param>
    /// <param name="builder">
    /// A method to call when configuring the underlying context.
    /// </param>
    /// <param name="configurator">
    /// The middleware configuration to use when generating transactions.
    /// </param>
    /// <returns>
    /// The same instance of the object used to configure Tritón.
    /// </returns>
    public static ITritonConfigurable UseContext(this ITritonConfigurable configurable, Type contextType, Action<DbContextOptionsBuilder> builder, IMiddlewareConfigurator? configurator = null)
    {
        return configurable.UseService(contextType, (m, f) => new TritonService(m, f), builder, configurator);
    }

    /// <summary>
    /// Adds a <see cref="DbContext"/> to the collection of services hosted 
    /// within a <see cref="Pool"/>, wrapping it in a
    /// <see cref="TritonService"/>.
    /// </summary>
    /// <param name="configurable">The instance to configure.</param>
    /// <param name="contextType">The type of context to register.</param>
    /// <param name="configurator">
    /// Middleware configuration to use when generating transactions.
    /// </param>
    /// <returns>
    /// The same instance of the object used to configure Tritón.
    /// </returns>
    public static ITritonConfigurable UseContext(this ITritonConfigurable configurable, Type contextType, IMiddlewareConfigurator? configurator = null)
    {
        return configurable.UseService(contextType, (m, f) => new TritonService(m, f), DbContextOptionsSource.None, configurator);
    }

    /// <summary>
    /// Adds a <see cref="DbContext"/> to the collection of services hosted
    /// within a <see cref="Pool"/>, wrapping it in a
    /// <see cref="TritonService"/>.
    /// </summary>
    /// <param name="configurable">The instance to configure.</param>
    /// <typeparam name="T">The type of context to register.</typeparam>
    /// <param name="configurator">
    /// Middleware configuration to use when generating transactions.
    /// </param>
    /// <returns>
    /// The same instance of the object used to configure Tritón.
    /// </returns>
    public static ITritonConfigurable UseContext<T>(this ITritonConfigurable configurable, IMiddlewareConfigurator? configurator = null) where T : DbContext
    {
        return configurable.UseService<TritonService, T>((m, f) => new TritonService(m, f), configurator);
    }

    /// <summary>
    /// Adds a <see cref="DbContext"/> to the collection of services hosted
    /// within  a <see cref="Pool"/>, wrapping it in a
    /// <see cref="TritonService"/>.
    /// </summary>
    /// <typeparam name="T">The type of context to register.</typeparam>
    /// <param name="configurable">The instance to configure.</param>
    /// <param name="options">
    /// An instance of DbContextOptions to use when configuring the underlying
    /// context.
    /// </param>
    /// <param name="configurator">
    /// Middleware configuration to use when generating transactions.
    /// </param>
    /// <returns>
    /// The same instance of the object used to configure Tritón.
    /// </returns>
    public static ITritonConfigurable UseContext<T>(this ITritonConfigurable configurable, DbContextOptions<T> options, IMiddlewareConfigurator? configurator = null) where T : DbContext
    {
        return configurable.UseService((m, f) => new TritonService(m, f), options, configurator);
    }

    /// <summary>
    /// Adds a <see cref="DbContext"/> to the collection of services hosted
    /// within a <see cref="Pool"/>, wrapping it in a 
    /// <see cref="TritonService"/>.
    /// </summary>
    /// <typeparam name="T">The type of context to register.</typeparam>
    /// <param name="configurable">The instance to configure.</param>
    /// <param name="builder">
    /// A method to call when configuring the underlying context.
    /// </param>
    /// <param name="configurator">
    /// Middleware configuration to use when generating transactions.
    /// </param>
    /// <returns>
    /// The same instance of the object used to configure Tritón.
    /// </returns>
    public static ITritonConfigurable UseContext<T>(this ITritonConfigurable configurable, Action<DbContextOptionsBuilder<T>> builder, IMiddlewareConfigurator? configurator = null) where T : DbContext
    {
        return configurable.UseService((m, f) => new TritonService(m, f), builder, configurator);
    }

    /// <summary>
    /// Adds a <see cref="DbContext"/> to the collection of services hosted
    /// within a <see cref="Pool"/>, wrapping it in a
    /// <see cref="TritonService"/>.
    /// </summary>
    /// <typeparam name="T">The type of context to register.</typeparam>
    /// <param name="configurable">The instance to configure.</param>
    /// <param name="optionsSource">
    /// An object to use to obtain the context configuration to use when
    /// generating transactions.
    /// </param>
    /// <param name="configurator">
    /// Middleware configuration to use when generating transactions.
    /// </param>
    /// <returns>
    /// The same instance of the object used to configure Tritón.
    /// </returns>
    public static ITritonConfigurable UseContext<T>(this ITritonConfigurable configurable, DbContextOptionsSource<T>? optionsSource, IMiddlewareConfigurator? configurator = null) where T : DbContext
    {
        return configurable.UseService((m, f) => new TritonService(m, f), optionsSource, configurator);
    }

    /// <summary>
    /// Adds a <see cref="DbContext"/> to the collection of services hosted
    /// within a <see cref="Pool"/>, wrapping it in a
    /// <see cref="TritonService"/>.
    /// </summary>
    /// <param name="configurable">The instance to configure.</param>
    /// <param name="contextType">The type of context to register.</param>
    /// <param name="optionsSource">
    /// An object to use to obtain the context configuration to use when
    /// generating transactions.
    /// </param>
    /// <param name="configurator">
    /// Middleware configuration to use when generating transactions.
    /// </param>
    /// <returns>
    /// The same instance of the object used to configure Tritón.
    /// </returns>
    public static ITritonConfigurable UseContext(this ITritonConfigurable configurable, Type contextType, IDbContextOptionsSource? optionsSource, IMiddlewareConfigurator? configurator = null)
    {
        return configurable.UseService(contextType, (m, f) => new TritonService(m, f), optionsSource, configurator);
    }

    /// <summary>
    /// Registers a service to access data.
    /// </summary>
    /// <typeparam name="TService">The type of service to register.</typeparam>
    /// <typeparam name="TContext">The type of context to register.</typeparam>
    /// <param name="configurable">The instance to configure.</param>
    /// <param name="factoryCallback">
    /// A method to call to create the service.
    /// </param>
    /// <param name="configurator">
    /// Middleware configuration to use when generating transactions.
    /// </param>
    /// <returns>
    /// The same instance of the object used to configure Tritón.
    /// </returns>
    public static ITritonConfigurable UseService<TService, TContext>(this ITritonConfigurable configurable, Func<IMiddlewareConfigurator, EfCoreTransFactory<TContext>, TService> factoryCallback, IMiddlewareConfigurator? configurator = null)
        where TService : ITritonService
        where TContext : DbContext
    {
        return configurable.UseService(factoryCallback, (DbContextOptionsSource<TContext>?)null, configurator);
    }

    /// <summary>
    /// Registers a service to access data.
    /// </summary>
    /// <typeparam name="TService">The type of service to register.</typeparam>
    /// <typeparam name="TContext">The type of context to register.</typeparam>
    /// <param name="configurable">The instance to configure.</param>
    /// <param name="factoryCallback">
    /// A method to call to create the service.
    /// </param>
    /// <param name="options">
    /// An instance of context options configuration to use to set up the
    /// underlying context.
    /// </param>
    /// <param name="configurator">
    /// Middleware configuration to use when generating transactions.
    /// </param>
    /// <returns>
    /// The same instance of the object used to configure Tritón.
    /// </returns>
    public static ITritonConfigurable UseService<TService, TContext>(this ITritonConfigurable configurable, Func<IMiddlewareConfigurator, EfCoreTransFactory<TContext>, TService> factoryCallback, DbContextOptions<TContext> options, IMiddlewareConfigurator? configurator = null)
        where TService : ITritonService
        where TContext : DbContext
    {
        return configurable.UseService(factoryCallback, new DbContextOptionsSource<TContext>(options), configurator);
    }

    /// <summary>
    /// Registers a service to access data.
    /// </summary>
    /// <typeparam name="TService">The type of service to register.</typeparam>
    /// <typeparam name="TContext">The type of context to register.</typeparam>
    /// <param name="configurable">The instance to configure.</param>
    /// <param name="factoryCallback">
    /// A method to call to create the service.
    /// </param>
    /// <param name="configCallback">
    /// A configuration method to call to configure the underlying context.
    /// </param>
    /// <param name="configurator">
    /// Middleware configuration to use when generating transactions.
    /// </param>
    /// <returns>
    /// The same instance of the object used to configure Tritón.
    /// </returns>
    public static ITritonConfigurable UseService<TService, TContext>(this ITritonConfigurable configurable, Func<IMiddlewareConfigurator, EfCoreTransFactory<TContext>, TService> factoryCallback, Action<DbContextOptionsBuilder<TContext>> configCallback, IMiddlewareConfigurator? configurator = null)
        where TService : ITritonService
        where TContext : DbContext
    {
        return configurable.UseService(factoryCallback, new DbContextOptionsSource<TContext>(configCallback), configurator);
    }

    /// <summary>
    /// Registers a service to access data.
    /// </summary>
    /// <typeparam name="TService">The type of service to register.</typeparam>
    /// <typeparam name="TContext">The type of context to register.</typeparam>
    /// <param name="configurable">The instance to configure.</param>
    /// <param name="factoryCallback">
    /// A method to call to create the service.
    /// </param>
    /// <param name="optionsSource">
    /// An object to use to obtain the context configuration to use when generating transactions.
    /// </param>
    /// <param name="configurator">
    /// Middleware configuration to use when generating transactions.
    /// </param>
    /// <returns>
    /// The same instance of the object used to configure Tritón.
    /// </returns>
    public static ITritonConfigurable UseService<TService, TContext>(this ITritonConfigurable configurable, Func<IMiddlewareConfigurator, EfCoreTransFactory<TContext>, TService> factoryCallback, DbContextOptionsSource<TContext>? optionsSource, IMiddlewareConfigurator? configurator = null)
        where TService : ITritonService
        where TContext : DbContext
    {
        configurable.Pool.Register(() => factoryCallback.Invoke(configurator ?? configurable.Pool.Resolve<IMiddlewareConfigurator>() ?? new TransactionConfiguration(), new EfCoreTransFactory<TContext>(optionsSource ?? DbContextOptionsSource.None)));
        return configurable;
    }

    /// <summary>
    /// Registers a service to access data.
    /// </summary>
    /// <param name="configurable">The instance to configure.</param>
    /// <param name="contextType">The type of context to register.</param>
    /// <param name="factoryCallback">
    /// A method to call to create the service.
    /// </param>
    /// <param name="options">
    /// An instance of DbContextOptions to use to configure the underlying
    /// context.
    /// </param>
    /// <param name="configurator">
    /// Middleware configuration to use when generating transactions.
    /// </param>
    /// <returns>
    /// The same instance of the object used to configure Tritón.
    /// </returns>
    public static ITritonConfigurable UseService(this ITritonConfigurable configurable, Type contextType, Func<IMiddlewareConfigurator, ITransactionFactory, ITritonService> factoryCallback, DbContextOptions options, IMiddlewareConfigurator? configurator = null)
    {
        return configurable.UseService(contextType, factoryCallback, new DbContextOptionsSource(options), configurator);
    }

    /// <summary>
    /// Registers a service to access data.
    /// </summary>
    /// <param name="configurable">The instance to configure.</param>
    /// <param name="contextType">The type of context to register.</param>
    /// <param name="factoryCallback">
    /// A method to call to create the service.
    /// </param>
    /// <param name="configCallback">
    /// A method to use to configure the underlying context.
    /// </param>
    /// <param name="configurator">
    /// Middleware configuration to use when generating transactions.
    /// </param>
    /// <returns>
    /// The same instance of the object used to configure Tritón.
    /// </returns>
    public static ITritonConfigurable UseService(this ITritonConfigurable configurable, Type contextType, Func<IMiddlewareConfigurator, ITransactionFactory, ITritonService> factoryCallback, Action<DbContextOptionsBuilder> configCallback, IMiddlewareConfigurator? configurator = null)
    {
        return configurable.UseService(contextType, factoryCallback, new DbContextOptionsSource(configCallback), configurator);
    }

    /// <summary>
    /// Registers a service to access data.
    /// </summary>
    /// <param name="configurable">The instance to configure.</param>
    /// <param name="contextType">The type of context to register.</param>
    /// <param name="factoryCallback">
    /// A method to call to create the service.
    /// </param>
    /// <param name="optionsSource">
    /// An object to use to get the context configuration to use when
    /// generating transactions.
    /// </param>
    /// <param name="configurator">
    /// Middleware configuration to use when generating transactions.
    /// </param>
    /// <returns>
    /// The same instance of the object used to configure Tritón.
    /// </returns>
    public static ITritonConfigurable UseService(this ITritonConfigurable configurable, Type contextType, Func<IMiddlewareConfigurator, ITransactionFactory, ITritonService> factoryCallback, IDbContextOptionsSource? optionsSource, IMiddlewareConfigurator? configurator = null)
    {
        CheckContextType(contextType, optionsSource);
        return configurable.UseService(factoryCallback, () => CreateEfFactory(contextType, optionsSource), configurator);
    }

    /// <summary>
    /// Automatically discovers all services and data contexts to expose
    /// through <see cref="Pool"/>.
    /// </summary>
    /// <returns>
    /// The same instance of the object used to configure Tritón.
    /// </returns>
    public static ITritonConfigurable DiscoverContexts(this ITritonConfigurable configurable)
    {
        foreach (var type in ReflectionHelpers.GetTypes<DbContext>(true).Where(p => p.GetConstructor(Type.EmptyTypes) is not null))
        {
            configurable.UseContext(type);
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
        var options = (optionsSource ?? DbContextOptionsSource.None).GetOptions();
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