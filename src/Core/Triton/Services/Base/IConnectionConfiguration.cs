namespace TheXDS.Triton.Services.Base
{
    /// <summary>
    ///     Define una serie de miembros a implementar por un tipo que exponga propiedades de configuración para administrar conexiones de datos.
    /// </summary>
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
}