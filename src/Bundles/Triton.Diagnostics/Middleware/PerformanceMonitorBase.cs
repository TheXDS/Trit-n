using System.Diagnostics;
using TheXDS.MCART.Events;
using TheXDS.MCART.Types.Base;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Diagnostics.Middleware;

/// <summary>
/// Clase base para los distintos tipos de contadores de rendimiento disponibles.
/// </summary>
public abstract class PerformanceMonitorBase : NotifyPropertyChanged, ITransactionMiddleware
{
    private readonly Stopwatch _stopwatch = new();
    private readonly CrudAction[] _allowedActions =
    {
        CrudAction.Commit,
        CrudAction.Read,
    };

    /// <summary>
    /// Ocurre cuando se ha producido la acción Crud
    /// <see cref="CrudAction.Commit"/>.
    /// </summary>
    public event EventHandler<ValueEventArgs<double>>? Elapsed;

    /// <summary>
    /// Obtiene la cantidad de eventos de guardado registrados por esta
    /// instancia.
    /// </summary>
    public abstract int EventCount { get; }

    /// <summary>
    /// Obtiene el tiempo promedio en milisegundos que han tomado las
    /// operaciones de guardado.
    /// </summary>
    public abstract double AverageMs { get; }

    /// <summary>
    /// Obtiene la cantidad de tiempo en milisegundos que ha tomado la
    /// operación de guardado más corta.
    /// </summary>
    public abstract double MinMs { get; }

    /// <summary>
    /// Obtiene la cantidad de tiempo en milisegundos que ha tomado la
    /// operación de guardado más larga.
    /// </summary>
    public abstract double MaxMs { get; }

    /// <summary>
    /// Reinicia los contadores de rendimiento de esta instancia.
    /// </summary>
    public void Reset()
    {
        OnReset();
        NotifyState();
    }

    /// <summary>
    /// Reinicia los contadores de rendimiento de esta instancia.
    /// </summary>
    protected abstract void OnReset();

    /// <summary>
    /// Registra un evento de Crud en el contador de rendimiento.
    /// </summary>
    /// <param name="milliseconds">
    /// Milisegundos que la operación ha tomado en completarse.
    /// </param>
    protected abstract void RegisterEvent(double milliseconds);

    ServiceResult? ITransactionMiddleware.PrologAction(CrudAction arg1, IEnumerable<Model>? _)
    {
        if (_allowedActions.Contains(arg1) && !_stopwatch.IsRunning) _stopwatch.Restart();
        return null;
    }

    ServiceResult? ITransactionMiddleware.EpilogAction(CrudAction arg1, IEnumerable<Model>? _)
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