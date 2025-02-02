﻿using System.Security;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Security;
using TheXDS.Triton.Models;

namespace TheXDS.Triton.Services;

/// <summary>
/// Defines a set of members to be implemented by a type that exposes user management and security services.
/// </summary>
public interface IUserService : ITritonService
{
    /// <summary>
    /// Creates a new login credential by executing essential steps on it.
    /// </summary>
    /// <param name="username">
    /// The username to use for identifying the new credential.
    /// </param>
    /// <param name="password">
    /// The password to register with the new credential.
    /// </param>
    /// <param name="granted">
    /// Permission flags granted to the new credential.
    /// </param>
    /// <param name="revoked">
    /// Permission flags revoked from the new credential.
    /// </param>
    /// <param name="enabled">
    /// Indicates whether the new credential will be enabled.
    /// </param>
    /// <param name="passwordChangeScheduled">
    /// Indicates whether the new credential is scheduled to change its password upon login.
    /// </param>
    /// <param name="groups">
    /// The groups that the credential belongs to.
    /// </param>
    /// <returns>
    /// A task that, when completed, contains the result reported by the underlying service.
    /// </returns>
    Task<ServiceResult> AddNewLoginCredential(
        string username,
        SecureString password,
        PermissionFlags granted = PermissionFlags.None,
        PermissionFlags revoked = PermissionFlags.None,
        bool enabled = true,
        bool passwordChangeScheduled = false,
        params UserGroup[] groups)
    {
        return AddNewLoginCredential<Pbkdf2Storage>(
            username,
            password,
            granted,
            revoked,
            enabled,
            passwordChangeScheduled,
            groups);
    }

    /// <summary>
    /// Permite crear nuevas credenciales de inicio de sesión, ejecutando
    /// algunos pasos esenciales sobre la misma.
    /// </summary>
    /// <typeparam name="TAlg">
    /// Tipo de algoritmo de almacenamiento de contraseñas a utilizar.
    /// </typeparam>
    /// <param name="username">
    /// Nombre de inicio de sesión a utilizar para identificar a la nueva
    /// credencial.
    /// </param>
    /// <param name="password">
    /// Contraseña a registrar en la nueva credencial.
    /// </param>
    /// <param name="granted">Banderas de permisos otorgados.</param>
    /// <param name="revoked">Banderas de permisos denegados.</param>
    /// <param name="enabled">
    /// Indica si la nueva credencial estará habilitada.
    /// </param>
    /// <param name="passwordChangeScheduled">
    /// Indica si la nueva credencial está programada para cambiar la
    /// contraseña al iniciar sesión.
    /// </param>
    /// <param name="groups">
    /// Indica los grupos a los cuales la credencial pertenece.
    /// </param>
    /// <returns>
    /// Una tarea que, al finalizar, contiene el resultado reportado de la
    /// operación ejecutada por el servicio subyacente.
    /// </returns>
    async Task<ServiceResult> AddNewLoginCredential<TAlg>(string username, SecureString password, PermissionFlags granted = PermissionFlags.None, PermissionFlags revoked = PermissionFlags.None, bool enabled = true, bool passwordChangeScheduled = false, params UserGroup[] groups) where TAlg : IPasswordStorage, new()
    {
        await using var j = GetTransaction();
        var r = await j.SearchAsync<LoginCredential>(p => p.Username == username);
        if (!r.Success) return r;
        if (r.Result!.Length != 0) return FailureReason.EntityDuplication;
        Guid id = await j.GetUniqueIdAsync<LoginCredential, Guid>(Guid.NewGuid);
        var newCred = new LoginCredential(username, await HashPasswordAsync<TAlg>(password))
        {
            Id = id,
            Granted = granted,
            Revoked = revoked,
            Enabled = enabled,
            PasswordChangeScheduled = passwordChangeScheduled,
        };
        foreach (var group in groups)
        {
            newCred.Membership.Add(new() { Group = group, Member = newCred });
        }
        j.Create(newCred);
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
        if (!(r.Success && r.Result is { } result)) return r.CastUp<Session?>(null);

        if (result.Valid != true) return FailureReason.Forbidden;

        await using var j = GetWriteTransaction();
        Session s = new() { Timestamp = DateTime.UtcNow, Token = GenerateToken() };
        result.LoginCredential.Sessions.Add(s);
        j.Update(result.LoginCredential);
        j.Create(s);
        var retVal = await j.CommitAsync();
        s.Credential ??= result.LoginCredential;
        return retVal.CastUp(s);
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
        if (!r.Success || r.Result is not { } c) return r.CastUp<bool?>(null);
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

    /// <summary>
    /// Obtiene una sesión previamente activa utilizando el token de la misma.
    /// </summary>
    /// <param name="token">Token de sesión a ser continuada.</param>
    /// <returns>
    /// Una tarea que contiene el resultado de la operación asíncrona. El
    /// resultado podrá incluir una referencia a la sesión que será continuada,
    /// o <see langword="null"/> en caso que la sesión no exista, sea inválida
    /// o que ocurra una falla en el servicio de datos subyacente.
    /// </returns>
    async Task<ServiceResult<Session?>> ContinueSession(string token)
    {
        await using var j = GetReadTransaction();
        var query = j.All<Session>();
        if (query.Success)
        {
            return await Task.Run(() => query.FirstOrDefault(p => p.Token == token)) is { } session && IsSessionValid(session) ? session : FailureReason.Forbidden;
        }
        else
        {
            return query.Reason!;
        }
    }

    /// <summary>
    /// Finaliza una sesión abierta.
    /// </summary>
    /// <param name="session">Sesión a finalizar.</param>
    /// <returns>
    /// Una tarea que, al finalizar, contiene el resultado reportado de la
    /// operación ejecutada por el servicio subyacente.
    /// </returns>
    async Task<ServiceResult> EndSession(Session session)
    {
        if (session.EndTimestamp.HasValue) return FailureReason.Idempotency;
        await using var t = GetWriteTransaction();
        session.EndTimestamp = DateTime.UtcNow;
        t.Update(session);
        return await t.CommitAsync();
    }

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
        if (r.Success)
        {
            return r.Result?.FirstOrDefault() is { } credential
                ? new ServiceResult<LoginCredential?>(credential)
                : FailureReason.NotFound;
        }
        return r.CastUp<LoginCredential?>(null);
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
        return HashPassword<Pbkdf2Storage>(password);
    }

    /// <summary>
    /// Calcula el hash utilizado para almacenar la información de
    /// comprobación de la contraseña.
    /// </summary>
    /// <typeparam name="TAlg">Tipo de algoritmo a utilizar.</typeparam>
    /// <param name="password">
    /// Contraseña para la cual generar el Hash seguro.
    /// </param>
    /// <returns>
    /// Un arreglo de bytes con el Hash que ha sido calculado a partir de
    /// la contraseña provista.
    /// </returns>
    byte[] HashPassword<TAlg>(SecureString password) where TAlg : IPasswordStorage, new()
    {
        return PasswordStorage.CreateHash<TAlg>(password);
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
        return HashPasswordAsync<Pbkdf2Storage>(password);
    }

    /// <summary>
    /// Calcula de forma asíncrona el hash utilizado para almacenar la
    /// información de comprobación de la contraseña.
    /// </summary>
    /// <typeparam name="TAlg">Tipo de algoritmo a utilizar.</typeparam>
    /// <param name="password">
    /// Contraseña para la cual generar el Hash seguro.
    /// </param>
    /// <returns>
    /// Una tarea que, al finalizar, devolverá un arreglo de bytes con el
    /// Hash que ha sido calculado a partir de la contraseña provista.
    /// </returns>
    Task<byte[]> HashPasswordAsync<TAlg>(SecureString password) where TAlg : IPasswordStorage, new()
    {
        return Task.Run(() => HashPassword<TAlg>(password));
    }

    /// <summary>
    /// Verifica que las credenciales provistas sean válidas.
    /// </summary>
    /// <param name="userId">Nombre de inicio de sesión.</param>
    /// <param name="password">Contraseña.</param>
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
        VerifyPasswordResult GetResult(byte[] expected, LoginCredential? user)
        {
            var success = PasswordStorage.VerifyPassword(password, expected) ?? false;
            return new VerifyPasswordResult(success, success ? user : null);
        }

        var r = await GetCredential(userId);
        return r switch
        {
            { Success: false, Reason: FailureReason.NotFound } => VerifyPasswordResult.Invalid,
            { Success: false } failure => failure.CastUp<VerifyPasswordResult?>(null),
            { Success: true, Result: { PasswordHash: { } passwd, Enabled: true } user } => GetResult(passwd, user),
            _ => VerifyPasswordResult.Invalid
        };
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

    private static string GenerateToken(int length = 128)
    {
        using var rnd = System.Security.Cryptography.RandomNumberGenerator.Create();
        var retVal = new byte[length];
        rnd.GetBytes(retVal);
        return Convert.ToBase64String(retVal);
    }

    private static bool IsSessionValid(Session session)
    {
        return session.EndTimestamp is null && session.Timestamp + TimeSpan.FromSeconds(session.TtlSeconds) > DateTime.UtcNow;
    }
}
