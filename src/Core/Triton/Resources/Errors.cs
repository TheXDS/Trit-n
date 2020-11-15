using System;
using TheXDS.Triton.Exceptions;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Resources
{
    /// <summary>
    /// Contiene métodos y propiedades públicas que obtienen distintas
    /// excepciones para lanzar o representar estados de error.
    /// </summary>
    public static class Errors
    {
        /// <summary>
        /// Obtiene una excepción que indica que no se ha encontrado el
        /// servicio especificado.
        /// </summary>
        /// <typeparam name="T">
        /// Servicio que no ha sido encontrado.
        /// </typeparam>
        /// <returns>Una nueva instancia de la clase
        /// <see cref="MissingServiceException"/>.
        /// </returns>
        public static Exception MissingService<T>() where T : notnull, IService
        {
            return new MissingServiceException(typeof(T));
        }
    }
}
