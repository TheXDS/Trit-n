using System;

namespace TheXDS.Triton.Annotations
{
    /// <summary>
    ///     Describe la categoría a la cual un método pertenece.
    /// </summary>
    [Flags]
    public enum MethodCategory
    {
        /// <summary>
        ///     Método cuya familia no ha sido especificada, o método de funcionalidad genérica.
        /// </summary>
        Unspecified,

        /// <summary>
        ///     Método de apertura de vista.
        /// </summary>
        Show,

        /// <summary>
        ///     Método de lectura de datos.
        /// </summary>
        View,

        /// <summary>
        ///     Método genérico de lectura.
        /// </summary>
        Read,

        /// <summary>
        ///     Método de creación.
        /// </summary>
        New,

        /// <summary>
        ///     Método de edición.
        /// </summary>
        Edit = 8,

        /// <summary>
        ///     Método de borrado.
        /// </summary>
        Delete = 16,

        /// <summary>
        ///     Método genérico de escritura.
        /// </summary>
        Write = New | Edit | Delete,

        /// <summary>
        ///     Método genérico de lectura/escritura.
        /// </summary>
        ReadWrite = Read | Write,

        /// <summary>
        ///     Método de función adicional (herramienta)
        /// </summary>
        Tool = 32,

        /// <summary>
        ///     Método de función de configuración.
        /// </summary>
        Config = 64,

        /// <summary>
        ///     Método de permisos de supervisor.
        /// </summary>
        Manager = ReadWrite | Tool,

        /// <summary>
        ///     Método de función administrativa.
        /// </summary>
        Admin = Manager | Config,
        /// <summary>
        ///     Método restringido a su invocación por super-usuarios
        /// </summary>
        Root = -1
    }
}