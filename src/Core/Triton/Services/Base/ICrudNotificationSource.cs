using System.Runtime.CompilerServices;
using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Services.Base
{
    /// <summary>
    ///     Define una serie de miembros a implementar por un tipo que permita
    ///     notificar eventos CRUD externamente.
    /// </summary>
    public interface ICrudNotificationSource
    {
        /// <summary>
        ///     Notifica a los receptores pertinentes acerca de una operación
        ///     Crud sobre una entidad especificada.
        /// </summary>
        /// <param name="entity">
        ///     Entidad sobre la cual se ha realizado una operación Crud.
        /// </param>
        /// <param name="action">
        ///     Operación realizada.
        /// </param>
        /// <returns>
        ///     El resultado reportado de la operación ejecutada por el
        ///     servicio subyacente.
        /// </returns>
        ServiceResult Notify(Model entity, CrudAction action);

        /// <summary>
        ///     Ayuda a determinar al origen de notificaciones sobre el momento
        ///     en el que los cambios ya han sido escritos en la base de datos.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Commit() { }

        /// <summary>
        ///     Ayuda a determinar al origen de notificaciones sobre una falla
        ///     a la hora de realizar el guardado de los datos.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Fail() { }
    }
}
