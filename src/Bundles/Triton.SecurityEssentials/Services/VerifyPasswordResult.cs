using TheXDS.Triton.Models;

namespace TheXDS.Triton.Services
{
    /// <summary>
    /// Representa el resultado de una comprobación de contraseña.
    /// </summary>
    public class VerifyPasswordResult
    {
        /// <summary>
        /// Obtiene un resultado inválido sin credencial.
        /// </summary>
        public static VerifyPasswordResult Invalid => new(false, null!);
        
        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="VerifyPasswordResult"/>.
        /// </summary>
        /// <param name="valid">
        /// Valor que indica si las credenciales provistas fueron válidas.
        /// </param>
        /// <param name="loginCredential">
        /// Credencial que ha sido obtenida.
        /// </param>
        public VerifyPasswordResult(bool? valid, LoginCredential loginCredential)
        {
            Valid = valid;
            LoginCredential = loginCredential;
        }

        /// <summary>
        /// Obtiene un valor que indica si las credenciales provistas fueron
        /// válidas.
        /// </summary>
        public bool? Valid { get; }

        /// <summary>
        /// Obtiene una referencia a la credencial que ha sido obtenida para
        /// validación.
        /// </summary>
        public LoginCredential LoginCredential { get; }
    }
}
