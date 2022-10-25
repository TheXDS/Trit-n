namespace TheXDS.Triton.Tests.EFCore.Models;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TheXDS.Triton.Tests.Models;

/// <summary>
/// Contexto de datos de prueba que representa un Blog simple.
/// </summary>
public class BlogContext : DbContext
{
    private static readonly ILoggerFactory _m = LoggerFactory.Create(b => b.AddConsole().AddDebug());

    /// <summary>
    /// Tabla que contiene todos los Posts del blog.
    /// </summary>
    public virtual DbSet<Post> Posts { get; set; } = null!;

    /// <summary>
    /// Tabla que contiene a todos los usuarios.
    /// </summary>
    public virtual DbSet<User> Users { get; set; } = null!;

    /// <summary>
    /// Tabla que contiene todos los comentarios del blog.
    /// </summary>
    public virtual DbSet<Comment> Comments { get; set; } = null!;

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseInMemoryDatabase(GetType().FullName!)
            .UseLoggerFactory(_m);
    }

    /// <summary>
    /// Inicializa la clase <see cref="BlogContext"/>.
    /// </summary>
    /// <remarks>
    /// Las inicializaciones realizadas en este método son únicamente con
    /// fines de prueba.
    /// </remarks>
    static BlogContext()
    {
        using var c = new BlogContext();
        User u1, u2, u3;
        Post post;

        c.Users.AddRange(            
            u1 = new User("user1", "User #1", new DateTime(2001, 1, 1)),
            u2 = new User("user2", "User #2", new DateTime(2009, 3, 4)),
            u3 = new User("user3", "User #3", new DateTime(2004, 9, 11))
        );

        c.Posts.Add(post = new Post("Test", "This is a test.", u1, new DateTime(2016, 12, 31)) { Published = true });

        c.Comments.AddRange(
            new Comment(u2, post, "It works!", new DateTime(2017, 1, 1)),
            new Comment(u3, post, "Yay! c:", new DateTime(2017, 1, 2)),
            new Comment(u1, post, "Shuddap >:(", new DateTime(2017, 1, 3)),
            new Comment(u3, post, "ok :c", new DateTime(2017, 1, 4))
        );
        c.SaveChanges();
    }        
}