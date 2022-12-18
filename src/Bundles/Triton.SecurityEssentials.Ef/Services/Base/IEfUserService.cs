using Microsoft.EntityFrameworkCore;
using TheXDS.Triton.SecurityEssentials.Ef.Models;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.SecurityEssentials.Ef.Services.Base;

/// <summary>
/// Define una serie de miembros a implementar por un tipo que exponga
/// acceso a un contexto de datos que almacena información de autenticación
/// y permisos de usuarios.
/// </summary>
/// <typeparam name="T">
/// Tipo de <see cref="DbContext"/> que contiene los
/// <see cref="DbSet{TEntity}"/> necesarios para implementar autenticación
/// y permisos de usuarios.
/// </typeparam>
public interface IEfUserService<T> : IUserService, ITritonService<T> where T : DbContext, IUserDbContext, new()
{
}
