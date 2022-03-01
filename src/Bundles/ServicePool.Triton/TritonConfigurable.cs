using Microsoft.EntityFrameworkCore;
using System;
using TheXDS.MCART.Types.Extensions;
using TheXDS.ServicePool.Triton.Resources;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;

namespace TheXDS.ServicePool.Triton
{
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
            Pool.DiscoverAll<Service>();
            foreach (var j in MCART.Helpers.Objects.GetTypes<DbContext>(true))
            {
                Pool.Register(() => new Service(Pool.Resolve<IMiddlewareConfigurator>()!, typeof(EfCoreTransFactory<>).MakeGenericType(j).New<ITransactionFactory>()));
            }
            return this;
        }

        /// <inheritdoc/>
        public ITritonConfigurable UseContext<T>() where T : DbContext, new()
        {
            Pool.Register(() => new Service(Pool.Discover<IMiddlewareConfigurator>() ?? new TransactionConfiguration(), new EfCoreTransFactory<T>()));
            return this;
        }

        /// <inheritdoc/>
        public ITritonConfigurable UseContext(Type context)
        {
            if (!context.Implements<DbContext>()) 
            {
                throw Errors.TypeMustImplDbContext(nameof(context));
            }
            Pool.Register(() => new Service(Pool.Discover<IMiddlewareConfigurator>() ?? new TransactionConfiguration(), typeof(EfCoreTransFactory<>).MakeGenericType(context).New<ITransactionFactory>()));
            return this;
        }

        /// <inheritdoc/>
        public ITritonConfigurable UseMiddleware<T>() where T : ITransactionMiddleware, new()
        {
            Pool.Discover<IMiddlewareConfigurator>()!.Attach<T>();
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
        public ITritonConfigurable UseService<T>() where T : Service
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
}