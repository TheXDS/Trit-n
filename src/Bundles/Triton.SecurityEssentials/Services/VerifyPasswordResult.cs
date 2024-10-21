using System.Diagnostics.CodeAnalysis;
using TheXDS.Triton.Models;

namespace TheXDS.Triton.Services;

/// <summary>
/// Representa el resultado de una comprobación de contraseña.
/// </summary>
/// <param name="valid">
/// Valor que indica si las credenciales provistas fueron válidas.
/// </param>
/// <param name="loginCredential">
/// Credencial que ha sido obtenida.
/// </param>
/// <remarks>
/// Mientras los servicios subyacentes de datos pueden devolver
/// <see langword="true"/> al ejecutar las operaciones de acceso a datos, las
/// comprobaciones deberían ocultar deliberadamente la razón por la cual la
/// verificación de la contraseña de un usuario ha fallado.
/// </remarks>
public class VerifyPasswordResult(bool valid, LoginCredential? loginCredential)
{
    /// <summary>
    /// Inicializa una nueva isntancia de la clase
    /// <see cref="VerifyPasswordResult"/> indicando que la verificación ha
    /// sido exitosa.
    /// </summary>
    /// <param name="loginCredential">
    /// Referencia a la credencial que ha sido verificada satisfactoriamente.
    /// </param>
    public VerifyPasswordResult(LoginCredential loginCredential) : this(true, loginCredential)
    {
    }

    /// <summary>
    /// Obtiene un resultado inválido sin credencial.
    /// </summary>
    public static VerifyPasswordResult Invalid => new(false, null);

    /// <summary>
    /// Obtiene un valor que indica si las credenciales provistas fueron
    /// válidas.
    /// </summary>
    [MemberNotNullWhen(true, nameof(LoginCredential))]
    public bool Valid { get; } = valid;

    /// <summary>
    /// Obtiene una referencia a la credencial que ha sido obtenida para
    /// validación.
    /// </summary>
    public LoginCredential? LoginCredential { get; } = loginCredential;
}
