using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TheXDS.Triton.Component;

namespace TheXDS.Triton.Models.Base
{
    /// <summary>
    /// Define una serie de propiedades a implementar por un modelo que pueda
    /// ser marcado como eliminado sin ser removido de la base de datos.
    /// </summary>
    public interface ISoftDeletable
    {
        /// <summary>
        ///     Obtiene o establece un valor que indica si el elemento ha sido
        ///     borrado o no.
        /// </summary>
        bool IsDeleted { get; set; }
    }

    public abstract class ModelBase<T> where T : struct, IComparable<T>
    {
        public T Id { get; set; }
    }

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

    /// <summary>
    /// Define una serie de propiedades a implementar por modelos que puedan
    /// exponer una descripción de solo lectura.
    /// </summary>
    public interface IDescriptible
    {
        /// <summary>
        /// Obtiene la descripción de esta entidad.
        /// </summary>
        string Description { get; }
    }

    /// <summary>
    /// Define una serie de propiedades a implementar por modelos que puedan contener nombre.
    /// </summary>
    public interface INameable
    {
        /// <summary>
        /// Obtiene o establece el nombre de la entidad.
        /// </summary>
        string Name { get; set; }
    }
}
