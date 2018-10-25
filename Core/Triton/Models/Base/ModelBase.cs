using System;
using System.ComponentModel.DataAnnotations;

namespace TheXDS.Triton.Models.Base
{
    public abstract class ModelBase<T> where T : struct, IComparable<T>
    {
        /// <summary>
        ///     Campo llave primario de la entidad.
        /// </summary>
        [Key]
        public T Id { get; set; }
    }
    public abstract class ModelBase:ModelBase<Guid> { }
}
