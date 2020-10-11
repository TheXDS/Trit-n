using System.Collections.Generic;
using TheXDS.Triton.Services.Base;

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
}
