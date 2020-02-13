using System;
using System.ComponentModel.DataAnnotations;

namespace TheXDS.Triton.Models.Base
{
    /// <summary>
    /// Clase base para todos los modelos de datos de Triton.
    /// </summary>
    public abstract class Model
    {
    }

    /// <summary>
    /// Clase base para todos los modelos de Triton que expone un campo llave a utilizar como Id de la entidad.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Model<T> : Model where T : IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// Obtiene o establece el campo llave de esta entidad.
        /// </summary>
        public T Id { get; set; } = default!;
    }

    /// <summary>
    /// Clase base para todos los modelos que contengan información de
    /// versión de fila para permitir concurrencia de acceso.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ConcurrentModel<T> : Model<T> where T : IComparable<T>, IEquatable<T>
    {
#pragma warning disable CA1819
        /// <summary>
        /// Implementa un campo de versión de fila para permitir
        /// concurrencia.
        /// </summary>
        [Timestamp]
        public byte[] RowVersion { get; set; } = default!;
#pragma warning restore CA1819
    }
}
