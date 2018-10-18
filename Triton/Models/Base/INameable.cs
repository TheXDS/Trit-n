namespace TheXDS.Triton.Models.Base
{
    /// <summary>
    /// Define una serie de propiedades a implementar por modelos que puedan contener nombre.
    /// </summary>
    public interface INameable
    {
        /// <summary>
        /// Obtiene o establece el nombre de la entidad.
        /// </summary>
        string Name { get; set; }
    }
}