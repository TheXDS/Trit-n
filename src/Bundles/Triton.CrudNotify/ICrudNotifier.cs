using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Middleware
{
    /// <summary>
    ///     Define una serie de miembros a implementar por un tipo que envíe
    ///     notificaciones de eventos Crud a otros equipos conectados.
    /// </summary>
    public interface ICrudNotifier
    {
        /// <summary>
        ///     Envía una notificación de un evento Crud a todos los equipos
        ///     conectados.
        /// </summary>
        /// <param name="action">
        ///     Acción Crud que se ha realizado.
        /// </param>
        /// <param name="entity">
        ///     Entidad sobre la cual se ha realizado una operación Crud.
        /// </param>
        /// <returns>
        ///     El resultado de una operación de servicio.
        /// </returns>
        ServiceResult NotifyPeers(CrudAction action, Model? entity);
    }
}
