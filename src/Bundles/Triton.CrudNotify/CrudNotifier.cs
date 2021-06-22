using System.Collections.Generic;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Middleware
{
    /// <summary>
    /// Middleware que permite enviar notificaciones de acciones crud por
    /// medio de la red a otros clientes conectados a través de un
    /// protocolo TCP personalizado.
    /// </summary>
    public static class CrudNotifier
    {
        private static readonly List<ICrudNotifier> _notifiers = new();
        
        /// <summary>
        /// Agrega una nueva instancia de un servicio de notificación de
        /// eventos Crud a todas las transacciones de datos.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de servicio de notificación a instanciar.
        /// </typeparam>
        /// <param name="config">
        /// Objeto de configuración de transacciones a configurar.
        /// </param>
        /// <returns>
        /// La misma instancia que <paramref name="config"/>.
        /// </returns>
        public static TransactionConfiguration AddNotifyService<T>(this TransactionConfiguration config) where T : ICrudNotifier, new()
        {
            return AddNotifyService(config, new T());
        }

        /// <summary>
        /// Agrega un servicio de notificación de eventos Crud a todas las
        /// transacciones de datos.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de servicio de notificación a agregar.
        /// </typeparam>
        /// <param name="config">
        /// Objeto de configuración de transacciones a configurar.
        /// </param>
        /// <param name="crudNotifier">
        /// Instancia de un servicio de notificaciones de eventos Crud a
        /// agregar a las transacciones de datos.
        /// </param>
        /// <returns>
        /// La misma instancia que <paramref name="config"/>.
        /// </returns>
        public static TransactionConfiguration AddNotifyService<T>(this TransactionConfiguration config, T crudNotifier) where T : ICrudNotifier
        {
            return config.AddEpilog(crudNotifier.PushInto(_notifiers).NotifyPeers);
        }
    }
}
