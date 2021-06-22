using System;
using System.ComponentModel.DataAnnotations;

namespace TheXDS.Triton.Models.Base
{
    /// <summary>
    /// Clase base para todos los modelos que contengan información de
    /// versión de fila para permitir concurrencia de acceso.
    /// </summary>
    /// <typeparam name="T">Tipo de campo llave de la entidad.</typeparam>
    public abstract class ConcurrentModel<T> : Model<T> where T : notnull, IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// Implementa un campo de versión de fila para permitir
        /// concurrencia.
        /// </summary>
        [Timestamp]
        public byte[] RowVersion { get; set; } = default!;
    }
}
