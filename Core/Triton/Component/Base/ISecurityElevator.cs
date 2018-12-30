using System.Reflection;

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
    }
}