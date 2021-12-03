using Microsoft.EntityFrameworkCore;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Services;

namespace ServicePool.Triton
{
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

        /// <summary>
        /// Agrega un Middleware a la configuración de transacciones a utilizar
        /// por los servicios de Tritón.
        /// </summary>
        /// <typeparam name="T">Tipo de Middleware a agregar.</typeparam>
        /// <returns>
        /// La misma instancia del objeto utilizado para configurar Tritón.
        /// </returns>
        ITritonConfigurable UseMiddleware<T>() where T : ITransactionMiddleware, new();
    }
}