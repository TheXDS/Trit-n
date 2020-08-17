namespace TheXDS.Triton.Services.Base
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que
    /// exponga su configuración.
    /// </summary>
    public interface IExposeConfiguration<T>
    {
        /// <summary>
        /// Obtiene una referencia a la configuración expuesta para el
        /// objeto.
        /// </summary>
        T Configuration { get; }
    }
}
