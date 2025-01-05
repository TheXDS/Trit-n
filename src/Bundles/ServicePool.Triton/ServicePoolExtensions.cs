using TheXDS.ServicePool.Extensions;

namespace TheXDS.ServicePool.Triton;

/// <summary>
/// Contains extension methods that allow configuring Tritón to be used with
/// <see cref="Pool"/>.
/// </summary>
public static class ServicePoolExtensions
{
    private sealed class TritonConfigurable : ITritonConfigurable
    {
        public static TritonConfigurable Create(in Pool pool) => new(pool);

        public Pool Pool { get; }

        private TritonConfigurable(Pool pool)
        {
            Pool = pool;
        }
    }

    private static TritonConfigurable RegisterNewConfigIntoPool(Pool pool)
    {
        var c = TritonConfigurable.Create(pool);
        return c.RegisterInto(pool);
    }

    /// <summary>
    /// Configures a <see cref="Pool"/> to host Tritón data services.
    /// </summary>
    /// <param name="pool">The <see cref="Pool"/> to configure.</param>
    /// <returns>
    /// An object that can be used to configure the Tritón services.
    /// </returns>
    public static ITritonConfigurable UseTriton(this Pool pool)
    {
        ArgumentNullException.ThrowIfNull(pool);
        return pool.Discover<ITritonConfigurable>() ?? RegisterNewConfigIntoPool(pool);
    }

    /// <summary>
    /// Configures a <see cref="Pool"/> to host Tritón data services.
    /// </summary>
    /// <param name="pool">The <see cref="Pool"/> to configure.</param>
    /// <param name="configurator">
    /// A configuration delegate for the Tritón services.
    /// </param>
    /// <returns>
    /// The same instance as <paramref name="pool"/>, allowing Fluent syntax
    /// usage.
    /// </returns>
    public static Pool UseTriton(this Pool pool, Action<ITritonConfigurable> configurator)
    {
        ArgumentNullException.ThrowIfNull(pool);
        ArgumentNullException.ThrowIfNull(configurator);
        configurator(UseTriton(pool));
        return pool;
    }
}