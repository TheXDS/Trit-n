using Microsoft.EntityFrameworkCore;
using TheXDS.Triton.Services;

namespace TheXDS.ServicePool.Triton;

/// <summary>
/// Contains extension methods that allow configuring Tritón to be used with 
/// <see cref="Pool"/> and Entity Framework.
/// </summary>
public static class ServicePoolEfExtensions
{
    /// <summary>
    /// Resolves an instance of a service that can be used to access the
    /// requested database context.
    /// </summary>
    /// <typeparam name="T">
    /// Type of data context to use.
    /// </typeparam>
    /// <param name="pool">
    /// The <see cref="Pool"/> to configure.
    /// </param>
    /// <returns>
    /// An instance of <see cref="ITritonService"/> that allows access to the
    /// requested database context.
    /// </returns>
    /// <remarks>
    /// Prefer resolving a specific service directly if you need to access its
    /// functionality directly.
    /// </remarks>
    public static ITritonService ResolveTritonService<T>(this Pool pool) where T : DbContext, new()
    {
        var s = pool.OfType<TritonService>().FirstOrDefault(p => p.Factory is EfCoreTransFactory<T>);
        if (s is not null) return s;
        pool.UseTriton().UseContext<T>();
        return ResolveTritonService<T>(pool);
    }
}