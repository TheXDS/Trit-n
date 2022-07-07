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
    public class TritonService : ITritonService
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T FindT<T>() where T : class => Objects.FindFirstObject<T>() ?? throw new MissingTypeException(typeof(T));

        /// <summary>
        /// Inicializa una nueva instancia de la clase 
        /// <see cref="TritonService"/>, buscando automáticamente la
        /// configuración de transacciones a utilizar.
        /// </summary>
        public TritonService() : this(FindT<ITransactionFactory>())
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase 
        /// <see cref="TritonService"/>, buscando automáticamente la
        /// configuración de transacciones a utilizar.
        /// </summary>
        /// <param name="factory">Fábrica de transacciones a utilizar.</param>
        public TritonService(ITransactionFactory factory) : this(new TransactionConfiguration(), factory)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase 
        /// <see cref="TritonService"/>, buscando automáticamente la
        /// configuración de transacciones a utilizar.
        /// </summary>
        /// <param name="transactionConfiguration">
        /// Configuración a utilizar para las transacciones generadas por este
        /// servicio.
        /// </param>
        public TritonService(IMiddlewareConfigurator transactionConfiguration) : this(transactionConfiguration, FindT<ITransactionFactory>())
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase 
        /// <see cref="TritonService"/>, especificando la configuración a utilizar.
        /// </summary>
        /// <param name="transactionConfiguration">
        /// Configuración a utilizar para las transacciones generadas por este
        /// servicio.
        /// </param>
        /// <param name="factory">Fábrica de transacciones a utilizar.</param>
        public TritonService(IMiddlewareConfigurator transactionConfiguration, ITransactionFactory factory)
        {
            Configuration = transactionConfiguration ?? throw new ArgumentNullException(nameof(transactionConfiguration));
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <summary>
        /// Obtiene la configuración predeterminada a utilizar al crear
        /// transacciones.
        /// </summary>
        public IMiddlewareConfigurator Configuration { get; }

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
        public ICrudReadTransaction GetReadTransaction() => Factory.GetReadTransaction(Configuration.GetRunner());

        /// <summary>
        /// Obtiene una transacción que permite escribir información en la
        /// base de datos.
        /// </summary>
        /// <returns>
        /// Una transacción que permite escribir información en la base de
        /// datos.
        /// </returns>
        public ICrudWriteTransaction GetWriteTransaction() => Factory.GetWriteTransaction(Configuration.GetRunner());

        /// <summary>
        /// Obtiene una transacción que permite leer y escribir información
        /// en la base de datos.
        /// </summary>
        /// <returns>
        /// Una transacción que permite leer y escribir información en la
        /// base de datos.
        /// </returns>
        public ICrudReadWriteTransaction GetTransaction() => Factory.GetTransaction(Configuration.GetRunner());

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
