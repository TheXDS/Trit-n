namespace TheXDS.Triton.Diagnostics.Middleware;

/// <summary>
/// A performance counter that contains a collection of samples that can be
/// used to generate graphs.
/// </summary>
public class PlottablePerfMonitor : PerformanceMonitorBase
{
    private readonly Queue<double> _events = new();
    private int _maxSamples = 1000;

    /// <summary>
    /// Gets the collection of registered events.
    /// </summary>
    public IEnumerable<double> Events => _events;

    /// <inheritdoc/>
    public override int EventCount => _events.Count;

    /// <inheritdoc/>
    public override double AverageMs => Get(Enumerable.Average);

    /// <inheritdoc/>
    public override double MinMs => Get(Enumerable.Min);

    /// <inheritdoc/>
    public override double MaxMs => Get(Enumerable.Max);

    /// <summary>
    /// Gets or sets the maximum number of samples to contain in this
    /// performance monitor.
    /// </summary>
    public int MaxSamples
    {
        get => _maxSamples;
        set
        {
            lock (_events)
            {
                _maxSamples = value;
                while (_events.Count > value) _events.Dequeue();
            }
        }
    }

    /// <inheritdoc/>
    protected override void OnReset()
    {
        _events.Clear();
    }

    /// <inheritdoc/>
    protected override void RegisterEvent(double milliseconds)
    {
        lock (_events)
        {
            if (_events.Count == _maxSamples) _events.Dequeue();
            _events.Enqueue(milliseconds);
        }
        Notify(nameof(Events));
    }

    private double Get(Func<IEnumerable<double>, double> func)
    {
        return _events.Count != 0 ? func(_events) : double.NaN;
    }
}