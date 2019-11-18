using Microsoft.EntityFrameworkCore;
using System;
using TheXDS.MCART;
using TheXDS.MCART.Exceptions;

namespace TheXDS.Triton.Services.Base
{
    /// <summary>
    ///     Clase base para todos los servicios. Provee de la funcionalidad
    ///     básica de instanciación de contexto de datos y provee acceso a la 
    ///     configuración del servicio.
    /// </summary>
    /// <typeparam name="TConfiguration">
    ///     Tipo de configuración a exponer para la clase derivada del
    ///     servicio.
    /// </typeparam>
    /// <typeparam name="TFactory">
    ///     Tipo de fábrica de transacciones que la clase derivada deberá
    ///     exponer.
    /// </typeparam>
    /// <typeparam name="TContext">
    ///     Tipo de contexto de datos a instanciar.
    /// </typeparam>
    public abstract class ServiceBase<TConfiguration, TFactory, TContext>: IServiceBase<TConfiguration> where TConfiguration : class, IServiceConfigurationBase<TFactory> where TContext : DbContext, new() where TFactory : ILiteCrudTransactionFactory
    {
        /// <summary>
        ///     Obtiene una referencia a la configuración activa para este servicio.
        /// </summary>
        public TConfiguration ActiveSettings { get; }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase 
        ///     <see cref="ServiceBase{TConfiguration, TFactory, TContext}"/>,
        ///     buscando automáticamente la configuración a utilizar.
        /// </summary>
        protected ServiceBase() : this(Objects.FindFirstObject<TConfiguration>() ?? throw new MissingTypeException(typeof(TConfiguration)))
        {
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase 
        ///     <see cref="ServiceBase{TConfiguration, TFactory, TContext}"/>,
        ///     especificando la configuración a utilizar.
        /// </summary>
        /// <param name="settings">
        ///     Configuración a utilizar para este servicio.
        /// </param>
        protected ServiceBase(TConfiguration settings)
        {
            if (settings is null) throw new ArgumentNullException(nameof(settings));
            ActiveSettings = settings;
        }

        /// <summary>
        ///     Obtiene una transacción para lectura de datos.
        /// </summary>
        /// <returns>
        ///     Una transacción para lectura de datos.
        /// </returns>
        public ICrudReadTransaction GetReadTransaction()
        {
            return ActiveSettings.CrudTransactionFactory.ManufactureReadTransaction<TContext>(ActiveSettings.ConnectionConfiguration);
        }

        /// <summary>
        ///     Obtiene una transacción para escritura de datos.
        /// </summary>
        /// <returns>
        ///     Una transacción para escritura de datos.
        /// </returns>
        public ICrudWriteTransaction GetWriteTransaction()
        {
            return ActiveSettings.CrudTransactionFactory.ManufactureWriteTransaction<TContext>(ActiveSettings.ConnectionConfiguration);
        }
    }
}
