namespace TheXDS.Triton.Models.Base
{
    /// <summary>
    /// Clase base para todos los modelos de datos de Triton.
    /// </summary>
    public abstract class Model
    {
        /// <summary>
        /// Obtiene el Id de la entidad como una cadena.
        /// </summary>
        public abstract string IdAsString { get; }
    }
}
