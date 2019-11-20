using System;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Security.Base;

namespace TheXDS.Triton.Services.Base
{
    /// <summary>
    ///     Define una serie de miembros a implementar por un tipo que exponga
    ///     propiedades de configuración para administrar conexiones de datos.
    /// </summary>
    public interface ITransactionConfiguration
    {
        /// <summary>
        ///     Obtiene un valor que representa la cantidad de milisegundos a
        ///     esperar al servidor de datos antes de marcar la operación como
        ///     fallida.
        /// </summary>
        int ConnectionTimeout => 15000;

        /// <summary>
        ///     Obtiene un objeto que permite publicar notificaciones acerca de
        ///     la ejecución de operaciones Crud sobre una entidad.
        /// </summary>
        ICrudNotificationSource? Notifier { get; }

        /// <summary>
        ///     Realiza comprobaciones adicionales antes de ejecutar una acción
        ///     de crud, devolviendo <see langword="null"/> si la operación 
        ///     puede continuar.
        /// </summary>
        /// <param name="action">
        ///     Acción Crud a intentar realizar.
        /// </param>
        /// <param name="entity">
        ///     Entidad sobre la cual se ejecutará la acción.
        /// </param>
        /// <returns>
        ///     Un <see cref="ServiceResult"/> con el resultado del preámbulo,
        ///     o <see langword="null"/> si la operación puede continuar.
        /// </returns>
        ServiceResult? Preamble(CrudAction action, Model? entity) => null;
    }
}