using TheXDS.ServicePool.Extensions;
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
    PoolBase Pool { get; }

    /// <summary>
    /// Registra un servicio para acceder a datos.
    /// </summary>
    /// <param name="serviceBuilder">
    /// Método a llamar para crear el servicio.
    /// </param>
    /// <param name="transactionFactoryBuilder">
    /// Método a llamar para crear al objeto a utilizar como fábrica de
    /// transacciones.
    /// </param>
    /// <param name="configurator">
    /// Configuración de Middleware a utilizar al generar transacciones.
    /// </param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    ITritonConfigurable UseService(Func<IMiddlewareConfigurator, ITransactionFactory, ITritonService> serviceBuilder, Func<ITransactionFactory> transactionFactoryBuilder, IMiddlewareConfigurator? configurator = null)
    {
        Pool.Register(() => serviceBuilder.Invoke(configurator ?? Pool.Resolve<IMiddlewareConfigurator>() ?? new TransactionConfiguration(), transactionFactoryBuilder.Invoke()));
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

    private static TransactionConfiguration RegisterNewIntoPool(PoolBase pool)
    {
        var t = new TransactionConfiguration();

        switch (pool)
        {
            case FlexPool fp: return t.RegisterInto(fp);
            case Pool p:
                p.RegisterNow(t, [typeof(IMiddlewareConfigurator), typeof(IMiddlewareRunner)]);
                return t;
            default: throw new NotImplementedException();
        }
    }
}