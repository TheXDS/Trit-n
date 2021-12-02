using Microsoft.EntityFrameworkCore;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;

namespace ServicePool.Triton
{
    internal class TritonConfigurable : ITritonConfigurable
    {
        private readonly TheXDS.ServicePool.ServicePool pool;

        public TritonConfigurable(TheXDS.ServicePool.ServicePool pool)
        {
            this.pool = pool;
        }

        public ITritonConfigurable DiscoverContexts()
        {
            pool.DiscoverAll<Service>();
            foreach (var j in TheXDS.MCART.Helpers.Objects.GetTypes<DbContext>(true))
            {
                pool.Register(() => new Service(pool.Resolve<IMiddlewareConfigurator>()!, typeof(EfCoreTransFactory<>).MakeGenericType(j).New<ITransactionFactory>()));
            }
            return this;
        }

        public ITritonConfigurable UseContext<T>() where T : DbContext, new()
        {
            pool.Register(() => new Service(pool.Discover<IMiddlewareConfigurator>() ?? new TransactionConfiguration(), new EfCoreTransFactory<T>()));
            return this;
        }

        public ITritonConfigurable UseMiddleware<T>() where T : ITransactionMiddleware, new()
        {
            pool.Discover<IMiddlewareConfigurator>()!.Attach<T>();
            return this;
        }

        public ITritonConfigurable UseTransactionPrologs(params MiddlewareAction[] actions)
        {
            foreach (MiddlewareAction j in actions)
            {
                pool.Discover<IMiddlewareConfigurator>()!.AddProlog(j);
            }
            return this;
        }

        public ITritonConfigurable UseTransactionEpilogs(params MiddlewareAction[] actions)
        {
            foreach (MiddlewareAction j in actions)
            {
                pool.Discover<IMiddlewareConfigurator>()!.AddEpilog(j);
            }
            return this;
        }

        public ITritonConfigurable UseService<T>() where T : Service
        {
            pool.Register<T>();
            return this;
        }
    }
}