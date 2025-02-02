﻿namespace TheXDS.Triton.Services;

/// <summary>
/// Base class for Triton services that provide access to a data context with user authentication and permission information.
/// </summary>
public abstract class UserServiceBase : TritonService, IUserService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserServiceBase"/> class,
    /// automatically detecting the transaction configuration to use.
    /// </summary>
    /// <param name="factory">Transaction factory to use.</param>
    public UserServiceBase(ITransactionFactory factory) : base(factory)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserServiceBase"/> class,
    /// specifying the configuration to use.
    /// </summary>
    /// <param name="transactionConfiguration">
    /// Configuration to use for transactions generated by this service.
    /// </param>
    /// <param name="factory">Transaction factory to use.</param>
    protected UserServiceBase(IMiddlewareConfigurator transactionConfiguration, ITransactionFactory factory)
        : base(transactionConfiguration, factory)
    {
    }
}
