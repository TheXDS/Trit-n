namespace TheXDS.Triton.Services.Base
{
    /// <summary>
    ///     Define una serie de miembros a implementar por un tipo que exponga
    ///     propiedades de configuración básicas utilizadas por todos los
    ///     servicios de Tritón.
    /// </summary>
    public interface IServiceConfigurationBase
    {
        /// <summary>
        ///     Obtiene la configuración a utilizar para administrar las
        ///     conexiones a datos.
        /// </summary>
        IConnectionConfiguration ConnectionConfiguration { get; }
    }
}