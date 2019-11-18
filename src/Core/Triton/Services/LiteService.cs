using Microsoft.EntityFrameworkCore;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Services
{
    /// <summary>
    ///     Clase que permite administrar un <see cref="DbContext"/> por medio
    ///     de transacciones que exponen operaciones Crud.
    /// </summary>
    /// <typeparam name="T">
    ///     Tipo de contexto a administrar. Debe ser instanciable por medio de
    ///     un constructor sin parámetros.
    /// </typeparam>
    public class LiteService<T> : ServiceBase<ILiteServiceConfiguration, ILiteCrudTransactionFactory, T>, ILiteService where T : DbContext, new()
    {
        /// <summary>
        ///     Inicializa una nueva instancia de la clase 
        ///     <see cref="LiteService{T}"/>, buscando automáticamente la
        ///     configuración a utilizar.
        /// </summary>
        public LiteService() : base()
        {
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase 
        ///     <see cref="LiteService{T}"/>, especificando la configuración a 
        ///     utilizar.
        /// </summary>
        /// <param name="settings">
        ///     Configuración a utilizar para este servicio.
        /// </param>
        public LiteService(ILiteServiceConfiguration settings) : base(settings)
        {
        }
    }
}