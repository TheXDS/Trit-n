using TheXDS.MCART.Types.Base;

namespace TheXDS.Triton.Component;

/// <summary>
/// Define una serie de miembros a implementar por un tipo que permita realizar
/// verificaciones de autenticación multi-factor con datos proporcionados para
/// el usuario que desea iniciar sesión.
/// </summary>
public interface IMfaProcessor : IExposeGuid
{
    /// <summary>
    /// Comprueba que la información de MFA provista es válida.
    /// </summary>
    /// <param name="mfaData">Datos de MFA del usuario.</param>
    /// <returns>
    /// <see langword="true"/> si el MFA valida correctamente la información
    /// del usuario, <see langword="false"/> en caso contrario.
    /// </returns>
    /// <remarks>
    /// Al implementar y registrar un servicio de MFA, éste debe implementar la
    /// interacción de UI requerida para obtener los datos de autenticación de
    /// múltiple factor; como OTP, llaves de hardware, entrada de número PIN,
    /// solicitudes biométricas, etc.
    /// </remarks>
    bool IsMfaValid(byte[] mfaData);
}
