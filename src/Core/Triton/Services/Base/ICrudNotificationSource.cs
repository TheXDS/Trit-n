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
    }
}
