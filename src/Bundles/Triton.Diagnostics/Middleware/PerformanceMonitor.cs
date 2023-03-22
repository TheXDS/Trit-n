using TheXDS.MCART.Math;

namespace TheXDS.Triton.Diagnostics.Middleware;

/// <summary>
/// Middleware que permite obtener información específica sobre el
/// tiempo que toma ejecutar acciones Crud.
/// </summary>
public class PerformanceMonitor : PerformanceMonitorBase
{
    private int _evt;
    private double _avg;
    private double _min;
    private double _max;

    /// <inheritdoc/>
    public override int EventCount => _evt;

    /// <inheritdoc/>
    public override double AverageMs => _avg;

    /// <inheritdoc/>
    public override double MinMs => _min;

    /// <inheritdoc/>
    public override double MaxMs => _max;

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="PerformanceMonitor"/>.
    /// </summary>
    public PerformanceMonitor()
    {
        Reset();
    }

    /// <inheritdoc/>
    protected override void OnReset()
    {
        _avg = double.NaN;
        _evt = 0;
        _min = double.NaN;
        _max = double.NaN;
    }

    /// <inheritdoc/>
    protected override void RegisterEvent(double milliseconds)
    {
        if (!_avg.IsValid()) _avg = 0.0;
        if (milliseconds > _max || !_max.IsValid()) _max = milliseconds;
        if (milliseconds < _min || !_min.IsValid()) _min = milliseconds;
        _avg = ((_avg * _evt) + milliseconds) / ++_evt;
    }
}