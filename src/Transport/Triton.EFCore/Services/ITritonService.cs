namespace TheXDS.Triton.Services;

/// <summary>
/// Define una serie de miembros a implementar por un tipo que exponga acceso a
/// un contexto de datos de Entity Framework.
/// </summary>
/// <typeparam name="T">
/// Tipo de <see cref="DbContext"/> al cual este servicio ofrece acceso.
/// </typeparam>
public interface ITritonService<out T> : ITritonService where T : DbContext
{
}