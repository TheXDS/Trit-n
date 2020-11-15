using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.MCART.Exceptions;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Exceptions;
using TheXDS.Triton.Services.Base;
using TheXDS.Triton.Resources;

namespace TheXDS.Triton.Services
{
    /// <summary>
    /// Aloja una colección de servicios de Tritón.
    /// </summary>
    public class ServiceHost : ICollection<IService>
    {
        private readonly List<IService> _services = new List<IService>();

        /// <summary>
        /// Obtiene la cuenta de servicios hospedados en esta instancia.
        /// </summary>
        public int Count => ((ICollection<IService>)_services).Count;

        /// <summary>
        /// Obtiene un valor que indica si esta colección es se solo lectura.
        /// </summary>
        public bool IsReadOnly => ((ICollection<IService>)_services).IsReadOnly;

        /// <summary>
        /// Agrega un servicio a este Host.
        /// </summary>
        /// <param name="service">Servicio a agregar.</param>
        public void Add(IService service) => _services.Add(service ?? throw new ArgumentNullException(nameof(service)));

        /// <summary>
        /// Descarga todos los servicios cargados de este host.
        /// </summary>
        public void Clear()
        {
            foreach (var j in _services.OfType<IDisposable>()) j.Dispose();
            _services.Clear();
        }

        /// <summary>
        /// Descarga todos los servicios cargados de este host, desechando de
        /// manera asíncrona aquellos que implementen la interfaz
        /// <see cref="IAsyncDisposable"/>.
        /// </summary>
        public async Task ClearAsync()
        {
            var l = Enumerable.OfType<IAsyncDisposable>(_services).ToArray();
            await Task.WhenAll(l.Select(p => p.DisposeAsync().AsTask())).ConfigureAwait(false);
            foreach (var j in l.Cast<IService>()) _services.Remove(j);
            Clear();
        }

        /// <summary>
        /// Determina si este Host aloja al servicio especificado.
        /// </summary>
        /// <param name="service">Servicio a buscar.</param>
        /// <returns>
        /// <see langword="true"/> si este Host aloja al servicio especificado,
        /// <see langword="false"/> en caso contrario.
        /// </returns>
        public bool Contains(IService service) => _services.Contains(service);

        /// <summary>
        /// Determina si este Host aloja un servicio del tipo especificado.
        /// </summary>
        /// <typeparam name="T">Tipo de servicio a buscar.</typeparam>
        /// <returns>
        /// <see langword="true"/> si este Host aloja a un servicio del tipo
        /// especificado, <see langword="false"/> en caso contrario.
        /// </returns>
        public bool ConstainsOf<T>() where T : IService
        {
            return _services.Any(p => p.GetType().Implements<T>());
        }

        /// <summary>
        /// Copia la referencia a todos los servicios alojados en este Host a
        /// un arreglo, empezando desde el índice cero del mismo.
        /// </summary>
        /// <param name="array">Arreglo de destino de la copia.</param>
        public void CopyTo(IService[] array) => _services.CopyTo(array);

        /// <summary>
        /// Copia la referencia a todos los servicios alojados en este Host a
        /// un arreglo, especificando el índice en el cual comenzar la copia.
        /// </summary>
        /// <param name="array">Arreglo de destino de la copia.</param>
        /// <param name="arrayIndex">Índice desde el cual empezar a copiar las
        /// referencias de instancia.</param>
        public void CopyTo(IService[] array, int arrayIndex) => _services.CopyTo(array, arrayIndex);

        /// <summary>
        /// Quita una instancia activa de servicio de este Host.
        /// </summary>
        /// <param name="item">Instancia del servicio a quitar.</param>
        /// <returns>
        /// <see langword="true"/> si la instancia ha sido quitada
        /// exitosamente; <see langword="false"/> en caso contrario, por
        /// ejemplo si la misma no existía en esta colección.
        /// </returns>
        public bool Remove(IService item)
        {
            bool retval;
            if ((retval = _services.Remove(item)) && item is IDisposable i) i.Dispose();
            return retval;
        }

        /// <summary>
        /// Quita una instancia activa de servicio de este Host, desechándola
        /// de forma asíncrona.
        /// </summary>
        /// <param name="item">Instancia del servicio a quitar.</param>
        /// <returns></returns>
        public async Task<bool> RemoveAsync(IService item)
        {
            bool retval;
            if (retval = _services.Remove(item))
            {
                switch (item)
                {
                    case IDisposable i:
                        i.Dispose();
                        break;
                    case IAsyncDisposable a:
                        await a.DisposeAsync().ConfigureAwait(false);
                        break;
                }
            }
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
            return this.FirstOf<T>() ?? throw Errors.MissingService<T>();
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
                return this.FirstOf(type ?? throw new ArgumentNullException(nameof(type))) ?? throw new MissingServiceException(type);
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

        /// <summary>
        /// Obtiene un enumerador, el cual permite recorrer todos los elementos
        /// de esta colección.
        /// </summary>
        /// <returns>
        /// Un enumerador para esta colección de servicios.
        /// </returns>
        public IEnumerator<IService> GetEnumerator()
        {
            return ((IEnumerable<IService>)_services).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_services).GetEnumerator();
        }
    }
}
