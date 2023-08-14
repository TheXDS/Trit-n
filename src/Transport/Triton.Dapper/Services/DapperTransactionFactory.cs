using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Dapper.Services;

/// <summary>
/// Implementa un <see cref="ITransactionFactory"/> que crea y administra
/// operaciones de datos utilizando la librería Dapper.
/// </summary>
public class DapperTransactionFactory : ITransactionFactory
{
    private readonly IDbConnectionFactory _factory;
    private readonly IDictionary<Type, DapperModelDescriptor> _modelOverrides;

    /// <summary>
    /// Define un delegado que permite configurar un diccionario de sustitución
    /// de metadatos para uno o más modelos.
    /// </summary>
    /// <param name="overridesDictionary">
    /// Diccionario de sustituciones de metadatos para cada modelo.
    /// </param>
    public delegate void ModelOverrideConfigurator(IDictionary<Type, DapperModelDescriptor> overridesDictionary);
    
    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="DapperTransactionFactory"/>.
    /// </summary>
    /// <param name="factory">
    /// Fábrica de conexiones a bases de datos a utilzar.
    /// </param>
    public DapperTransactionFactory(IDbConnectionFactory factory)
    {
        _factory = factory;
        _modelOverrides = new Dictionary<Type, DapperModelDescriptor>();
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="DapperTransactionFactory"/>.
    /// </summary>
    /// <param name="factory">
    /// Fábrica de conexiones a bases de datos a utilzar.
    /// </param>
    /// <param name="modelOverrideConfigurator">
    /// Delegado de configuración de las sustituciones de metadatos de los
    /// modelos.
    /// </param>
    public DapperTransactionFactory(IDbConnectionFactory factory, ModelOverrideConfigurator? modelOverrideConfigurator)
        : this(factory)
    {
        modelOverrideConfigurator?.Invoke(_modelOverrides);
    }

    /// <summary>
    /// Obtiene un diccionario de sustituciones aplicar al procesar modelos de datos.
    /// </summary>
    public IDictionary<Type, DapperModelDescriptor> ModelOverrides => _modelOverrides;

    /// <inheritdoc/>
    public ICrudReadWriteTransaction GetTransaction(IMiddlewareConfigurator configuration)
    {
        return new DapperTransaction(_factory.OpenConnection(), _modelOverrides, configuration.GetRunner());
    }
}
