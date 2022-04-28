namespace TheXDS.Triton.Models;
using System;

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
    /// Permisos especiales.
    /// </summary>
    Special = Export | Lock | Elevate,

    /// <summary>
    /// Todos los permisos posibles
    /// </summary>
    All = byte.MaxValue
}
