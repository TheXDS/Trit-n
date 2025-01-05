using TheXDS.ServicePool.Extensions;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Services;

namespace TheXDS.ServicePool.Triton;

/// <summary>
/// Defines the contract for types that expose a set of 
/// configuration functions for Tritón when used with a
/// <see cref="ServicePool.Pool"/>.
/// </summary>
public interface ITritonConfigurable
{
    /// <summary>
    /// Gets a reference to the service pool in which this instance has been
    /// registered.
    /// </summary>
    Pool Pool { get; }

    /// <summary>
    /// Registers a service for data access.
    /// </summary>
    /// <param name="serviceBuilder">
    /// A method to call to create the service.
    /// </param>
    /// <param name="transactionFactoryBuilder">
    /// A method to call to create the transaction factory object.
    /// </param>
    /// <param name="configurator">
    /// Middleware configuration to use when generating transactions. Defaults
    /// to <see langword="null"/>.
    /// </param>
    /// <returns>
    /// The same instance of the object used to configure Tritón.
    /// </returns>
    ITritonConfigurable UseService(Func<IMiddlewareConfigurator, ITransactionFactory, ITritonService> serviceBuilder, Func<ITransactionFactory> transactionFactoryBuilder, IMiddlewareConfigurator? configurator = null)
    {
        Pool.Register(() => serviceBuilder.Invoke(configurator ?? Pool.Resolve<IMiddlewareConfigurator>() ?? new TransactionConfiguration(), transactionFactoryBuilder.Invoke()));
        return this;
    }

    /// <summary>
    /// Adds a Middleware to the default transaction configuration used by
    /// Tritón services.
    /// </summary>
    /// <typeparam name="T">Type of Middleware to add.</typeparam>
    /// <returns>
    /// The same instance of the object used to configure Tritón.
    /// </returns>
    /// <seealso cref="ConfigureMiddlewares(Action{IMiddlewareConfigurator})"/>.
    ITritonConfigurable UseMiddleware<T>() where T : ITransactionMiddleware, new() => UseMiddleware<T>(out _);

    /// <summary>
    /// Adds a Middleware to the default transaction configuration used by
    /// Tritón services.
    /// </summary>
    /// <param name="newMiddleware">
    /// Parameter output. The newly created and registered Middleware.
    /// </param>
    /// <typeparam name="T">Type of Middleware to add.</typeparam>
    /// <returns>
    /// The same instance of the object used to configure Tritón.
    /// </returns>
    /// <seealso cref="ConfigureMiddlewares(Action{IMiddlewareConfigurator})"/>.
    ITritonConfigurable UseMiddleware<T>(out T newMiddleware) where T : ITransactionMiddleware, new()
    {
        newMiddleware = new();
        GetMiddlewareConfigurator().Attach(newMiddleware);
        return this;
    }

    /// <summary>
    /// Executes a default middleware configuration method to be used when no
    /// custom configuration is specified when registering a context or
    /// service.
    /// </summary>
    /// <param name="configuratorCallback">
    /// Method to use to configure the Middlewares to be used in the
    /// transactions of the configured Tritón instance.
    /// </param>
    /// <returns>
    /// The same instance of the object used to configure Tritón.
    /// </returns>
    /// <remarks>
    /// For objects that implement <see cref="ITransactionMiddleware"/>, you
    /// can use the methods <see cref="UseMiddleware{T}(out T)"/> or
    /// <see cref="UseMiddleware{T}()"/> instead.
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
    /// Adds a collection of prologue Middleware actions to the registered
    /// transaction configuration.
    /// </summary>
    /// <param name="actions">Prologue Middleware actions to add.</param>
    /// <returns>
    /// The same instance of the object used to configure Tritón.
    /// </returns>
    ITritonConfigurable UseTransactionPrologues(params MiddlewareAction[] actions)
    {
        foreach (MiddlewareAction j in actions)
        {
            GetMiddlewareConfigurator().AddPrologue(j);
        }
        return this;
    }

    /// <summary>
    /// Adds a collection of epilogue Middleware actions to the registered
    /// transaction configuration.
    /// </summary>
    /// <param name="actions">Epilogue Middleware actions to add.</param>
    /// <returns>
    /// The same instance of the object used to configure Tritón.
    /// </returns>
    ITritonConfigurable UseTransactionEpilogues(params MiddlewareAction[] actions)
    {
        foreach (MiddlewareAction j in actions)
        {
            GetMiddlewareConfigurator().AddEpilogue(j);
        }
        return this;
    }

    private IMiddlewareConfigurator GetMiddlewareConfigurator()
    {
        return Pool.Resolve<IMiddlewareConfigurator>() ?? new TransactionConfiguration().RegisterInto(Pool);
    }
}