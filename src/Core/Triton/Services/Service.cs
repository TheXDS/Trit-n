using Microsoft.EntityFrameworkCore;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Services
{
    /// <summary>
    ///     Expone un servicio con funcionalidad estándar para gestionar
    ///     contextos de datos de Entity Framework Core por medio de
    ///     transacciones y operaciones CRUD.
    /// </summary>
    /// <typeparam name="T">
    ///     Tipo de contexto de datos a administrar.
    /// </typeparam>
    public class Service<T> : ServiceBase<IServiceConfiguration, ICrudTransactionFactory, T>, IService where T : DbContext, new()
    {
        /// <summary>
        ///     Inicializa una nueva instancia de la clase 
        ///     <see cref="Service{T}"/>, buscando automáticamente la
        ///     configuración a utilizar.
        /// </summary>
        public Service() : base()
        {
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase 
        ///     <see cref="Service{T}"/>, especificando la configuración a
        ///     utilizar.
        /// </summary>
        /// <param name="settings">
        ///     Configuración a utilizar para este servicio.
        /// </param>
        public Service(IServiceConfiguration settings) : base(settings)
        {
        }

        /// <summary>
        ///     Obtiene una transacción para lectura y escritura de datos.
        /// </summary>
        /// <returns>
        ///     Una transacción para lectura y escritura de datos.
        /// </returns>
        public ICrudReadWriteTransaction GetReadWriteTransaction()
        {
            return ActiveSettings.CrudTransactionFactory.ManufactureReadWriteTransaction<T>(ActiveSettings.ConnectionConfiguration);
        }
    }
}