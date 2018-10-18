using System;

namespace TheXDS.Triton.Core.Models.Base
{
    public abstract class ModelBase<T> where T : struct, IComparable<T>
    {
        public T Id { get; set; }
    }
}
