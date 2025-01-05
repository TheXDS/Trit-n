using TheXDS.MCART.Types;
using TheXDS.Triton.Middleware;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Diagnostics.Middleware;

/// <summary>
/// Middleware that simulates random delays in the connection with the data
/// origin.
/// </summary>
/// <param name="min">Minimum delay to simulate in milliseconds.</param>
/// <param name="max">Maximum delay to simulate in milliseconds.</param>
public class DelaySimulator(int min, int max) : ITransactionMiddleware
{
    private readonly Random random = new();

    /// <summary>
    /// Gets or sets the range of delay to use for the simulation of delays.
    /// </summary>
    public Range<int> DelayRange { get; set; } = new Range<int>(min, max);

    /// <summary>
    /// Initializes a new instance of the <see cref="DelaySimulator"/> class.
    /// </summary>
    public DelaySimulator() : this(500, 1500)
    {
    }

    ServiceResult? ITransactionMiddleware.PrologueAction(CrudAction action, IEnumerable<ChangeTrackerItem>? entity)
    {
        if (action == CrudAction.Read || action == CrudAction.Commit) Thread.Sleep(random.Next(DelayRange));
        return null;
    }
}