namespace TheXDS.Triton.Component
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que exponga
    /// funcionalidad de gestión de seguridad por medio de un proveedor de
    /// autenticación.
    /// </summary>
    public interface IAuthenticable
    {
        /// <summary>
        /// Obtiene una referencia al proveedor de auttenticación registrado
        /// para esta instancia.
        /// </summary>
        IAuthenticationBroker AuthenticationBroker { get; }
    }
}