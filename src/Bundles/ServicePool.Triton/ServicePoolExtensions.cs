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
        /// Un objeto que puede utilizarse para configiurar los servicios de
        /// Tritón.
        /// </returns>
        public static ITritonConfigurable UseTriton(this TheXDS.ServicePool.ServicePool pool)
        {
            return UseTriton(pool, _ => { });
        }

        /// <summary>
        /// Configura un <see cref="TheXDS.ServicePool.ServicePool"/> para
        /// hostear servicios de datos de Tritón.
        /// </summary>
        /// <param name="pool">
        /// <see cref="TheXDS.ServicePool.ServicePool"/> a configurar.
        /// </param>
        /// <param name="middlewareConfigurator">
        /// Método a utilizar para configurar los Middleware a inyectar en la
        /// configuración de transacción.
        /// </param>
        /// <returns>
        /// Un objeto que puede utilizarse para configiurar los servicios de
        /// Tritón.
        /// </returns>
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

        /// <summary>
        /// Resuelve una instancia de un servicio que puede utilizarse para 
        /// acceder a la base de datos solicitada.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de contexto de datos a utilizar.
        /// </typeparam>
        /// <param name="pool">
        /// <see cref="TheXDS.ServicePool.ServicePool"/> a configurar.
        /// </param>
        /// <returns>
        /// Una instancia de <see cref="IService"/> que permite acceder al 
        /// contexto de datos solicitado.
        /// </returns>
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
}