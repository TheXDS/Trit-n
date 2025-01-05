using TheXDS.MCART.Exceptions;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Diagnostics.Middleware;

/// <summary>
/// A static middleware that blocks all write operations, returning the result
/// as either a <see cref="ServiceResult.Ok"/> or an error produced in the
/// epilogue of the transaction.
/// </summary>
public class ReadOnlySimulator
{
    private readonly IMiddlewareConfigurator _config;
    private readonly bool _runEpilogs;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySimulator"/>
    /// class.
    /// </summary>
    /// <param name="config">
    /// Configuration for which this Middleware is being registered.
    /// </param>
    /// <param name="runEpilogs">
    /// Indicates whether to run epilogues for write operations.
    /// </param>
    public ReadOnlySimulator(IMiddlewareConfigurator config, bool runEpilogs)
    {
        _config = config;
        _runEpilogs = runEpilogs;
        config.AddLatePrologue(SkipActualCall);
    }

    private ServiceResult? SkipActualCall(CrudAction arg1, IEnumerable<ChangeTrackerItem>? arg2)
    {
        if (arg1 == CrudAction.Read) return null;
        return (_runEpilogs ? (_config ?? throw new TamperException()).GetRunner().RunEpilogue(arg1, arg2) : null) ?? ServiceResult.Ok;
    }
}