namespace TheXDS.Triton.Services.Base
{
    /// <summary>
    ///     Define una serie de miembros a implementar por un tipo que exponga
    ///     propiedades de configuración utilizadas por un servicio ligero.
    /// </summary>
    public interface ILiteServiceConfiguration : IServiceConfigurationBase<ILiteCrudTransactionFactory>
    {
    }
}