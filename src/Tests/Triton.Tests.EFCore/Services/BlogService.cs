namespace TheXDS.Triton.Tests.EFCore.Services;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TheXDS.Triton.Services;
using Models;
using TheXDS.Triton.Tests.Models;

/// <summary>
/// Servicio de pruebas que permite administrar el contexto de datos
/// <see cref="BlogContext"/>.
/// </summary>
public class BlogService : Service
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="BlogService"/>.
    /// </summary>
    public BlogService() : base(new EfCoreTransFactory<BlogContext>())
    {
    }

    /// <summary>
    /// Obtiene una lista agrupada de todos los usuarios junto con sus 3
    /// primeros posts.
    /// </summary>
    /// <returns>
    /// Una lista agrupada de todos los usuarios junto con sus 3 primeros 
    /// posts.
    /// </returns>
    public IEnumerable<IGrouping<User, Post>> GetAllUsersFirst3Posts()
    {
        return WithReadTransaction(t =>
            t.All<User>()
                .Include(p => p.Posts)
                .ThenInclude(p => p.Author)
                .SelectMany(p => p.Posts.Take(3).OrderBy(q => q.CreationTime))
                .ToList()
                .GroupBy(p => p.Author)
        );
    }
}