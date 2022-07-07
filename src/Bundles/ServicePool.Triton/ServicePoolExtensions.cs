using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TheXDS.MCART.Types.Extensions;
using TheXDS.ServicePool.Extensions;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;

namespace TheXDS.ServicePool.Triton
{
    /// <summary>
    /// Contiene métodos de extensión que permiten configurar Tritón para
    /// utilizarse en conjunto con
    /// <see cref="ServicePool"/>.
    /// </summary>
    public static class ServicePoolExtensions
    {
        /// <summary>
        /// Configura un <see cref="ServicePool"/> para
        /// hostear servicios de datos de Tritón.
        /// </summary>
        /// <param name="pool">
        /// <see cref="ServicePool"/> a configurar.
        /// </param>
        /// <returns>
        /// Un objeto que puede utilizarse para configiurar los servicios de
        /// Tritón.
        /// </returns>
        public static ITritonConfigurable UseTriton(this ServicePool pool)
        {
            pool.RegisterNow(new TransactionConfiguration());
            return pool.Discover<ITritonConfigurable>() ?? new TritonConfigurable(pool).RegisterInto(pool);
        }

        /// <summary>
        /// Configura un <see cref="ServicePool"/> para
        /// hostear servicios de datos de Tritón.
        /// </summary>
        /// <param name="pool">
        /// <see cref="ServicePool"/> a configurar.
        /// </param>
        /// <param name="configurator">
        /// Delegado de configuración de los servicios de Tritón.
        /// </param>
        /// <returns>
        /// La misma instancia que <paramref name="pool"/>, permitiendo el uso
        /// de sintaxis Fluent.
        /// </returns>
        public static ServicePool UseTriton(this ServicePool pool, Action<ITritonConfigurable> configurator)
        {
            configurator(UseTriton(pool));
            return pool;
        }

        /// <summary>
        /// Resuelve una instancia de un servicio que puede utilizarse para 
        /// acceder a la base de datos solicitada.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de contexto de datos a utilizar.
        /// </typeparam>
        /// <param name="pool">
        /// <see cref="ServicePool"/> a configurar.
        /// </param>
        /// <returns>
        /// Una instancia de <see cref="ITritonService"/> que permite acceder al 
        /// contexto de datos solicitado.
        /// </returns>
        /// <remarks>
        /// Prefiera resolver directamente un <see cref="ITritonService"/> si
        /// necesita acceder directamente a la funcionalidad de un servicio
        /// concreto.
        /// </remarks>
        public static ITritonService ResolveTritonService<T>(this ServicePool pool) where T : DbContext, new()
        {
            var s = pool.ResolveAll<TritonService>().FirstOrDefault(p => p.Factory is EfCoreTransFactory<T>);
            if (s is not null) return s;
            pool.UseTriton().UseContext<T>();
            return ResolveTritonService<T>(pool);
        }
    }
}