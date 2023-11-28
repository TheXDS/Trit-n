namespace TheXDS.Triton.Services;

/// <summary>
/// Implementa un <see cref="IDbContextOptionsSource"/> para configuraciones
/// que no requieren saber con precisión el tipo del contexto a configurar.
/// </summary>
public class DbContextOptionsSource : DbContextOptionsSourceBase<DbContextOptionsBuilder, DbContextOptions>
{
    /// <summary>
    /// Obtiene una instancia de tipo <see cref="IDbContextOptionsSource"/> que
    /// representa un origen sin configuración, es decir, se utilizará un
    /// constructor sin parámetros para inicializar los objetos de tipo
    /// <see cref="DbContext"/>.
    /// </summary>
    public static IDbContextOptionsSource None => new DbContextOptionsSource();

    private DbContextOptionsSource()
    { }

    /// <summary>
    /// Inicializa una nueva instacia de la clase
    /// <see cref="DbContextOptionsSource"/>.
    /// </summary>
    /// <param name="options">
    /// Instancia estática de opciones a utilizar para instanciar un
    /// <see cref="DbContext"/>.
    /// </param>
    public DbContextOptionsSource(DbContextOptions options) : base(options)
    {
    }

    /// <summary>
    /// Inicializa una nueva instacia de la clase
    /// <see cref="DbContextOptionsSource"/>.
    /// </summary>
    /// <param name="builder">
    /// Método a llamar para configurar las opciones a utilizar para instanciar
    /// objetos de tipo <see cref="DbContext"/>.
    /// </param>
    public DbContextOptionsSource(Action<DbContextOptionsBuilder> builder) : base(builder)
    {
    }
}
