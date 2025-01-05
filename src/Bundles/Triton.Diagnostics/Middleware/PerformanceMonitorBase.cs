using System.Diagnostics;
using TheXDS.MCART.Events;
using TheXDS.MCART.Types.Base;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Diagnostics.Middleware;

/// <summary>
/// A base class for different types of performance counters available.
/// </summary>
public abstract class PerformanceMonitorBase : NotifyPropertyChanged, ITransactionMiddleware
{
    private readonly Stopwatch _stopwatch = new();
    private readonly CrudAction[] _allowedActions =
    [
        CrudAction.Commit,
        CrudAction.Read,
    ];

    /// <summary>
    /// Occurs when a CRUD action has been committed.
    /// </summary>
    public event EventHandler<ValueEventArgs<double>>? Elapsed;

    /// <summary>
    /// Gets the number of save operations recorded by this instance.
    /// </summary>
    public abstract int EventCount { get; }

    /// <summary>
    /// Gets the average time in milliseconds taken by save operations.
    /// </summary>
    public abstract double AverageMs { get; }

    /// <summary>
    /// Gets the minimum time in milliseconds taken by a save operation.
    /// </summary>
    public abstract double MinMs { get; }

    /// <summary>
    /// Gets the maximum time in milliseconds taken by a save operation.
    /// </summary>
    public abstract double MaxMs { get; }

    /// <summary>
    /// Resets the performance counters of this instance.
    /// </summary>
    public void Reset()
    {
        OnReset();
        NotifyState();
    }

    /// <summary>
    /// Resets the performance counters of this instance.
    /// </summary>
    protected abstract void OnReset();

    /// <summary>
    /// Registers a CRUD event in the performance counter.
    /// </summary>
    /// <param name="milliseconds">
    /// Milliseconds taken by the operation to complete.
    /// </param>
    protected abstract void RegisterEvent(double milliseconds);

    ServiceResult? ITransactionMiddleware.PrologueAction(CrudAction arg1, IEnumerable<ChangeTrackerItem>? _)
    {
        if (_allowedActions.Contains(arg1) && !_stopwatch.IsRunning) _stopwatch.Restart();
        return null;
    }

    ServiceResult? ITransactionMiddleware.EpilogueAction(CrudAction arg1, IEnumerable<ChangeTrackerItem>? _)
    {
        if (!_allowedActions.Contains(arg1)) return null;
        _stopwatch.Stop();
        RegisterEvent(_stopwatch.Elapsed.TotalMilliseconds);
        Elapsed?.Invoke(this, AverageMs);
        NotifyState();
        return null;
    }

    private void NotifyState()
    {
        Notify(nameof(EventCount));
        Notify(nameof(AverageMs));
        Notify(nameof(MinMs));
        Notify(nameof(MaxMs));
    }
}