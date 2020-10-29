using System;
using System.Runtime.CompilerServices;
using TheXDS.MCART;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Exceptions;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Services
{
    /// <summary>
    /// Clase base para implementación simple de servicios. Permite establecer
    /// o descubrir la configuración de transacción a utilizar.
    /// </summary>
    public abstract class Service : IService
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T FindT<T>() where T : class => Objects.FindFirstObject<T>() ?? throw new MissingTypeException(typeof(T));

        /// <summary>
        /// Inicializa una nueva instancia de la clase 
        /// <see cref="Service"/>, buscando automáticamente la
        /// configuración de transacciones a utilizar.
        /// </summary>
        protected Service() : this(FindT<ITransactionFactory>())
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase 
        /// <see cref="Service"/>, buscando automáticamente la
        /// configuración de transacciones a utilizar.
        /// </summary>
        /// <param name="factory">Fábrica de transacciones a utilizar.</param>
        protected Service(ITransactionFactory factory) : this(new TransactionConfiguration(), factory)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase 
        /// <see cref="Service"/>, buscando automáticamente la
        /// configuración de transacciones a utilizar.
        /// </summary>
        /// <param name="transactionConfiguration">
        /// Configuración a utilizar para las transacciones generadas por este
        /// servicio.
        /// </param>
        protected Service(TransactionConfiguration transactionConfiguration) : this(transactionConfiguration, FindT<ITransactionFactory>())
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase 
        /// <see cref="Service"/>, especificando la configuración a utilizar.
        /// </summary>
        /// <param name="transactionConfiguration">
        /// Configuración a utilizar para las transacciones generadas por este
        /// servicio.
        /// </param>
        /// <param name="factory">Fábrica de transacciones a utilizar.</param>
        protected Service(TransactionConfiguration transactionConfiguration, ITransactionFactory factory)
        {
            Configuration = transactionConfiguration ?? throw new ArgumentNullException(nameof(transactionConfiguration));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <summary>
        /// Obtiene la configuración predeterminada a utilizar al crear
        /// transacciones.
        /// </summary>
        public TransactionConfiguration Configuration { get; }

        /// <summary>
        /// Obtiene la fábrica de transacciones a utilizar por el servicio.
        /// </summary>
        private readonly ITransactionFactory _factory;

        ICrudReadTransaction IService.GetReadTransaction() => _factory.GetReadTransaction(Configuration);
        ICrudWriteTransaction IService.GetWriteTransaction() => _factory.GetWriteTransaction(Configuration);
        ICrudReadWriteTransaction IService.GetTransaction() => _factory.GetTransaction(Configuration);


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
            var t = ((IService)this).GetReadTransaction();
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
