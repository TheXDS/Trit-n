using Microsoft.EntityFrameworkCore;
using TheXDS.Triton.Models;

namespace TheXDS.Triton.SecurityEssentials.Ef.Models;

/// <summary>
/// Define una serie de miembros a implementar por un
/// <see cref="DbContext"/> que incluya las tablas necesarias para
/// almacenar información de autenticación y permisos de usuarios.
/// </summary>
public interface IUserDbContext
{
    /// <summary>
    /// Obtiene o establece una referencia al <see cref="DbSet{TEntity}"/>
    /// que almacena objetos de tipo <see cref="LoginCredential"/>.
    /// </summary>
    DbSet<LoginCredential> LoginCredentials { get; set; }
    
    /// <summary>
    /// Obtiene o establece una referencia al <see cref="DbSet{TEntity}"/>
    /// que almacena objetos de tipo <see cref="MultiFactorEntry"/>.
    /// </summary>
    DbSet<MultiFactorEntry> MfaEntries { get; set; }

    /// <summary>
    /// Obtiene o establece una referencia al <see cref="DbSet{TEntity}"/>
    /// que almacena objetos de tipo <see cref="SecurityDescriptor"/>.
    /// </summary>
    DbSet<SecurityDescriptor> SecurityDescriptors { get; set; }

    /// <summary>
    /// Obtiene o establece una referencia al <see cref="DbSet{TEntity}"/>
    /// que almacena objetos de tipo <see cref="Session"/>.
    /// </summary>
    DbSet<Session> Sessions { get; set; }

    /// <summary>
    /// Obtiene o establece una referencia al <see cref="DbSet{TEntity}"/>
    /// que almacena objetos de tipo <see cref="UserGroup"/>.
    /// </summary>
    DbSet<UserGroup> UserGroups { get; set; }

    /// <summary>
    /// Obtiene o establece una referencia al <see cref="DbSet{TEntity}"/>
    /// que almacena objetos de tipo <see cref="UserGroupMembership"/>.
    /// </summary>
    DbSet<UserGroupMembership> UserGroupMemberships { get; set; }
}
