using Microsoft.EntityFrameworkCore;
using System;
using TheXDS.MCART;
using TheXDS.MCART.Exceptions;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Services
{
    /// <summary>
    ///     Clase que describe la implementación mínima de servicios que Tritón
    ///     expone para administrar un contexto de datos.
    /// </summary>
    /// <typeparam name="TContext">
    ///     Tipo de contexto de datos a instanciar.
    /// </typeparam>
    public sealed class SimpleService<TContext> : IService where TContext : DbContext, new()
    {
        /// <summary>
        ///     Obtiene una referencia al tipo de contexto para el cual este
        ///     servicio generará transacciones.
        /// </summary>
        public Type ContextType => typeof(TContext);

        /// <summary>
        ///     Obtiene una referencia a la configuración activa para este
        ///     servicio.
        /// </summary>
        public IServiceConfiguration Configuration { get; } = Objects.FindFirstObject<IServiceConfiguration>() ?? throw new MissingTypeException(typeof(IServiceConfiguration));

        /// <summary>
        ///     Obtiene una transacción para lectura y escritura de datos.
        /// </summary>
        /// <returns>
        ///     Una transacción para lectura y escritura de datos.
        /// </returns>
        public ICrudReadWriteTransaction GetReadWriteTransaction()
        {
            return Configuration.CrudTransactionFactory.ManufactureReadWriteTransaction<TContext>(Configuration.TransactionConfiguration);
        }
    }
}
