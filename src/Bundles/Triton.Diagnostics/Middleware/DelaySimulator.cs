using TheXDS.MCART.Types;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Services;
using TheXDS.Triton.Models.Base;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Triton.Diagnostics.Middleware;

/// <summary>
/// Middleware que simula retrasos aleatorios en la conexión con el orígen de
/// datos.
/// </summary>
public class DelaySimulator : ITransactionMiddleware
{
    private readonly Random random = new();

    /// <summary>
    /// Obtiene o establece el rango de retraso a utilizar para la simulación
    /// de retrasos.
    /// </summary>
    public Range<int> DelayRange { get; set; }

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="DelaySimulator"/>.
    /// </summary>
    public DelaySimulator() : this(500, 1500)
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="DelaySimulator"/>.
    /// </summary>
    /// <param name="min">Retraso mínimo a simular en milisegundos.</param>
    /// <param name="max">Retraso máximo a simular en milisegundos.</param>
    public DelaySimulator(int min, int max)
    {
        DelayRange = new Range<int>(min, max);
    }

    ServiceResult? ITransactionMiddleware.PrologAction(CrudAction action, IEnumerable<Model>? entity)
    {
        Thread.Sleep(random.Next(DelayRange));
        return null;
    }
}
