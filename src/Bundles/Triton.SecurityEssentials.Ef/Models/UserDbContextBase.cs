using Microsoft.EntityFrameworkCore;
using TheXDS.Triton.Models;

namespace TheXDS.Triton.SecurityEssentials.Ef.Models;

/// <summary>
/// Clase base para un contexto de datos que incluye distintos
/// <see cref="DbSet{TEntity}"/> que almacenan información de autenticación y
/// permisos de usuarios.
/// </summary>
public abstract class UserDbContextBase : DbContext, IUserDbContext
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="UserDbContextBase"/>.
    /// </summary>
    protected UserDbContextBase()
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="UserDbContextBase"/>, permitiendo especificar las opciones
    /// del contexto de datos.
    /// </summary>
    /// <param name="options">Opciones del contexto de datos.</param>
    protected UserDbContextBase(DbContextOptions options) : base(options)
    {
    }

    /// <inheritdoc/>
    public DbSet<LoginCredential> LoginCredentials { get; set; } = null!;

    /// <inheritdoc/>
    public DbSet<MultiFactorEntry> MfaEntries { get; set; } = null!;

    /// <inheritdoc/>
    public DbSet<SecurityDescriptor> SecurityDescriptors { get; set; } = null!;

    /// <inheritdoc/>
    public DbSet<Session> Sessions { get; set; } = null!;

    /// <inheritdoc/>
    public DbSet<UserGroup> UserGroups { get; set; } = null!;

    /// <inheritdoc/>
    public DbSet<UserGroupMembership> UserGroupMemberships { get; set; } = null!;
}
