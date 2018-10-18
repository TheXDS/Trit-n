using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TheXDS.Triton.Core.Component;

namespace TheXDS.Triton.Core.Models.Base
{
    public abstract class ViewModelBase<TModel, TKey> : NotifyPropertyChanged
        where TModel : ModelBase<TKey>, new()
        where TKey : struct, IComparable<TKey>
    {
 
        protected TModel Entity { get; }

        /// <summary>
        ///     Obtiene un valor que determina si este elemento es nuevo.
        /// </summary>
        public bool IsNew => Entity.Id.CompareTo(default) == 0;

        /// <summary>
        ///     Obtiene un valor que determina si este elemento puede ser borrado.
        /// </summary>
        public virtual bool CanBeDeleted => true;

        /// <summary>
        ///     Obtiene un valor que determina si este elemento puede ser eliminado por completo
        ///     de la base de datos.
        /// </summary>
        public virtual bool CanBePurged => (Entity as ISoftDeletable)?.IsDeleted ?? true;
    }
}