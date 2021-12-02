using System;
using System.Runtime.CompilerServices;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Exceptions;
using TheXDS.MCART.Helpers;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Services
{
    /// <summary>
    /// Clase base para implementación simple de servicios. Permite establecer
    /// o descubrir la configuración de transacción a utilizar.
    /// </summary>
    public class Service : IService
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T FindT<T>() where T : class => Objects.FindFirstObject<T>() ?? throw new MissingTypeException(typeof(T));

        /// <summary>
        /// Inicializa una nueva instancia de la clase 
        /// <see cref="Service"/>, buscando automáticamente la
        /// configuración de transacciones a utilizar.
        /// </summary>
        public Service() : this(FindT<ITransactionFactory>())
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase 
        /// <see cref="Service"/>, buscando automáticamente la
        /// configuración de transacciones a utilizar.
        /// </summary>
        /// <param name="factory">Fábrica de transacciones a utilizar.</param>
        public Service(ITransactionFactory factory) : this(new TransactionConfiguration(), factory)
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
        public Service(TransactionConfiguration transactionConfiguration) : this(transactionConfiguration, FindT<ITransactionFactory>())
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
        public Service(TransactionConfiguration transactionConfiguration, ITransactionFactory factory)
        {
            Configuration = transactionConfiguration ?? throw new ArgumentNullException(nameof(transactionConfiguration));
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <summary>
        /// Obtiene la configuración predeterminada a utilizar al crear
        /// transacciones.
        /// </summary>
        public TransactionConfiguration Configuration { get; }

        /// <summary>
        /// Obtiene una referencia a la fábrica de transacciones a utilizar por
        /// el servicio.
        /// </summary>
        public ITransactionFactory Factory { get; }

        /// <summary>
        /// Obtiene una transacción que permite leer información de la base
        /// de datos.
        /// </summary>
        /// <returns>
        /// Una transacción que permite leer información de la base de 
        /// datos.
        /// </returns>
        public ICrudReadTransaction GetReadTransaction() => Factory.GetReadTransaction(Configuration);

        /// <summary>
        /// Obtiene una transacción que permite escribir información en la
        /// base de datos.
        /// </summary>
        /// <returns>
        /// Una transacción que permite escribir información en la base de
        /// datos.
        /// </returns>
        public ICrudWriteTransaction GetWriteTransaction() => Factory.GetWriteTransaction(Configuration);

        /// <summary>
        /// Obtiene una transacción que permite leer y escribir información
        /// en la base de datos.
        /// </summary>
        /// <returns>
        /// Una transacción que permite leer y escribir información en la
        /// base de datos.
        /// </returns>
        public ICrudReadWriteTransaction GetTransaction() => Factory.GetTransaction(Configuration);

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
