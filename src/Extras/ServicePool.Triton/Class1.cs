using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;

namespace ServicePool.Triton
{
    /// <summary>
    /// Contiene métodos de extensión que permiten configurar Tritón para
    /// utilizarse en conjunto con
    /// <see cref="TheXDS.ServicePool.ServicePool"/>.
    /// </summary>
    public static class ServicePoolExtensions
    {
        /// <summary>
        /// Configura un <see cref="TheXDS.ServicePool.ServicePool"/> para
        /// hostear servicios de datos de Tritón.
        /// </summary>
        /// <param name="pool">
        /// <see cref="TheXDS.ServicePool.ServicePool"/> a configurar.
        /// </param>
        /// <returns>
        /// Una instancia de 
        /// </returns>
        public static ITritonConfigurable UseTriton(this TheXDS.ServicePool.ServicePool pool)
        {
            return UseTriton(pool, _ => { });
        }

        public static ITritonConfigurable UseTriton(this TheXDS.ServicePool.ServicePool pool, Action<IMiddlewareConfigurator> middlewareConfigurator)
        {
            TransactionConfiguration? tc = pool.Resolve<TransactionConfiguration>();
            if (tc is null)
            {
                pool.RegisterNow(tc = new());
                middlewareConfigurator(tc);
            }
            return new TritonConfigurable(pool);
        }

        public static IService ResolveTritonService<T>(this TheXDS.ServicePool.ServicePool pool) where T : DbContext, new()
        {
            var s = pool.ResolveAll<Service>().FirstOrDefault(p => p.Factory.GetType().Implements<EfCoreTransFactory<T>>());
            if (s is null)
            {
                UseTriton(pool).UseContext<T>();
                return ResolveTritonService<T>(pool);
            }
            return s;
        }
    }

    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que exponga
    /// funciones de configuración para Tritón cuando se utiliza en conjunto
    /// con un <see cref="TheXDS.ServicePool.ServicePool"/>.
    /// </summary>
    public interface ITritonConfigurable
    {
        /// <summary>
        /// Descubre automáticamente todos los servicios y contextos de datos a
        /// exponer por medio de <see cref="TheXDS.ServicePool.ServicePool"/>.
        /// </summary>
        /// <returns>
        /// La misma instancia del objeto utilizado para configurar Tritón.
        /// </returns>
        ITritonConfigurable DiscoverContexts();

        /// <summary>
        /// Agrega un <see cref="DbContext"/> a la colección de servicios
        /// hosteados dentro de un
        /// <see cref="TheXDS.ServicePool.ServicePool"/>, envolviendolo en un 
        /// <see cref="Service"/>.
        /// </summary>
        /// <typeparam name="T">Tipo de contexto a registrar.</typeparam>
        /// <returns>
        /// La misma instancia del objeto utilizado para configurar Tritón.
        /// </returns>
        ITritonConfigurable UseContext<T>() where T : DbContext, new();

        /// <summary>
        /// Agrega un servicio a la colección de servicios hosteados dentro de
        /// un <see cref="TheXDS.ServicePool.ServicePool"/>.
        /// </summary>
        /// <typeparam name="T">Tipo de servicio a registrar.</typeparam>
        /// <returns>
        /// La misma instancia del objeto utilizado para configurar Tritón.
        /// </returns>
        ITritonConfigurable UseService<T>() where T : Service;
    }

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

        public ITritonConfigurable UseService<T>() where T : Service
        {
            pool.Register<T>();
            return this;
        }
    }
}