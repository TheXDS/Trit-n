using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Services;

/// <summary>
/// Clase base para los servicios de Tritón que permitan acceso a un
/// contexto de datos con información de autenticación y permisos de
/// usuarios.
/// </summary>
public abstract class UserServiceBase : TritonService, IUserService
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase 
    /// <see cref="UserServiceBase"/>, buscando automáticamente la
    /// configuración de transacciones a utilizar.
    /// </summary>
    public UserServiceBase() : base()
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase 
    /// <see cref="UserServiceBase"/>, buscando automáticamente la
    /// configuración de transacciones a utilizar.
    /// </summary>
    /// <param name="factory">Fábrica de transacciones a utilizar.</param>
    public UserServiceBase(ITransactionFactory factory) : base(factory)
    {
    }
    /// <summary>
    /// Inicializa una nueva instancia de la clase 
    /// <see cref="UserServiceBase"/>, buscando automáticamente la
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
    /// <see cref="UserServiceBase"/>, especificando la configuración a
    /// utilizar.
    /// </summary>
    /// <param name="transactionConfiguration">
    /// Configuración a utilizar para las transacciones generadas por este
    /// servicio.
    /// </param>
    /// <param name="factory">Fábrica de transacciones a utilizar.</param>
    protected UserServiceBase(IMiddlewareConfigurator transactionConfiguration, ITransactionFactory factory)
        : base(transactionConfiguration, factory)
    {
    }
}
