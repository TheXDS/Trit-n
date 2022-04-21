namespace TheXDS.Triton.Models;
using System;
using TheXDS.Triton.Models.Base;

/// <summary>
/// Define las banderas de permisos que pueden ser otorgados o denegados a un
///  objeto <see cref="SecurityObject"/>
/// </summary>
[Flags]
public enum PermissionFlags : byte
{
    /// <summary>
    /// Ningún permiso.
    /// </summary>
    None = 0,

    /// <summary>
    /// Permiso de visibilidad.
    /// </summary>
    View = 1,

    /// <summary>
    /// Permiso de lectura.
    /// </summary>
    Read = 2,

    /// <summary>
    /// Permisos de creación de nuevas entidades.
    /// </summary>
    Create = 4,

    /// <summary>
    /// Permisos de actualización de entidades.
    /// </summary>
    Update = 8,

    /// <summary>
    /// Permisos de borrado.
    /// </summary>
    Delete = 16,

    /// <summary>
    /// Permisos de exportación de datos. También afecta la capacidad de crear reportes.
    /// </summary>
    Export = 32,

    /// <summary>
    /// Permisos de acceso exclusivo.
    /// </summary>
    Lock = 64,

    /// <summary>
    /// Permisos de elevación. Otorga o deniega la posibilidad de solicitar permisos no existentes.
    /// </summary>
    Elevate = 128,

    /// <summary>
    /// Todos los permisos de lectura.
    /// </summary>
    FullRead = View | Read,

    /// <summary>
    /// Todos los permisos de escritura.
    /// </summary>
    FullWrite = Create | Update | Delete,

    /// <summary>
    /// Todos los permisos comunes de lectura y escritura.
    /// </summary>
    ReadWrite = FullRead | FullWrite,

    /// <summary>
    /// Todos los permisos posibles
    /// </summary>
    All = byte.MaxValue
}

/// <summary>
/// Clase base para los modelos que contengan banderas de seguridad.
/// </summary>
public abstract class SecurityBase : Model<Guid>
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="SecurityBase"/>.
    /// </summary>
    protected SecurityBase()
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="SecurityBase"/>.
    /// </summary>
    /// <param name="granted">Banderas que describen los permisos otorgados.</param>
    /// <param name="revoked">Banderas que describen los permisos denegados.</param>
    protected SecurityBase(PermissionFlags granted, PermissionFlags revoked)
    {
        Granted = granted;
        Revoked = revoked;
    }

    /// <summary>
    /// Obtiene o establece las banderas que describen los permisos otorgados
    /// al objeto de seguridad que contenga a esta entidad.
    /// </summary>
    public PermissionFlags Granted { get; set; }
    
    /// <summary>
    /// Obtiene o establece las banderas que describen los permisos otorgados
    /// al objeto de seguridad que contenga a esta entidad.
    /// </summary>
    public PermissionFlags Revoked { get; set; }
}

/// <summary>
/// Clase base para las entidades que soportan descriptores de seguridad y
///  pertenencia a grupos.
/// </summary>
public abstract class SecurityObject : SecurityBase
{
    /// <summary>
    /// Obtiene o establece una coleccion que define las pertenencias a grupos
    ///  de usuario para la entidad actual.
    /// </summary>
    public virtual ICollection<UserGroupMembership> Membership { get; set; }

    /// <summary>
    /// Obtiene o establece el nombre a mostrar para esta entidad.
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="SecurityObject"/>.
    /// </summary>
    /// <param name="displayName">Nombre a mostrar para esta entidad.</param>
    /// <param name="granted">Banderas que describen los permisos otorgados.</param>
    /// <param name="revoked">Banderas que describen los permisos denegados.</param>
    protected SecurityObject(string displayName, PermissionFlags granted, PermissionFlags revoked) : base(granted, revoked)
    {
        DisplayName = displayName;
        Membership = new List<UserGroupMembership>();
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="SecurityObject"/>.
    /// </summary>
    protected SecurityObject()
        : this(null!, default, default)
    {
    }
}

/// <summary>
/// Modelo que representa a un descriptor de seguridad que indica permisos
/// otorgados y/o denegados a una entidad de seguridad con respecto a un
/// determinado contexto.
/// </summary>
public class SecurityDescriptor : SecurityBase
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase
    ///  <see cref="SecurityDescriptor"/>.
    /// </summary>
    /// <param name="contextId">
    /// Valor que indica un Id del contexto al cual se aplicará este descriptor
    /// de seguridad.
    /// </param>
    /// <param name="granted">
    /// Banderas que describen los permisos otorgados.
    /// </param>
    /// <param name="revoked">
    /// Banderas que describen los permisos denegados.
    /// </param>
    public SecurityDescriptor(string contextId, PermissionFlags granted, PermissionFlags revoked) : base(granted, revoked)
    {
        ContextId = contextId;
    }

    /// <summary>
    /// Obtiene o establece el Id de contexto al cual se aplicarán las banderas
    /// de seguridad de esta entidad.
    /// </summary>
    public string ContextId { get; set; }
}

/// <summary>
/// Modelo que representa a un usuario individual con facultad de iniciar sesión en un sistema que requiere autenticación.
/// </summary>
public class User : SecurityObject
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="User"/>.
    /// </summary>
    public User()
        : this(null!, null!, default, default, null!)
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="User"/>.
    /// </summary>
    /// <param name="displayName">Nombre a mostrar para esta entidad.</param>
    /// <param name="passwordHash">
    /// Blob binario con el Hash a utilizar para autenticar al usuario.
    /// </param>
    /// <param name="granted">
    /// Banderas que describen los permisos otorgados.
    /// </param>
    /// <param name="revoked">
    /// Banderas que describen los permisos denegados.
    /// </param>
    /// <param name="username">
    /// Nombre de inicio de sesión a asociar con el usuario.
    /// </param>
    public User(string displayName, byte[] passwordHash, PermissionFlags granted, PermissionFlags revoked, string username) : base(displayName, granted, revoked)
    {
        PasswordHash = passwordHash;
        Sessions = new List<Session>();
        RegisteredMfa = new List<MultiFactorEntry>();
        Username = username;
    }

    /// <summary>
    /// Obtiene o establece el nombre de inicio de sesión a asociar con esta
    /// entidad.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Obtiene o establece el Hash precomputado utilizado para autenticar al
    /// usuario.
    /// </summary>
    public byte[] PasswordHash { get; set; }

    /// <summary>
    /// Obtiene o establece la colección de sesiones activas para el usuario
    ///  representado por esta entidad.
    /// </summary>
    public virtual ICollection<Session> Sessions { get; set; }

    /// <summary>
    /// Obtiene o establece la colección de objetos de autenticación en dos
    /// factores registrados para el usuario.
    /// </summary>
    public virtual ICollection<MultiFactorEntry> RegisteredMfa { get; set; }
}

/// <summary>
/// Representa una entrada de datos de autenticación en dos factores.
/// </summary>
public class MultiFactorEntry : Model<Guid>
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="MultiFactorEntry"/>.
    /// </summary>
    public MultiFactorEntry()
        : this(Guid.Empty, null!)
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="MultiFactorEntry"/>.
    /// </summary>
    /// <param name="mfaPreprocessor">
    /// <see cref="Guid"/> utilizado para identificar el procesador de MFA a
    /// utilizar para verificar esta entidad.
    /// </param>
    /// <param name="data">
    /// Blob binario de datos personalizados a utilizar por el procesador de
    /// autenticación en dos factores.
    /// </param>
    public MultiFactorEntry(Guid mfaPreprocessor, byte[] data)
    {
        MfaProcessor = mfaPreprocessor;
        Data = data;
    }

    /// <summary>
    /// Obtiene o establece al usuario que posee esta entrada de autenticación
    /// en dos factores.
    /// </summary>
    public User User { get; set; } = null!;

    ///
    /// <summary>
    /// Obtiene o establece el <see cref="Guid"/> utilizado para identificar el
    /// procesador de MFA a utilizar para verificar esta entidad.
    /// </summary>
    public Guid MfaProcessor { get; set; }

    /// <summary>
    /// Obtiene o establece un blob binario de datos personalizados a utilizar
    /// por el procesador de autenticación en dos factores.
    /// </summary>
    public byte[] Data { get; set; }
}

/// <summary>
/// Modelo que representa una sessión de usuario activa.
/// </summary>
public class Session : TimestampModel<Guid>
{
    /// <summary>
    /// Obtiene o establece el usuario para el cual se ha creado este objeto de sesión.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Obtiene o establece el tóken de sesión para esta entidad.
    /// </summary>
    /// <value></value>
    public string Token { get; set; }

    /// <summary>
    /// Obtiene o establece el tiempo de vida en horas para esta sessión.
    /// </summary>
    public int TtlHours { get; set; }

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="Session"/>.
    /// </summary>
    /// <param name="ttlHours">Tiempo de vida en horas para esta sesión.</param>
    /// <param name="token"></param>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    public Session(int ttlHours, string token, DateTime timestamp) : base(timestamp)
    {
        TtlHours = ttlHours;
        Token = token;
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="Session"/>.
    /// </summary>
    public Session()
     : this(default, null!, default)
    {
    }
}

/// <summary>
/// Modelo que representa a un grupo de usuarios que comparten ciertas
/// facultades, propiedades y permisos de seguridad.
/// </summary>
public class UserGroup : SecurityObject
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="UserGroup"/>.
    /// </summary>
    public UserGroup()
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="UserGroup"/>.
    /// </summary>
    /// <param name="displayName">Nombre a mostrar para esta entidad.</param>
    /// <param name="granted">Banderas que describen los permisos otorgados.</param>
    /// <param name="revoked">Banderas que describen los permisos denegados.</param>
    public UserGroup(string displayName, PermissionFlags granted, PermissionFlags revoked) : base(displayName, granted, revoked)
    {
    }
}

/// <summary>
/// modelo que representa la membresía de un usuario a un grupo de usuarios.
/// </summary>
public class UserGroupMembership : Model<Guid>
{
    /// <summary>
    /// Obtiene o estabelce el grupo del cual el usuario es miembro.
    /// </summary>
    public UserGroup Group { get; set; } = null!;

    /// <summary>
    /// Obtiene o establece el usuario que es miembro de un grupo.
    /// </summary>
    public User User { get; set; } = null!;
}