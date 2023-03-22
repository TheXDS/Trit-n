using Microsoft.EntityFrameworkCore;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Extensions;
using TheXDS.ServicePool.Triton.Resources;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;

namespace TheXDS.ServicePool.Triton;

internal class TritonConfigurable : ITritonConfigurable
{
    /// <inheritdoc/>
    public ServicePool Pool { get; }

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="TritonConfigurable"/>.
    /// </summary>
    /// <param name="pool">
    /// Repositorio de servicios en el cual se está registrando esta
    /// instancia.
    /// </param>
    public TritonConfigurable(ServicePool pool)
    {
        Pool = pool;
    }

    /// <inheritdoc/>
    public ITritonConfigurable DiscoverContexts()
    {
        foreach (var j in ReflectionHelpers.GetTypes<DbContext>(true).Where(p => p.GetConstructor(Type.EmptyTypes) is not null))
        {
            UseContext(j);
        }
        return this;
    }

    /// <inheritdoc/>
    public ITritonConfigurable UseContext<T>() where T : DbContext, new()
    {
        return UseContext(typeof(T));
    }

    /// <inheritdoc/>
    public ITritonConfigurable UseContext(Type context)
    {
        if (!context.Implements<DbContext>()) 
        {
            throw Errors.TypeMustImplDbContext(nameof(context));
        }
        Pool.Register(() => new TritonService(Pool.Discover<IMiddlewareConfigurator>() ?? new TransactionConfiguration(), typeof(EfCoreTransFactory<>).MakeGenericType(context).New<ITransactionFactory>()));
        return this;
    }

    /// <inheritdoc/>
    public ITritonConfigurable UseMiddleware<T>(out T newMiddleware) where T : ITransactionMiddleware, new()
    {
        newMiddleware = new();
        Pool.Discover<IMiddlewareConfigurator>()!.Attach(newMiddleware);
        return this;
    }

    /// <inheritdoc/>
    public ITritonConfigurable UseTransactionPrologs(params MiddlewareAction[] actions)
    {
        foreach (MiddlewareAction j in actions)
        {
            Pool.Discover<IMiddlewareConfigurator>()!.AddProlog(j);
        }
        return this;
    }

    /// <inheritdoc/>
    public ITritonConfigurable UseTransactionEpilogs(params MiddlewareAction[] actions)
    {
        foreach (MiddlewareAction j in actions)
        {
            Pool.Discover<IMiddlewareConfigurator>()!.AddEpilog(j);
        }
        return this;
    }

    /// <inheritdoc/>
    public ITritonConfigurable UseService<T>() where T : TritonService
    {
        Pool.Register<T>();
        return this;
    }

    /// <inheritdoc/>
    public ITritonConfigurable ConfigureMiddlewares(Action<IMiddlewareConfigurator> configuratorCallback)
    {
        (configuratorCallback ?? throw new ArgumentNullException(nameof(configuratorCallback))).Invoke(Pool.Discover<IMiddlewareConfigurator>()!);
        return this;
    }
}