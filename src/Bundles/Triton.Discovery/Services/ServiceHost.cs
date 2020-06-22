using System;
using System.Collections.Generic;
using System.Linq;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Services.Base;
using TheXDS.MCART;

namespace TheXDS.Triton.Services
{
    /// <summary>
    /// Extensiones de descubrimiento de servicios.
    /// </summary>
    public static class DiscoveryExtensions
    {
        /// <summary>
        /// Descubre servicios de Tritón utilizando los orígenes especificados.
        /// </summary>
        /// <param name="host">
        /// Host de servicios en el cual cargar los servicios descubiertos.
        /// </param>
        /// <param name="sources">
        /// Orígenes de servicios a utilizar para descubrir servicios.
        /// </param>
        public static void Discover(this ServiceHost host, params IServiceSource[] sources)
        {
            Discover(host, sources.AsEnumerable());
        }

        /// <summary>
        /// Descubre servicios de Tritón utilizando los orígenes especificados.
        /// </summary>
        /// <param name="host">
        /// Host de servicios en el cual cargar los servicios descubiertos.
        /// </param>
        /// <param name="sources">
        /// Orígenes de servicios a utilizar para descubrir servicios.
        /// </param>
        public static void Discover(this ServiceHost host, IEnumerable<IServiceSource> sources)
        {
            foreach (var j in sources)
            {
                host.AddRange(j.GetServices());
            }
        }

        /// <summary>
        /// Descubre servicios de Tritón utilizando los orígenes especificados
        /// y los agrega a un nuevo host de servicios.
        /// </summary>
        /// <param name="sources">
        /// Orígenes de servicios a utilizar para descubrir servicios.
        /// </param>
        /// <returns>
        /// Un nuevo <see cref="ServiceHost"/> que contiene todos los servicios
        /// descubiertos.
        /// </returns>
        public static ServiceHost Discover(IEnumerable<IServiceSource> sources)
        {
            var h = new ServiceHost();
            h.Discover(sources);
            return h;
        }
    }

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

    public class ReflectionServiceSource : IServiceSource
    {
        public IEnumerable<IService> GetServices()
        {
            return Objects.FindAllObjects<IService>();
        }
    }
}
