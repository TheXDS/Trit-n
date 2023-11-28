namespace TheXDS.Triton.Services;

/// <summary>
/// Implementa un <see cref="IDbContextOptionsSource"/> para configuraciones
/// que requieren saber con precisión el tipo del contexto a configurar.
/// </summary>
public class DbContextOptionsSource<T> : DbContextOptionsSourceBase<DbContextOptionsBuilder<T>, DbContextOptions<T>> where T : DbContext
{
    /// <summary>
    /// Inicializa una nueva instacia de la clase
    /// <see cref="DbContextOptionsSource"/>.
    /// </summary>
    /// <param name="options">
    /// Instancia estática de opciones a utilizar para instanciar un
    /// <see cref="DbContext"/>.
    /// </param>
    public DbContextOptionsSource(DbContextOptions<T> options) : base(options)
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
    public DbContextOptionsSource(Action<DbContextOptionsBuilder<T>> builder) : base(builder)
    {
    }
}