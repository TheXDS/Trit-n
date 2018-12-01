namespace TheXDS.Triton.Models.Base
{
    /// <summary>
    ///     Define una serie de propiedades a implementar por un modelo que
    ///     pueda ser marcado como eliminado sin ser removido de la base de
    ///     datos.
    /// </summary>
    public interface ISoftDeletable
    {
        /// <summary>
        ///     Obtiene o establece un valor que indica si el elemento ha sido
        ///     borrado o no.
        /// </summary>
        bool IsDeleted { get; set; }

        /// <summary>
        ///     Obtiene un valor que indica si la entidad puede ser borrada.
        /// </summary>
        bool CanBeDeleted { get; }
    }
}