using Microsoft.EntityFrameworkCore;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;

namespace TheXDS.ServicePool.Triton;

/// <summary>
/// Contiene métodos de extensión que permiten configurar Tritón para
/// utilizarse en conjunto con <see cref="ServicePool"/> y Entity Framework.
/// </summary>
public static class ServicePoolEfExtensions
{
    /// <summary>
    /// Resuelve una instancia de un servicio que puede utilizarse para 
    /// acceder a la base de datos solicitada.
    /// </summary>
    /// <typeparam name="T">
    /// Tipo de contexto de datos a utilizar.
    /// </typeparam>
    /// <param name="pool">
    /// <see cref="ServicePool"/> a configurar.
    /// </param>
    /// <returns>
    /// Una instancia de <see cref="ITritonService"/> que permite acceder al 
    /// contexto de datos solicitado.
    /// </returns>
    /// <remarks>
    /// Prefiera resolver directamente un <see cref="ITritonService"/> si
    /// necesita acceder directamente a la funcionalidad de un servicio
    /// concreto.
    /// </remarks>
    public static ITritonService ResolveTritonService<T>(this Pool pool) where T : DbContext, new()
    {
        var s = pool.OfType<TritonService>().FirstOrDefault(p => p.Factory is EfCoreTransFactory<T>);
        if (s is not null) return s;
        pool.UseTriton().UseContext<T>();
        return ResolveTritonService<T>(pool);
    }
}
