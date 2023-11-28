using Microsoft.EntityFrameworkCore;
using TheXDS.Triton.SecurityEssentials.Ef.Models;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.SecurityEssentials.Ef.Services.Base;

/// <summary>
/// Clase base para los servicios de Tritón que permitan acceso a un
/// contexto de datos con información de autenticación y permisos de
/// usuarios.
/// </summary>
/// <typeparam name="T">
/// Tipo de <see cref="DbContext"/> que contiene los
/// <see cref="DbSet{TEntity}"/> necesarios para implementar autenticación
/// y permisos de usuarios.
/// </typeparam>
public abstract class UserServiceBase<T> : TritonService, IEfUserService<T> where T : DbContext, IUserDbContext, new()
{
}
