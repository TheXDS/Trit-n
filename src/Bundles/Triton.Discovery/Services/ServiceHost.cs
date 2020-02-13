using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TheXDS.Triton.Services.Base;
using TheXDS.MCART;
using TheXDS.MCART.Comparison;
using System.Diagnostics.CodeAnalysis;

namespace TheXDS.Triton.Services
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que permita
    /// instanciar servicios de Tritón de acuerdo a un criterio de exploración
    /// personalizado.
    /// </summary>
    public interface IServiceSource
    {
        /// <summary>
        /// Instancia y obtiene una colección de servicios por medio de este objeto.
        /// </summary>
        /// <returns>
        /// Una colección de servicios instanciados por medio de este objeto.
        /// </returns>
        IEnumerable<IService> GetServices();
    }

    /// <summary>
    /// Aloja una colección de servicios de Tritón.
    /// </summary>
    public class ServiceHost : ICollection<IService>
    {
        /// <summary>
        /// Inicializa un <see cref="ServiceHost"/> realizando una exploración
        /// de descubrimiento de clases de servicio instanciables y/o clases de
        /// contexto de datos.
        /// </summary>
        /// <param name="sources">
        /// Colección de orígenes de servicios a utilizar para inicializar la
        /// colección de servicios de la instancia a devolver.
        /// </param>
        /// <returns>
        /// Una nueva instancia de la clase <see cref="ServiceHost"/> que
        /// contiene servicios descubiertos por medio de la colección de 
        /// <see cref="IServiceSource"/> provista.
        /// </returns>
        public static ServiceHost Discover(IEnumerable<IServiceSource> sources)
        {
            var host = new ServiceHost();
            host._services.AddRange(sources.SelectMany(p => p.GetServices()).Distinct(new TypeComparer<IService>()));
            return host;
        }

        private readonly List<IService> _services = new List<IService>();

        /// <summary>
        /// Obtiene la cantidad de servicios cargados dentro de este Host.
        /// </summary>
        public int Count => ((ICollection<IService>)_services).Count;

        /// <summary>
        /// Obtiene un valor que indica si esta colección de servicios es de
        /// solo-lectura o no.
        /// </summary>
        public bool IsReadOnly => ((ICollection<IService>)_services).IsReadOnly;

        /// <summary>
        /// Agrega una instancia de servicio a este Host.
        /// </summary>
        /// <param name="item">Servicio activo a agregar a este host.</param>
        public void Add(IService item)
        {
            ((ICollection<IService>)_services).Add(item);
        }

        /// <summary>
        /// Descarga todos los servicios cargados de este host.
        /// </summary>
        public void Clear()
        {
            foreach (var j in _services.OfType<IDisposable>()) j.Dispose(); 
            ((ICollection<IService>)_services).Clear();
        }

        /// <summary>
        /// Comprueba si este host contiene a la instancia activa de servicio
        /// especificada.
        /// </summary>
        /// <param name="item">Instancia de servicio a comprobar.</param>
        /// <returns>
        /// <see langword="true"/> si este objeto contiene la instancia del
        /// servicio especificada, <see langword="false"/> en caso contrario.
        /// </returns>
        public bool Contains(IService item)
        {
            return ((ICollection<IService>)_services).Contains(item);
        }

        /// <summary>
        /// Copia las referencias de instancia de los servicios activos en este
        /// Host a un arreglo.
        /// </summary>
        /// <param name="array">Arreglo de destino.</param>
        /// <param name="arrayIndex">
        /// Índica en el cual empezar la copia.
        /// </param>
        public void CopyTo(IService[] array, int arrayIndex)
        {
            ((ICollection<IService>)_services).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Obtiene el enumerador de esta colección.
        /// </summary>
        /// <returns>El enumerador de esta colección.</returns>
        public IEnumerator<IService> GetEnumerator()
        {
            return ((ICollection<IService>)_services).GetEnumerator();
        }

        /// <summary>
        /// Quita uns intancia activa de servicio de esta instancia.
        /// </summary>
        /// <param name="item">Instancia del servicio a quitar.</param>
        /// <returns>
        /// <see langword="true"/> si la instancia ha sido quitada
        /// exitosamente; <see langword="false"/> en caso contrario, por
        /// ejemplo si la misma no existía en esta colección.
        /// </returns>
        public bool Remove(IService item)
        {            
            return ((ICollection<IService>)_services).Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((ICollection<IService>)_services).GetEnumerator();
        }
    }
}
