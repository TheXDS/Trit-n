using Microsoft.EntityFrameworkCore;

namespace TheXDS.Triton.Services;

/// <summary>
/// Servicio que incluye uina referencia fuertemente tipeada al
/// <see cref="DbContext"/> gestionado por el mismo.
/// </summary>
/// <typeparam name="T">
/// Tipo de <see cref="DbContext"/> a ser gestionado por este servicio.
/// </typeparam>
public class TritonService<T> : TritonService where T: DbContext, new()
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="TritonService{T}"/>.
    /// </summary>
    public TritonService() : base(new EfCoreTransFactory<T>())
    {
    }
}