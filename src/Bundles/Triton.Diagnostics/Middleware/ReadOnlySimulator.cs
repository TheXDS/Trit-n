using TheXDS.MCART.Exceptions;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Diagnostics.Middleware;

/// <summary>
/// Middleware estático que bloquea todas las operaciones de escritura
/// de datos, devolviendo para las mismas siempre el resultado
/// <see cref="ServiceResult.Ok"/> o un error producido en el epílogo
/// de la transacción.
/// </summary>
public static class ReadOnlySimulator
{
    private class Singleton
    {
        private readonly IMiddlewareConfigurator _config;
        private readonly bool _runEpilogs;

        public Singleton(IMiddlewareConfigurator config, bool runEpilogs)
        {
            _config = config;
            _runEpilogs = runEpilogs;
            config.AddLastProlog(SkipActualCall);
        }

        private ServiceResult? SkipActualCall(CrudAction arg1, IEnumerable<Model>? arg2)
        {
            if (arg1 == CrudAction.Read) return null;
            return (_runEpilogs ? (_config ?? throw new TamperException()).GetRunner().RunEpilog(arg1, arg2) : null) ?? ServiceResult.Ok;
        }
    }

    /// <summary>
    /// Configura la transacción para simular las operaciones sin realizar
    /// ninguna acción.
    /// </summary>
    /// <param name="config">
    /// Configuración de transacción sobre la cual aplicar.
    /// </param>
    /// <param name="runEpilogs">
    /// Indica si se ejecutarán o los los epílogos de las transacciones 
    /// generadas.
    /// </param>
    /// <returns>
    /// La misma instancia que <paramref name="config"/>, permitiendo
    /// utilizar sintaxis Fluent.
    /// </returns>
    public static IMiddlewareConfigurator UseSimulation(this IMiddlewareConfigurator config, bool runEpilogs = true)
    {
        var _ = new Singleton(config, runEpilogs);
        return config;
    }
}