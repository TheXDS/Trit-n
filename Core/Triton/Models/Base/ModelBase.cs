using System;
using System.ComponentModel.DataAnnotations;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Triton.Models.Base
{
    /// <summary>
    ///     Clase base para los modelos de datos administrables por
    ///     <see cref="TheXDS.Triton"/> que establece el tipo de campo llave a
    ///     utilizar.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ModelBase<T> : ModelBase where T : struct, IComparable<T>
    {
        /// <summary>
        ///     Campo llave primario de la entidad.
        /// </summary>
        [Key]
        public T Id { get; set; }

        private protected override Type KeyType => typeof(T);
    }

    /// <summary>
    ///     Clase base para todos los modelos de datos administrables por
    ///     <see cref="TheXDS.Triton"/>.
    /// </summary>
    public abstract class ModelBase
    {
        internal static bool KeyMatches(Type tModel, Type tKey)
        {
            if (!tModel.Implements<ModelBase>()) return false;
            if (tKey.IsClass || tKey.IsInterface || tKey.IsAbstract ||
                tKey.IsPointer || tKey.IsNotPublic ||
                !tKey.Implements(typeof(IComparable<>).MakeGenericType(tKey)))
                return false;
            if (!tModel.IsInstantiable()) return false;
            var m = tModel.New<ModelBase>();
            var r = m.KeyType == tKey;
            (m as IDisposable)?.Dispose();
            return r;
        }

        // ReSharper disable once MemberCanBeProtected.Global
        private protected abstract Type KeyType { get; }
    }

    public abstract class SoftDeletableModelBase<T> : ModelBase<T>, ISoftDeletable where T : struct, IComparable<T>
    {
        /// <inheritdoc />
        /// <summary>
        ///     Obtiene o establece un valor que indica si el elemento ha sido
        ///     borrado o no.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <inheritdoc />
        /// <summary>
        ///     Obtiene un valor que indica si la entidad puede ser borrada.
        /// </summary>
        public virtual bool CanBeDeleted => true;
    }
}
