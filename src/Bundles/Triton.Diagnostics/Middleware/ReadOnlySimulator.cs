using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using TheXDS.MCART.Exceptions;
using TheXDS.Triton.Diagnostics.Resources;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Middleware
{
    /// <summary>
    /// Middleware estático que bloquea todas las operaciones de escritura
    /// de datos, devolviendo para las mismas siempre el resultado
    /// <see cref="ServiceResult.Ok"/> o un error producido en el epílogo
    /// de la transacción.
    /// </summary>
    public static class ReadOnlySimulator
    {
        /// <summary>
        /// Obtiene o establece un valor que indica si se ejecutarán los
        /// epílogos luego de bloquear la operación de la transacción.
        /// </summary>
        public static bool RunEpilogs { get; set; } = true;

        private static TransactionConfiguration? _config;

        /// <summary>
        /// Configura la transacción para simular las operaciones sin realizar
        /// ninguna acción.
        /// </summary>
        /// <param name="config">
        /// Configuración de transacción sobre la cual aplicar.
        /// </param>
        /// <returns>
        /// La misma instancia que <paramref name="config"/>, permitiendo
        /// utilizar sintaxis Fluent.
        /// </returns>
        public static TransactionConfiguration UseSimulation(this TransactionConfiguration config)
        {
            if (_config is { }) throw new InvalidOperationException();                        
            return _config = config.AddLastProlog(SkipActualCall);
        }

        private static ServiceResult? SkipActualCall(CrudAction arg1, Model? arg2)
        {
            if (arg1 == CrudAction.Read) return null;
            return (RunEpilogs ? (_config ?? throw new TamperException()).RunEpilog(arg1, arg2) : null) ?? ServiceResult.Ok;
        }
    }

    public interface IJournalMiddleware
    {
        Task Log(CrudAction action, Model? entity, JournalMiddleware.Settings settings);
    }

    public interface IActorProvider
    {
        string? GetCurrentActor();
    }

    public interface IOldValueProvider
    {
        IEnumerable<KeyValuePair<PropertyInfo, string?>>? GetOldValues(Model? entity);
    }

    public class TextFileJournal : IJournalMiddleware
    {
        public string Path { get; set; }

        public Task Log(CrudAction action, Model? entity, JournalMiddleware.Settings settings)
        {
            return System.IO.File.WriteAllLinesAsync(Path, new[] {
                $"{ settings.ActorProvider?.GetCurrentActor() ?? "Se"} ha ejecutado una operación '{action}'"
            });
        }
    }


    public static class JournalMiddleware
    {
        public struct Settings
        {
            public IActorProvider? ActorProvider { get; init; }

            public IOldValueProvider? OldValueProvider { get; init; }
        }




        public static TransactionConfiguration UseJournal<T>(this TransactionConfiguration config) where T : notnull, IJournalMiddleware, new()
        {
            return config.UseJournal(new T(), default);
        }

        public static TransactionConfiguration UseJournal<T>(this TransactionConfiguration config, T journalSingleton) where T : notnull, IJournalMiddleware, new()
        {
            return config.UseJournal(journalSingleton, default);
        }

        public static TransactionConfiguration UseJournal<T>(this TransactionConfiguration config, Settings configuration) where T : notnull, IJournalMiddleware, new()
        {
            return config.UseJournal(new T(), configuration);
        }

        public static TransactionConfiguration UseJournal<T>(this TransactionConfiguration config, T journalSingleton, Settings configuration) where T : notnull, IJournalMiddleware
        {

            return config.AddLastEpilog((a, m) =>
            {
                try
                {
                    journalSingleton.Log(a, m, configuration);
                }
                catch (Exception ex)
                {
                    return ServiceResult.SucceedWith<ServiceResult>(string.Format(Strings.JournalError, ex.GetType(), ex.Message));
                }
                return null;
            });
        }

    }
}