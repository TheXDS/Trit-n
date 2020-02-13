using System;
using System.Collections.Generic;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Services
{
    /// <summary>
    /// Host de servicios con registro explícito.
    /// </summary>
    public class ExplicitServiceHost : IServiceHost
    {
        private readonly List<IService> _services = new List<IService>();

        /// <summary>
        /// Enumera los servicios existentes en este Host.
        /// </summary>
        public IEnumerable<IService> Services => _services;

        /// <summary>
        /// Registra una instancia de servicio en este Host.
        /// </summary>
        /// <param name="service">
        /// Instancia de servicio a registrar.
        /// </param>
        /// <returns>
        /// La misma instancia de <see cref="ExplicitServiceHost"/> para
        /// permitir sintaxis Fluent.
        /// </returns>
        public ExplicitServiceHost Register(IService service)
        {
            _services.Add(service ?? throw new ArgumentNullException(nameof(service)));
            return this;
        }
    }
}