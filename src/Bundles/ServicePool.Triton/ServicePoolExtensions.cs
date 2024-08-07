using TheXDS.ServicePool.Extensions;

namespace TheXDS.ServicePool.Triton;

/// <summary>
/// Contiene métodos de extensión que permiten configurar Tritón para
/// utilizarse en conjunto con
/// <see cref="ServicePool"/>.
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
    /// Configura un <see cref="ServicePool"/> para
    /// hostear servicios de datos de Tritón.
    /// </summary>
    /// <param name="pool">
    /// <see cref="ServicePool"/> a configurar.
    /// </param>
    /// <returns>
    /// Un objeto que puede utilizarse para configiurar los servicios de
    /// Tritón.
    /// </returns>
    public static ITritonConfigurable UseTriton(this Pool pool)
    {
        ArgumentNullException.ThrowIfNull(pool);
        return pool.Discover<ITritonConfigurable>() ?? RegisterNewConfigIntoPool(pool);
    }

    /// <summary>
    /// Configura un <see cref="ServicePool"/> para
    /// hostear servicios de datos de Tritón.
    /// </summary>
    /// <param name="pool">
    /// <see cref="ServicePool"/> a configurar.
    /// </param>
    /// <param name="configurator">
    /// Delegado de configuración de los servicios de Tritón.
    /// </param>
    /// <returns>
    /// La misma instancia que <paramref name="pool"/>, permitiendo el uso
    /// de sintaxis Fluent.
    /// </returns>
    public static Pool UseTriton(this Pool pool, Action<ITritonConfigurable> configurator)
    {
        ArgumentNullException.ThrowIfNull(pool);
        ArgumentNullException.ThrowIfNull(configurator);
        configurator(UseTriton(pool));
        return pool;
    }
}