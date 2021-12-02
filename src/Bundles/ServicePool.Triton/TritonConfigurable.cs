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
                pool.Register(() => new Service(pool.Resolve<TransactionConfiguration>()!, typeof(EfCoreTransFactory<>).MakeGenericType(j).New<ITransactionFactory>()));
            }
            return this;
        }

        public ITritonConfigurable UseContext<T>() where T : DbContext, new()
        {
            pool.Register(() => new Service(pool.Discover<TransactionConfiguration>() ?? new TransactionConfiguration(), new EfCoreTransFactory<T>()));
            return this;
        }

        public ITritonConfigurable UseMiddleware<T>() where T : ITransactionMiddleware, new()
        {
            TransactionConfiguration? c = pool.Discover<TransactionConfiguration>();
            if (c is null)
            {
                c = new TransactionConfiguration();
                pool.RegisterNow(c);
            }
            c.Attach<T>();
            return this;
        }

        public ITritonConfigurable UseService<T>() where T : Service
        {
            pool.Register<T>();
            return this;
        }
    }
}