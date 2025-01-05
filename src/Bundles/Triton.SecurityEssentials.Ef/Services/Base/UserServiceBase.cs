using Microsoft.EntityFrameworkCore;
using TheXDS.Triton.SecurityEssentials.Ef.Models;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.SecurityEssentials.Ef.Services.Base;

/// <summary>
/// Clase base para los servicios de Tritón que permitan acceso a un
/// contexto de datos con información de autenticación y permisos de
/// usuarios.
/// </summary>
/// <typeparam name="T">
/// Tipo de <see cref="DbContext"/> que contiene los
/// <see cref="DbSet{TEntity}"/> necesarios para implementar autenticación
/// y permisos de usuarios.
/// </typeparam>
public abstract class UserServiceBase<T> : UserServiceBase, IEfUserService<T> where T : DbContext, IUserDbContext, new()
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase 
    /// <see cref="UserServiceBase{T}"/>, buscando automáticamente la
    /// configuración de transacciones a utilizar.
    /// </summary>
    public UserServiceBase() : base()
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase 
    /// <see cref="UserServiceBase{T}"/>, buscando automáticamente la
    /// configuración de transacciones a utilizar.
    /// </summary>
    /// <param name="factory">Fábrica de transacciones a utilizar.</param>
    public UserServiceBase(ITransactionFactory factory) : base(factory)
    {
    }
    /// <summary>
    /// Inicializa una nueva instancia de la clase 
    /// <see cref="UserServiceBase{T}"/>, buscando automáticamente la
    /// configuración de transacciones a utilizar.
    /// </summary>
    /// <param name="transactionConfiguration">
    /// Configuración a utilizar para las transacciones generadas por este
    /// servicio.
    /// </param>
    public UserServiceBase(IMiddlewareConfigurator transactionConfiguration) : base(transactionConfiguration)
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase 
    /// <see cref="UserServiceBase{T}"/>, especificando la configuración a
    /// utilizar.
    /// </summary>
    /// <param name="transactionConfiguration">
    /// Configuración a utilizar para las transacciones generadas por este
    /// servicio.
    /// </param>
    /// <param name="factory">Fábrica de transacciones a utilizar.</param>
    protected UserServiceBase(IMiddlewareConfigurator transactionConfiguration, EfCoreTransFactory<T> factory)
        : base(transactionConfiguration, factory)
    {
    }
}
