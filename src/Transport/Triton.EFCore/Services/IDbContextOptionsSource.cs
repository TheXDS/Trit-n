namespace TheXDS.Triton.Services;

/// <summary>
/// Define una serie de miembros a implementar por un tipo que permita obtener
/// la configuración a utilizar cuando se instancia un <see cref="DbContext"/>
/// que acepte recibir un parámetro de tipo <see cref="DbContextOptions"/>.
/// </summary>
public interface IDbContextOptionsSource
{
    /// <summary>
    /// Obtiene la configuración a utilizar para crear una nueva instancia de
    /// la clase <see cref="DbContext"/>.
    /// </summary>
    /// <returns>
    /// Un <see cref="DbContextOptions"/> con la configuración a utilizar
    /// cuando se instancia un<see cref="DbContext"/>, o <see langword="null"/>
    /// cuando se debe llamar al constructor sin parámetros del
    /// <see cref="DbContext"/> a instanciar.
    /// </returns>
    DbContextOptions? GetOptions();
}
