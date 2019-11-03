using System;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Services
{
    /// <summary>
    ///     Expone valores de configuración para incializar servicios.
    /// </summary>
    public class ServiceConfiguration : IServiceConfiguration
    {
        /// <summary>
        ///     Obtiene un objeto que permite manufacturar transacciones Crud.
        /// </summary>
        public ICrudTransactionFactory CrudTransactionFactory { get; private set; }

        /// <summary>
        ///     Obtiene un objeto que permite publicar notificaciones acerca de
        ///     la ejecución de operaciones Crud sobre una entidad.
        /// </summary>
        public ICrudNotificationSource? Notifier { get; }
    }
}