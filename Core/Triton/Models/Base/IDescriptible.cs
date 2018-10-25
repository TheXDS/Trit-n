namespace TheXDS.Triton.Models.Base
{
    /// <summary>
    ///     Define una serie de propiedades a implementar por modelos que
    ///     puedan exponer una descripción de solo lectura.
    /// </summary>
    public interface IDescriptible
    {
        /// <summary>
        ///     Obtiene la descripción de esta entidad.
        /// </summary>
        string Description { get; }
    }
}