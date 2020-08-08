using System;
using System.Collections.Generic;
using TheXDS.Triton.Services.Base;
using TheXDS.MCART;

namespace TheXDS.Triton.Services
{
    /// <summary>
    /// Orígen de servicios que descubre y enumera todos los servicios
    /// definidos en el <see cref="AppDomain"/> actual.
    /// </summary>
    public class ReflectionServiceSource : IServiceSource
    {
        IEnumerable<IService> IServiceSource.GetServices()
        {
            return Objects.FindAllObjects<IService>();
        }
    }
}
