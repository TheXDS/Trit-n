using System.Security;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Security;
using TheXDS.Triton.Models;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Services
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que exponga
    /// servicios de gestión de usuarios y seguridad.
    /// </summary>
    public interface IUserService : IService
    {
        /// <summary>
        /// Obtiene una credencial de inicio de sesión registrada con el nombre
        /// de inicio de sesión especificado.
        /// </summary>
        /// <param name="username">
        /// Nombre de inicio de sesión registrado.
        /// </param>
        /// <returns>
        /// Una tarea que, al finalizar, contiene el resultado reportado de la
        /// operación ejecutada por el servicio subyacente, incluyendo como
        /// valor de resultado a la entidad obtenida en la operación de
        /// lectura. Si no existe una entidad con el nombre de inicio de sesión
        /// especificado, el valor de resultado será <see langword="null"/>.
        /// </returns>
        async Task<ServiceResult<LoginCredential?>> GetCredential(string username)
        {
            await using var j = GetReadTransaction();
            var r = await j.SearchAsync<LoginCredential>(p => p.Username == username);
            return r.Success ? (r.ReturnValue!.FirstOrDefault() ?? ServiceResult.FailWith<ServiceResult<LoginCredential?>>(FailureReason.NotFound)) : r.CastUp<LoginCredential?>(null);
        }

        /// <summary>
        /// Permite crear nuevas credenciales de inicio de sesión, ejecutando
        /// algunos pasos esenciales sobre la misma.
        /// </summary>
        /// <param name="username">
        /// Nombre de inicio de sesión a utilizar para identificar a la nueva
        /// credencial.
        /// </param>
        /// <param name="password">
        /// Contraseña a registrar en la nueva credencial.
        /// </param>
        /// <returns>
        /// Una tarea que, al finalizar, contiene el resultado reportado de la
        /// operación ejecutada por el servicio subyacente.
        /// </returns>
        async Task<ServiceResult> AddNewLoginCredential(string username, SecureString password)
        {
            await using var j = GetTransaction();

            var r = await j.SearchAsync<LoginCredential>(p => p.Username == username);
            if (!r.Success) return r;
            if (r.ReturnValue!.Any()) return FailureReason.EntityDuplication;

            Guid id;
            do
            {
                id = Guid.NewGuid();
            } while ((await j.ReadAsync<LoginCredential, Guid>(id)).ReturnValue is not null);

            j.Create(new LoginCredential(username, await HashPasswordAsync(password)) { Id = id });
            return await j.CommitAsync();
        }

        /// <summary>
        /// Ejecuta una operación de autenticación con las credenciales
        /// provistas.
        /// </summary>
        /// <param name="username">Nombre de inicio de sesión.</param>
        /// <param name="password">Contraseña.</param>
        /// <returns>
        /// Una tarea que, al finalizar, contiene el resultado reportado de la
        /// operación ejecutada por el servicio subyacente, incluyendo como
        /// valor de resultado la nueva sesión del usuario que se ha sido
        /// autenticado. Si no ha sido posible autenticar las credenciales
        /// provistas, sea esto porque el usuario no existe o porque la
        /// contraseña es inválida, el valor de resultado será
        /// <see langword="null"/>.
        /// </returns>
        async Task<ServiceResult<Session?>> Authenticate(string username, SecureString password)
        {
            var r = await VerifyPassword(username, password);
            if (!(r.Success && r.ReturnValue is { } result)) return r.CastUp<Session?>(null);
            if (!(result.Valid ?? false)) return FailureReason.Forbidden;
            await using var j = GetWriteTransaction();
            Session s = new() { Timestamp = DateTime.UtcNow, Credential = result.LoginCredential };
            j.Create(s);
            return (await j.CommitAsync()).CastUp(s);
        }

        /// <summary>
        /// Verifica que las credenciales provistas sean válidas.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns>
        /// Una tarea que, al finalizar, retornará una tupla de valores que indica si la 
        /// </returns>
        /// <remarks>
        /// Si desea autenticar a un usuario, utilice el método
        /// <see cref="Authenticate(string, SecureString)"/>, ya que éste
        /// creará y persistirá un objeto que represente a la sesión actual.
        /// </remarks>
        /// <seealso cref="Authenticate(string, SecureString)"/>.
        async Task<ServiceResult<VerifyPasswordResult?>> VerifyPassword(string userId, SecureString password)
        {
            var r = await GetCredential(userId);
            if (!r.Success) return r.CastUp<VerifyPasswordResult?>(null);
            return r.ReturnValue is not { PasswordHash: { } passwd, Enabled: true } user
                ? VerifyPasswordResult.Invalid 
                : new VerifyPasswordResult(PasswordStorage.VerifyPassword(password, passwd), user);
        }

        /// <summary>
        /// Calcula el hash utilizado para almacenar la información de
        /// comprobación de la contraseña.
        /// </summary>
        /// <param name="password">
        /// Contraseña para la cual generar el Hash seguro.
        /// </param>
        /// <returns>
        /// Un arreglo de bytes con el Hash que ha sido calculado a partir de
        /// la contraseña provista.
        /// </returns>
        byte[] HashPassword(SecureString password)
        {
            return PasswordStorage.CreateHash(new Pbkdf2Storage(), password);
        }

        /// <summary>
        /// Calcula de forma asíncrona el hash utilizado para almacenar la
        /// información de comprobación de la contraseña.
        /// </summary>
        /// <param name="password">
        /// Contraseña para la cual generar el Hash seguro.
        /// </param>
        /// <returns>
        /// Una tarea que, al finalizar, devolverá un arreglo de bytes con el
        /// Hash que ha sido calculado a partir de la contraseña provista.
        /// </returns>
        Task<byte[]> HashPasswordAsync(SecureString password)
        {
            return Task.Run(() => HashPassword(password));
        }

        /// <summary>
        /// Comprueba el acceso de un usuario a un contexto de seguridad
        /// específico.
        /// </summary>
        /// <param name="username">Nombre de usuario a comprobar.</param>
        /// <param name="context">Contexto de seguridad a comprobar.</param>
        /// <param name="requested">Banderas de acceso solicitadas.</param>
        /// <returns>
        /// <see langword="true"/> si el usuario posee acceso al recurso,
        /// <see langword="false"/> en caso que el acceso ha sido denegado
        /// explícitamente, o <see langword="null"/> si no existe un objeto ni
        /// bandera de seguridad definida para el contexto.
        /// </returns>
        async Task<ServiceResult<bool?>> CheckAccess(string username, string context, PermissionFlags requested)
        {
            var r = await GetCredential(username);
            if (!r.Success || r.ReturnValue is not { } c) return r.CastUp<bool?>(null);
            return CheckAccess(c, context, requested);
        }

        /// <summary>
        /// Comprueba el acceso de un usuario a un contexto de seguridad
        /// específico.
        /// </summary>
        /// <param name="credential">Credencial a comprobar.</param>
        /// <param name="context">Contexto de seguridad a comprobar.</param>
        /// <param name="requested">Banderas de acceso solicitadas.</param>
        /// <returns>
        /// <see langword="true"/> si el usuario posee acceso al recurso,
        /// <see langword="false"/> en caso que el acceso ha sido denegado
        /// explícitamente, o <see langword="null"/> si no existe un objeto ni
        /// bandera de seguridad definida para el contexto.
        /// </returns>
        ServiceResult<bool?> CheckAccess(SecurityObject credential, string context, PermissionFlags requested)
        {
            foreach (var j in new[] { credential }.Concat(credential.Membership.Select(p => p.Group)))
            {
                if (ChkAccessInternal(j, context, requested) is { } b) return b;
            }
            return new((bool?)null);
        }

        private static bool? ChkAccessInternal(SecurityObject obj, string context, PermissionFlags requested)
        {
            return obj.Descriptors.FirstOrDefault(p => p.ContextId == context) is { } d
                && IsSet(d, requested) is { } b
                ? b
                : IsSet(obj, requested);
        }

        private static bool? IsSet(SecurityBase obj, PermissionFlags flags)
        {
            return obj.Granted.HasFlag(flags) ? true
                : obj.Revoked.HasFlag(flags) ? false
                : null;
        }
    }
}
