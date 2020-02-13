using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TheXDS.MCART;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Exceptions;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Services
{
    /// <summary>
    /// Clase base para todos los servicios. Provee de la funcionalidad
    /// básica de instanciación de contexto de datos y provee acceso a la 
    /// configuración del servicio.
    /// </summary>
    /// <typeparam name="TContext">
    /// Tipo de contexto de datos a instanciar.
    /// </typeparam>
    public abstract class Service<TContext> : IService where TContext : DbContext, new()
    {
        /// <summary>
        /// Obtiene una referencia al tipo de contexto para el cual este
        /// servicio generará transacciones.
        /// </summary>
        public Type ContextType => typeof(TContext);

        /// <summary>
        /// Obtiene una referencia a la configuración activa para este
        /// servicio.
        /// </summary>
        public IServiceConfiguration Configuration { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase 
        /// <see cref="Service{TContext}"/>, buscando automáticamente la
        /// configuración a utilizar.
        /// </summary>
        protected Service() : this(Objects.FindFirstObject<IServiceConfiguration>() ?? throw new MissingTypeException(typeof(IServiceConfiguration)))
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase 
        /// <see cref="Service{TContext}"/>, especificando la configuración
        /// a utilizar.
        /// </summary>
        /// <param name="settings">
        /// Configuración a utilizar para este servicio.
        /// </param>
        protected Service(IServiceConfiguration settings)
        {
            if (settings is null) throw new ArgumentNullException(nameof(settings));
            Configuration = settings;
        }

        /// <summary>
        /// Obtiene una transacción para lectura de datos.
        /// </summary>
        /// <returns>
        /// Una transacción para lectura de datos.
        /// </returns>
        public ICrudReadTransaction GetReadTransaction()
        {
            return Configuration.CrudTransactionFactory.ManufactureReadTransaction<TContext>(Configuration.TransactionConfiguration);
        }

        /// <summary>
        /// Obtiene una transacción para escritura de datos.
        /// </summary>
        /// <returns>
        /// Una transacción para escritura de datos.
        /// </returns>
        public ICrudWriteTransaction GetWriteTransaction()
        {
            return Configuration.CrudTransactionFactory.ManufactureWriteTransaction<TContext>(Configuration.TransactionConfiguration);
        }

        /// <summary>
        /// Obtiene una transacción para lectura y escritura de datos.
        /// </summary>
        /// <returns>
        /// Una transacción para lectura y escritura de datos.
        /// </returns>
        public ICrudReadWriteTransaction<TContext> GetReadWriteTransaction()
        {
            return Configuration.CrudTransactionFactory.ManufactureReadWriteTransaction<TContext>(Configuration.TransactionConfiguration);
        }

        ICrudReadWriteTransaction IService.GetReadWriteTransaction() => GetReadWriteTransaction();

        /// <summary>
        /// Ejecuta una operación en el contexto de una transacción de lectura.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo devuelvo por la operación de lectura.
        /// </typeparam>
        /// <param name="action">
        /// Acción a ejecutar dentro de la transacción de lectura.
        /// </param>
        /// <returns>
        /// El resultado de la operación de lectura.
        /// </returns>
        [Sugar] 
        protected T WithReadTransaction<T>(Func<ICrudReadTransaction, T> action)
        {
            var t = GetReadTransaction();
            try
            {
                return action(t);
            }
            finally
            {
                if (!t.IsDisposed) t.Dispose();
            }
        }
    }
}
