using Microsoft.EntityFrameworkCore;
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
        return UseContext(contextType, (DbContextOptionsSource?)null, configurator);
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
        return UseContext(contextType, new DbContextOptionsSource(options), configurator);
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
        return UseContext(contextType, new DbContextOptionsSource(builder), configurator);
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
        return UseContext((DbContextOptionsSource<T>?)null, configurator);
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
        return UseContext(new DbContextOptionsSource<T>(options), configurator);
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
        return UseContext(new DbContextOptionsSource<T>(builder), configurator);
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
        var factory = typeof(EfCoreTransFactory<T>).New<ITransactionFactory>((IDbContextOptionsSource?)optionsSource ?? DbContextOptionsSource.None);
        Pool.Register(() => new TritonService(configurator ?? Pool.Resolve<IMiddlewareConfigurator>() ?? new TransactionConfiguration(), factory));
        return this;
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
        if (!contextType.Implements<DbContext>())
        {
            throw Errors.TypeMustImplDbContext(nameof(contextType));
        }
        var factory = typeof(EfCoreTransFactory<>).MakeGenericType(contextType).New<ITransactionFactory>(optionsSource ?? DbContextOptionsSource.None);
        Pool.Register(() => new TritonService(configurator ?? Pool.Resolve<IMiddlewareConfigurator>() ?? new TransactionConfiguration(), factory));
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
}