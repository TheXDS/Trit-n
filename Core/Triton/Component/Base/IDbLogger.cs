using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace TheXDS.Triton.Component.Base
{
    /// <summary>
    ///     Define una serie de miembros a implementar por una clase que
    ///     permita el registro de información de bitácora sobre los cambios
    ///     realizados en una base de datos.
    /// </summary>
    public interface IDbLogger
    {
        /// <summary>
        ///     Escribe una nueva entrada de bitácora con la información sobre
        ///     los cambios realizados en una base de datos desde el último
        ///     guardado.
        /// </summary>
        /// <param name="changes">
        ///     Objeto que contiene una colección de seguimiento de los cambios
        ///     realizados en la base de datos.
        /// </param>
        void Log(ChangeTracker changes);
        /// <summary>
        ///     Escribe de forma asíncrona una nueva entrada de bitácora con la
        ///     información sobre los cambios realizados en una base de datos
        ///     desde el último guardado.
        /// </summary>
        /// <param name="changes">
        ///     Objeto que contiene una colección de seguimiento de los cambios
        ///     realizados en la base de datos.
        /// </param>
        /// <returns>
        ///     Una tarea que permite observar el estado de la operación.
        /// </returns>
        Task LogAsync(ChangeTracker changes);
    }
}