using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TheXDS.Triton.Models.Base
{
    public abstract class Model
    {
    }

    public abstract class Model<T> : Model where T : IComparable<T>, IEquatable<T>
    {
        public T Id { get; set; }
    }

    public abstract class ConcurrentModel<T> : Model<T> where T : IComparable<T>, IEquatable<T>
    {
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
