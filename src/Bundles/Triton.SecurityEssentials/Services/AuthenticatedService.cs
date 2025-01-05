using TheXDS.Triton.Component;

namespace TheXDS.Triton.Services;

/// <summary>
/// Base class for a Triton service with authentication support.
/// </summary>
/// <param name="userService">Service to use for authenticating operations that require elevation.</param>
/// <param name="transactionConfiguration">Transaction configuration to use.</param>
/// <param name="factory">Transaction factory to use.</param>
public abstract class AuthenticatedService(IUserService userService, IMiddlewareConfigurator transactionConfiguration, ITransactionFactory factory) : TritonService(transactionConfiguration, factory), IAuthenticable
{
    /// <inheritdoc/>
    public IAuthenticationBroker AuthenticationBroker { get; } = new AuthenticationBroker(transactionConfiguration, userService);

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticatedService"/> class.
    /// </summary>
    /// <param name="userService">Service to use for authenticating operations that require elevation.</param>
    /// <param name="factory">Transaction factory to use.</param>
    public AuthenticatedService(IUserService userService, ITransactionFactory factory) 
        : this(userService, new TransactionConfiguration(), factory)
    {
    }
}