namespace TheXDS.Triton.Services;

/// <summary>
/// Clase base que define un objeto que obtiene la configuración a utilizar
/// para instanciar un <see cref="DbContext"/> que acepte recibir un parámetro
/// de tipo <see cref="DbContextOptions"/>.
/// </summary>
/// <typeparam name="TBuilder">
/// Tipo de objeto a utilizar para construir la configuración para instanciar
/// un <see cref="DbContext"/>.
/// </typeparam>
/// <typeparam name="TOptions">
/// Tipo de objeto que contiene la configuración a utilizar para instanciar un
/// <see cref="DbContext"/>.
/// </typeparam>
public abstract class DbContextOptionsSourceBase<TBuilder, TOptions> : IDbContextOptionsSource where TBuilder : DbContextOptionsBuilder, new() where TOptions : DbContextOptions
{
    private readonly Action<TBuilder>? builderCallback;
    private readonly TOptions? options;

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="DbContextOptionsSourceBase{TBuilder, TOptions}"/>
    /// </summary>
    /// <param name="builderCallback">
    /// Método a llamar para configurar un
    /// <see cref="DbContextOptionsBuilder"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Se produce si <see cref="builderCallback"/> es <see langword="null"/>.
    /// </exception>
    protected DbContextOptionsSourceBase(Action<TBuilder> builderCallback)
    {
        this.builderCallback = builderCallback ?? throw new ArgumentNullException(nameof(builderCallback));
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="DbContextOptionsSourceBase{TBuilder, TOptions}"/>
    /// </summary>
    /// <param name="options">
    /// Objeto de opciones estáticas a devolver cuando se deba instanciar un
    /// <see cref="DbContext"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Se produce si <see cref="options"/> es <see langword="null"/>.
    /// </exception>
    protected DbContextOptionsSourceBase(TOptions options)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase 
    /// <see cref="DbContextOptionsSourceBase{TBuilder, TOptions}"/>,
    /// explícitamente omitiendo especificar tanto un método de configuración
    /// como la configuración estática.
    /// </summary>
    protected DbContextOptionsSourceBase()
    {
    }

    DbContextOptions? IDbContextOptionsSource.GetOptions()
    {
        if (options is not null) return options;
        if (builderCallback is null) return null;
        var b = new TBuilder();
        builderCallback.Invoke(b);
        return b.Options;
    }
}
