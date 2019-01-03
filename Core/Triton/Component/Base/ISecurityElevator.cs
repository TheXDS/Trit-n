using System.Reflection;
using TheXDS.MCART.Security.Password;

namespace TheXDS.Triton.Component.Base
{
    

    /// <summary>
    ///     Define una serie de miembros a implementar por una clase que pueda
    ///     otorgar o denegar solicitudes de elevación de permisos para
    ///     ejecutar un método específico.
    /// </summary>
    public interface ISecurityElevator
    {
        /// <summary>
        ///     Comprueba los permisos de ejecución de un método, y realiza una
        ///     elevación de los mismos en caso de ser necesario.
        /// </summary>
        /// <param name="method">
        ///     Método a ejecutar.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> si se ha otorgado el permiso requerido
        ///     para ejecutar el método, <see langword="false"/> en caso
        ///     contrario.
        /// </returns>
        bool Elevate(MethodBase method);

        /// <summary>
        ///     Obtiene un valor que indica si este <see cref="ISecurityElevator"/>
        ///     se encuentra en modo elevado.
        /// </summary>
        bool Elevated { get; }

        /// <summary>
        ///     Indica a este <see cref="ISecurityElevator"/> que debe revocar
        ///     la elevación activa y utilizar las credenciales estándar de
        ///     comprobación de permisos.
        /// </summary>
        void Revoke();

        /// <summary>
        ///     Obtiene el modo de elevación utilizado por este
        ///     <see cref="ISecurityElevator"/>.
        /// </summary>
        ElevationMode Mode { get; }
    }

    /// <summary>
    ///     Enumera los posibles modos de elevación a utilizar por un
    ///     <see cref="ISecurityElevator"/>.
    /// </summary>
    public enum ElevationMode
    {
        /// <summary>
        ///     Elevación única de solicitud. Eleva, y olvida las nuevas
        ///     credenciales inmediatamente.
        /// </summary>
        Once,
        /// <summary>
        ///     Elevación con marca de tiempo. Eleva y mantiene las nuevas
        ///     credenciales durante un tiempo específico.
        /// </summary>
        Timeout,
        /// <summary>
        ///     Elevación persistente. Mantiene la elevación de credenciales
        ///     hasta que se ejecute una solicitud de revocación.
        /// </summary>
        Hold
    }
}