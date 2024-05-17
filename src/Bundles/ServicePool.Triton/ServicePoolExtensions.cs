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
        public static TritonConfigurable Create(in PoolBase pool) => new(pool);

        public PoolBase Pool { get; }

        private TritonConfigurable(PoolBase pool)
        {
            Pool = pool;
        }
    }

    private static TritonConfigurable RegisterNewConfigIntoPool(PoolBase pool)
    {
        var c = TritonConfigurable.Create(pool);
        return c.RegisterInto(pool);
        //switch (pool)
        //{
        //    case FlexPool fp:
        //    case Pool p:
        //        p.RegisterNow(c, [typeof(ITritonConfigurable)]);
        //        return c;
        //    default: throw new NotImplementedException();
        //}
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
    public static ITritonConfigurable UseTriton<TPool>(this TPool pool) where TPool : PoolBase
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
    public static TPool UseTriton<TPool>(this TPool pool, Action<ITritonConfigurable> configurator) where TPool : PoolBase
    {
        ArgumentNullException.ThrowIfNull(pool);
        ArgumentNullException.ThrowIfNull(configurator);
        configurator(UseTriton(pool));
        return pool;
    }
}