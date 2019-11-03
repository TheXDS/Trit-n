using System;

namespace TheXDS.Triton.Services.Base
{
    public interface IConnectionConfiguration
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
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IServiceConfiguration : IConnectionConfiguration
    {

        /// <summary>
        ///     Obtiene un objeto que permite manufacturar transacciones Crud.
        /// </summary>
        ICrudTransactionFactory CrudTransactionFactory { get; }

    }
}