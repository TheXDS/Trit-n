using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Extensions;
using TheXDS.ServicePool.Extensions;
using TheXDS.ServicePool.Triton.Resources;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;

namespace TheXDS.ServicePool.Triton;

/// <summary>
/// Define una serie de miembros a implementar por un tipo que exponga
/// funciones de configuración para Tritón cuando se utiliza en conjunto
/// con un <see cref="ServicePool"/>.
/// </summary>
public interface ITritonConfigurable
{
    /// <summary>
    /// Obtiene una referencia al repositorio de servicios en el cual se ha
    /// registrado esta instancia.
    /// </summary>
    ServicePool Pool { get; }

    /// <summary>
    /// Agrega un <see cref="DbContext"/> a la colección de servicios
    /// hosteados dentro de un
    /// <see cref="ServicePool"/>, envolviendolo en un 
    /// <see cref="TritonService"/>.
    /// </summary>
    /// <param name="contextType">Tipo de contexto a registrar.</param>
    /// <param name="configurator">
    /// Configuración de Middleware a utilizar al generar transacciones.
    /// </param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    ITritonConfigurable UseContext(Type contextType, IMiddlewareConfigurator? configurator = null)
    {
        return UseService(contextType, (m, f) => new TritonService(m, f), configurator);
    }

    /// <summary>
    /// Agrega un <see cref="DbContext"/> a la colección de servicios
    /// hosteados dentro de un
    /// <see cref="ServicePool"/>, envolviendolo en un 
    /// <see cref="TritonService"/>.
    /// </summary>
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
    ITritonConfigurable UseContext(Type contextType, DbContextOptions options, IMiddlewareConfigurator? configurator = null)
    {
        return UseService(contextType, (m, f) => new TritonService(m, f), options, configurator);
    }

    /// <summary>
    /// Agrega un <see cref="DbContext"/> a la colección de servicios
    /// hosteados dentro de un
    /// <see cref="ServicePool"/>, envolviendolo en un 
    /// <see cref="TritonService"/>.
    /// </summary>
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
    ITritonConfigurable UseContext(Type contextType, Action<DbContextOptionsBuilder> builder, IMiddlewareConfigurator? configurator = null)
    {
        return UseService(contextType, (m, f) => new TritonService(m, f), builder, configurator);
    }

    /// <summary>
    /// Agrega un <see cref="DbContext"/> a la colección de servicios
    /// hosteados dentro de un
    /// <see cref="ServicePool"/>, envolviendolo en un 
    /// <see cref="TritonService"/>.
    /// </summary>
    /// <typeparam name="T">Tipo de contexto a registrar.</typeparam>
    /// <param name="configurator">
    /// Configuración de Middleware a utilizar al generar transacciones.
    /// </param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    ITritonConfigurable UseContext<T>(IMiddlewareConfigurator? configurator = null) where T : DbContext
    {
        return UseService<TritonService, T>((m, f) => new TritonService(m, f), configurator);
    }

    /// <summary>
    /// Agrega un <see cref="DbContext"/> a la colección de servicios
    /// hosteados dentro de un
    /// <see cref="ServicePool"/>, envolviendolo en un 
    /// <see cref="TritonService"/>.
    /// </summary>
    /// <typeparam name="T">Tipo de contexto a registrar.</typeparam>
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
    ITritonConfigurable UseContext<T>(DbContextOptions<T> options, IMiddlewareConfigurator? configurator = null) where T : DbContext
    {
        return UseService((m, f) => new TritonService(m, f), options, configurator);
    }

    /// <summary>
    /// Agrega un <see cref="DbContext"/> a la colección de servicios
    /// hosteados dentro de un
    /// <see cref="ServicePool"/>, envolviendolo en un 
    /// <see cref="TritonService"/>.
    /// </summary>
    /// <typeparam name="T">Tipo de contexto a registrar.</typeparam>
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
    ITritonConfigurable UseContext<T>(Action<DbContextOptionsBuilder<T>> builder, IMiddlewareConfigurator? configurator = null) where T : DbContext
    {
        return UseService((m, f) => new TritonService(m, f), builder, configurator);
    }

    /// <summary>
    /// Agrega un <see cref="DbContext"/> a la colección de servicios
    /// hosteados dentro de un
    /// <see cref="ServicePool"/>, envolviendolo en un 
    /// <see cref="TritonService"/>.
    /// </summary>
    /// <typeparam name="T">Tipo de contexto a registrar.</typeparam>
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
    ITritonConfigurable UseContext<T>(DbContextOptionsSource<T>? optionsSource, IMiddlewareConfigurator? configurator = null) where T : DbContext
    {
        return UseService((m, f) => new TritonService(m, f), optionsSource, configurator);
    }

    /// <summary>
    /// Agrega un <see cref="DbContext"/> a la colección de servicios
    /// hosteados dentro de un
    /// <see cref="ServicePool"/>, envolviendolo en un 
    /// <see cref="TritonService"/>.
    /// </summary>
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
    ITritonConfigurable UseContext(Type contextType, DbContextOptionsSource? optionsSource, IMiddlewareConfigurator? configurator = null)
    {
        return UseService(contextType, (m, f) => new TritonService(m, f), optionsSource, configurator);
    }

    /// <summary>
    /// Registra un servicio para acceder a datos.
    /// </summary>
    /// <typeparam name="TService">Tipo de servicio a registrar.</typeparam>
    /// <typeparam name="TContext">Tipo de contexto de datos a registrar.</typeparam>
    /// <param name="factoryCallback">Método a llamar para crear el servicio.</param>
    /// <param name="configurator">
    /// Configuración de Middleware a utilizar al generar transacciones.
    /// </param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    ITritonConfigurable UseService<TService, TContext>(Func<IMiddlewareConfigurator, EfCoreTransFactory<TContext>, TService> factoryCallback, IMiddlewareConfigurator? configurator = null)
        where TService : ITritonService where TContext : DbContext
    {
        return UseService(factoryCallback, (DbContextOptionsSource<TContext>?)null, configurator);
    }

    /// <summary>
    /// Registra un servicio para acceder a datos.
    /// </summary>
    /// <typeparam name="TService">Tipo de servicio a registrar.</typeparam>
    /// <typeparam name="TContext">Tipo de contexto de datos a registrar.</typeparam>
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
    ITritonConfigurable UseService<TService, TContext>(Func<IMiddlewareConfigurator, EfCoreTransFactory<TContext>, TService> factoryCallback, DbContextOptions<TContext> options, IMiddlewareConfigurator? configurator = null)
        where TService : ITritonService where TContext : DbContext
    {
        return UseService(factoryCallback, new DbContextOptionsSource<TContext>(options), configurator);
    }

    /// <summary>
    /// Registra un servicio para acceder a datos.
    /// </summary>
    /// <typeparam name="TService">Tipo de servicio a registrar.</typeparam>
    /// <typeparam name="TContext">Tipo de contexto de datos a registrar.</typeparam>
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
    ITritonConfigurable UseService<TService, TContext>(Func<IMiddlewareConfigurator, EfCoreTransFactory<TContext>, TService> factoryCallback, Action<DbContextOptionsBuilder<TContext>> configCallback, IMiddlewareConfigurator? configurator = null)
        where TService : ITritonService where TContext : DbContext
    {
        return UseService(factoryCallback, new DbContextOptionsSource<TContext>(configCallback), configurator);
    }

    /// <summary>
    /// Registra un servicio para acceder a datos.
    /// </summary>
    /// <typeparam name="TService">Tipo de servicio a registrar.</typeparam>
    /// <typeparam name="TContext">Tipo de contexto de datos a registrar.</typeparam>
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
    ITritonConfigurable UseService<TService, TContext>(Func<IMiddlewareConfigurator, EfCoreTransFactory<TContext>, TService> factoryCallback, DbContextOptionsSource<TContext>? optionsSource, IMiddlewareConfigurator? configurator = null)
        where TService : ITritonService
        where TContext : DbContext
    {
        Pool.Register(() => factoryCallback.Invoke(configurator ?? Pool.Resolve<IMiddlewareConfigurator>() ?? new TransactionConfiguration(), new EfCoreTransFactory<TContext>(optionsSource ?? DbContextOptionsSource.None)));
        return this;
    }

    /// <summary>
    /// Registra un servicio para acceder a datos.
    /// </summary>
    /// <param name="contextType">Tipo de contexto de datos a registrar.</param>
    /// <param name="factoryCallback">Método a llamar para crear el servicio.</param>
    /// <param name="configurator">
    /// Configuración de Middleware a utilizar al generar transacciones.
    /// </param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    ITritonConfigurable UseService(Type contextType, Func<IMiddlewareConfigurator, ITransactionFactory, ITritonService> factoryCallback, IMiddlewareConfigurator? configurator = null)
    {
        return UseService(contextType, factoryCallback, (DbContextOptionsSource?)null, configurator);
    }

    /// <summary>
    /// Registra un servicio para acceder a datos.
    /// </summary>
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
    ITritonConfigurable UseService(Type contextType, Func<IMiddlewareConfigurator, ITransactionFactory, ITritonService> factoryCallback, DbContextOptions options, IMiddlewareConfigurator? configurator = null)
    {
        return UseService(contextType, factoryCallback, new DbContextOptionsSource(options), configurator);
    }

    /// <summary>
    /// Registra un servicio para acceder a datos.
    /// </summary>
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
    ITritonConfigurable UseService(Type contextType, Func<IMiddlewareConfigurator, ITransactionFactory, ITritonService> factoryCallback, Action<DbContextOptionsBuilder> configCallback, IMiddlewareConfigurator? configurator = null)
    {
        return UseService(contextType, factoryCallback, new DbContextOptionsSource(configCallback), configurator);
    }

    /// <summary>
    /// Registra un servicio para acceder a datos.
    /// </summary>
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
    ITritonConfigurable UseService(Type contextType, Func<IMiddlewareConfigurator, ITransactionFactory, ITritonService> factoryCallback, DbContextOptionsSource? optionsSource, IMiddlewareConfigurator? configurator = null)
    {
        CheckContextType(contextType);
        Pool.Register(() => factoryCallback.Invoke(configurator ?? Pool.Resolve<IMiddlewareConfigurator>() ?? new TransactionConfiguration(), CreateEfFactory(contextType, optionsSource)));
        return this;
    }

    /// <summary>
    /// Agrega un Middleware a la configuración predeterminada de transacciones
    /// a utilizar por los servicios de Tritón.
    /// </summary>
    /// <typeparam name="T">Tipo de Middleware a agregar.</typeparam>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    /// <seealso cref="ConfigureMiddlewares(Action{IMiddlewareConfigurator})"/>.
    ITritonConfigurable UseMiddleware<T>() where T : ITransactionMiddleware, new() => UseMiddleware<T>(out _);

    /// <summary>
    /// Agrega un Middleware a la configuración predeterminada de transacciones
    /// a utilizar por los servicios de Tritón.
    /// </summary>
    /// <param name="newMiddleware">
    /// Parámetro de salida. Middleware que ha sido creado y registrado.
    /// </param>
    /// <typeparam name="T">Tipo de Middleware a agregar.</typeparam>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    /// <seealso cref="ConfigureMiddlewares(Action{IMiddlewareConfigurator})"/>.
    ITritonConfigurable UseMiddleware<T>(out T newMiddleware) where T : ITransactionMiddleware, new()
    {
        newMiddleware = new();
        GetMiddlewareConfigurator().Attach(newMiddleware);
        return this;
    }

    /// <summary>
    /// Descubre automáticamente todos los servicios y contextos de datos a
    /// exponer por medio de <see cref="ServicePool"/>.
    /// </summary>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    ITritonConfigurable DiscoverContexts()
    {
        foreach (var j in ReflectionHelpers.GetTypes<DbContext>(true).Where(p => p.GetConstructor(Type.EmptyTypes) is not null))
        {
            UseContext(j);
        }
        return this;
    }

    /// <summary>
    /// Ejecuta un método de configuración de middlewares predeterminados a
    /// utilizar cuando no se especifique una configuración personalizada al
    /// registrar un contexto o un servicio de Tritón.
    /// </summary>
    /// <param name="configuratorCallback">
    /// Método a utilizar para configurar los Middlewares a utilizar en las
    /// transacciones de la instancia de Tritón configurada.
    /// </param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    /// <remarks>
    /// Para objetos que implementan <see cref="ITransactionMiddleware"/>,
    /// puede utilizar el método <see cref="UseMiddleware{T}(out T)"/> o
    /// <see cref="UseMiddleware{T}()"/> en su lugar.
    /// </remarks>
    /// <seealso cref="UseMiddleware{T}(out T)"/>.
    /// <seealso cref="UseMiddleware{T}()"/>.
    ITritonConfigurable ConfigureMiddlewares(Action<IMiddlewareConfigurator> configuratorCallback)
    {
        (configuratorCallback ?? throw new ArgumentNullException(nameof(configuratorCallback)))
            .Invoke(GetMiddlewareConfigurator());
        return this;
    }

    /// <summary>
    /// Agrega una colección de acciones de Middleware de prólogo a la
    /// configuración de transacciones registrada.
    /// </summary>
    /// <param name="actions">Acciones de prólogo a agregar.</param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    ITritonConfigurable UseTransactionPrologs(params MiddlewareAction[] actions)
    {
        foreach (MiddlewareAction j in actions)
        {
            GetMiddlewareConfigurator().AddProlog(j);
        }
        return this;
    }

    /// <summary>
    /// Agrega una colección de acciones de Middleware de epílogo a la
    /// configuración de transacciones registrada.
    /// </summary>
    /// <param name="actions">Acciones de epílogo a agregar.</param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    ITritonConfigurable UseTransactionEpilogs(params MiddlewareAction[] actions)
    {
        foreach (MiddlewareAction j in actions)
        {
            GetMiddlewareConfigurator().AddEpilog(j);
        }
        return this;
    }

    private IMiddlewareConfigurator GetMiddlewareConfigurator()
    {
        return Pool.Resolve<IMiddlewareConfigurator>() ?? new TransactionConfiguration().RegisterInto(Pool);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerNonUserCode]
    private static void CheckContextType(Type contextType)
    {
        if (!contextType.Implements<DbContext>())
        {
            throw Errors.TypeMustImplDbContext(nameof(contextType));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerNonUserCode]
    private static ITransactionFactory CreateEfFactory(Type contextType, IDbContextOptionsSource? optionsSource)
    {
        CheckContextType(contextType);
        return typeof(EfCoreTransFactory<>).MakeGenericType(contextType).New<ITransactionFactory>(optionsSource ?? DbContextOptionsSource.None);
    }
}