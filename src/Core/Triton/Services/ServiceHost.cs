using System;
using System.Collections.ObjectModel;
using TheXDS.Triton.Services.Base;
using System.Linq;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.Exceptions;
using TheXDS.Triton.Exceptions;

namespace TheXDS.Triton.Services
{
    /// <summary>
    /// Aloja una colección de servicios de Tritón.
    /// </summary>
    public class ServiceHost : Collection<IService>
    {
        /// <summary>
        /// Descarga todos los servicios cargados de este host.
        /// </summary>
        public new void Clear()
        {
            foreach (var j in Enumerable.OfType<IDisposable>(this)) j.Dispose();
            base.Clear();
        }

        /// <summary>
        /// Quita una intancia activa de servicio de este Host.
        /// </summary>
        /// <param name="item">Instancia del servicio a quitar.</param>
        /// <returns>
        /// <see langword="true"/> si la instancia ha sido quitada
        /// exitosamente; <see langword="false"/> en caso contrario, por
        /// ejemplo si la misma no existía en esta colección.
        /// </returns>
        public new bool Remove(IService item)
        {
            var retval = base.Remove(item);
            if (retval && item is IDisposable i) i.Dispose();
            return retval;
        }

        /// <summary>
        /// Obtiene la instancia del servicio del tipo especificado.
        /// </summary>
        /// <typeparam name="T">Tipo de servicio a invocar.</typeparam>
        /// <returns>
        /// Una instancia activa del servicio existente en este host.
        /// </returns>
        /// <exception cref="MissingServiceException">
        /// Se produce si el tipo de servicio invocado no existe en este host.
        /// </exception>
        public T Get<T>() where T : notnull, IService
        {
            return this.FirstOf<T>() ?? throw new MissingServiceException(typeof(T));
        }

        /// <summary>
        /// Indizador que obtiene o establece la instancia del servicio del
        /// tipo especificado.
        /// </summary>
        /// <param name="type">
        /// Tipo de servicio a obtener o establecer.
        /// </param>
        /// <returns>
        /// Una instancia activa del servicio existente en este host.
        /// </returns>
        /// <exception cref="MissingServiceException">
        /// Se produce si el tipo de servicio invocado no existe en este host.
        /// </exception>
        /// <exception cref="InvalidArgumentException">
        /// Se produce si <paramref name="type"/> no es un tipo que implementa
        /// <see cref="IService"/>.
        /// </exception>
        /// <exception cref="InvalidTypeException">
        /// Se produce si el valor a establecer para el tipo de servicio
        /// especificado no implementa el mismo.
        /// </exception>
        public IService this[Type type]
        {
            get
            {
                return this.FirstOf(type) ?? throw new MissingServiceException(type);
            }
            set
            {
                This_Contract(type, value);
                if (this.FirstOf(type) is IService oldSvc) Remove(oldSvc);
                value?.PushInto(this);
            }
        }

        private static void This_Contract(Type type, IService value)
        {
            if (!type.Implements<IService>()) throw new InvalidArgumentException(nameof(type));
            if (!value?.GetType().Implements(type) ?? false) throw new InvalidTypeException();
        }
    }
}
