using TheXDS.Triton.Middleware;

namespace TheXDS.Triton.Services;

/// <summary>
/// An object that provides configuration and other services to CRUD transactions.
/// </summary>
public sealed class TransactionConfiguration : IMiddlewareConfigurator
{
    private readonly ICollection<MiddlewareAction> _earlyPrologs = [];
    private readonly ICollection<MiddlewareAction> _midPrologs = [];
    private readonly ICollection<MiddlewareAction> _latePrologs = [];
    private readonly ICollection<MiddlewareAction> _earlyEpilogs = [];
    private readonly ICollection<MiddlewareAction> _midEpilogs = [];
    private readonly ICollection<MiddlewareAction> _lateEpilogs = [];

    /// <inheritdoc/>
    public IMiddlewareConfigurator AttachAt<T>(T middleware, in ActionPosition prologPosition, in ActionPosition epilogPosition) where T : ITransactionMiddleware
    {
        switch (prologPosition)
        {
            case ActionPosition.Default:
                AddPrologue(middleware.PrologAction);
                break;
            case ActionPosition.Early:
                AddEarlyPrologue(middleware.PrologAction);
                break;
            case ActionPosition.Late:
                AddLatePrologue(middleware.PrologAction);
                break;
        }
        switch (epilogPosition)
        {
            case ActionPosition.Default:
                AddEpilogue(middleware.EpilogAction);
                break;
            case ActionPosition.Early:
                AddEarlyEpilogue(middleware.EpilogAction);
                break;
            case ActionPosition.Late:
                AddLateEpilogue(middleware.EpilogAction);
                break;
        }
        return this;
    }

    /// <inheritdoc/>
    public IMiddlewareConfigurator AddPrologue(MiddlewareAction func)
    {
        _midPrologs.Add(func);
        return this;
    }

    /// <inheritdoc/>
    public IMiddlewareConfigurator AddEarlyPrologue(MiddlewareAction func)
    {
        _earlyPrologs.Add(func);
        return this;
    }

    /// <inheritdoc/>
    public IMiddlewareConfigurator AddLatePrologue(MiddlewareAction func)
    {
        _latePrologs.Add(func);
        return this;
    }

    /// <inheritdoc/>
    public IMiddlewareConfigurator AddEpilogue(MiddlewareAction func)
    {
        _midEpilogs.Add(func);
        return this;
    }

    /// <inheritdoc/>
    public IMiddlewareConfigurator AddEarlyEpilogue(MiddlewareAction func)
    {
        _earlyEpilogs.Add(func);
        return this;
    }

    /// <inheritdoc/>
    public IMiddlewareConfigurator AddLateEpilogue(MiddlewareAction func)
    {
        _lateEpilogs.Add(func);
        return this;
    }

    /// <inheritdoc/>
    public bool Detach(ITransactionMiddleware middleware)
    {
        return DetachPrologue(middleware.PrologAction) | DetachEpilogue(middleware.EpilogAction);
    }

    /// <inheritdoc/>
    public bool DetachPrologue(MiddlewareAction action)
    {
        return _earlyPrologs.Remove(action) || _midPrologs.Remove(action) || _latePrologs.Remove(action);
    }

    /// <inheritdoc/>
    public bool DetachEpilogue(MiddlewareAction action)
    {
        return _earlyEpilogs.Remove(action) || _midEpilogs.Remove(action) || _lateEpilogs.Remove(action);
    }

    IMiddlewareRunner IMiddlewareConfigurator.GetRunner()
    {
        return new TransactionRunner([.._earlyPrologs, .._midPrologs, .._latePrologs], [.._earlyEpilogs, .._midEpilogs, .._lateEpilogs]);
    }
}
