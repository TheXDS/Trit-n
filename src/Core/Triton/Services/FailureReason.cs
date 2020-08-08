namespace TheXDS.Triton.Services
{
    /// <summary>
    /// Enumera las posibles causas de fallo conocidas para una
    /// operación de servicio.
    /// </summary>
    public enum FailureReason
    {
        /// <summary>
        /// Fallo desconocido.
        /// </summary>
        Unknown,

        /// <summary>
        /// Operación no permitida.
        /// </summary>
        Forbidden,

        /// <summary>
        /// Error en el servicio.
        /// </summary>
        ServiceFailure,

        /// <summary>
        /// Error en la red.
        /// </summary>
        NetworkFailure,

        /// <summary>
        /// Error de la base de datos.
        /// </summary>
        DbFailure,

        /// <summary>
        /// Error de validación de datos.
        /// </summary>
        ValidationError,

        /// <summary>
        /// Error de concurrencia de datos.
        /// </summary>
        ConcurrencyFailure,

        /// <summary>
        /// Entidad no encontrada.
        /// </summary>
        NotFound,

        /// <summary>
        /// Query malformado (error de app cliente)
        /// </summary>
        BadQuery
    }
}