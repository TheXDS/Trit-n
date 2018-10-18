using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TheXDS.Triton.Component;

namespace TheXDS.Triton.Models.Base
{
    public abstract class ViewModelBase<TModel, TKey> : NotifyPropertyChanged
        where TModel : ModelBase<TKey>, new()
        where TKey : struct, IComparable<TKey>
    {
        private static readonly HashSet<PropertyInfo> ModelProperties = new HashSet<PropertyInfo>();

        static ViewModelBase()
        {
            foreach (var j in typeof(TModel)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite))
            {
                ModelProperties.Add(j);
            }
        }

        protected void Refresh()
        {
            lock (Entity)
            {
                foreach (var j in ModelProperties)
                {
                    OnPropertyChanged(j.Name);
                }
            }
        }

        protected TModel Entity { get; }


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