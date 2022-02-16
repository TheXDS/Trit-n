using System;
using TheXDS.Triton.Diagnostics.Resources;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Middleware
{
    /// <summary>
    /// Clase estática que define la funcionalidad de escritura de entradas de 
    /// bitácora cuando ocurre un cambio en la base de datos.
    /// </summary>
    public static class JournalMiddleware
    {
        /// <summary>
        /// Estructura que contiene los valores de configuración a utilizar
        /// para cada escritor de bitácora.
        /// </summary>
        public struct Settings
        {
            /// <summary>
            /// Obtiene un objeto que se puede utilizar para identificar al
            /// actor que ha ejecutado la acción CRUD.
            /// </summary>
            public IActorProvider? ActorProvider { get; init; }

            /// <summary>
            /// Obtiene un objeto que se puede utilizar para extraer los
            /// valores anteriores de la entidad que ha sido afectada por la
            /// operación CRUD.
            /// </summary>
            public IOldValueProvider? OldValueProvider { get; init; }
        }

        /// <summary>
        /// Agrega un escritor de bitácora a la configuración de transacciones.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de escritor de bitácora a instanciar.
        /// </typeparam>
        /// <param name="config">
        /// Configuración de transacción a la cual agregar el Middleware.
        /// </param>
        /// <returns>
        /// La misma instancia que <paramref name="config"/>, permitiendo el
        /// uso de sintaxis Fluent.
        /// </returns>
        public static IMiddlewareConfigurator UseJournal<T>(this IMiddlewareConfigurator config) where T : notnull, IJournalMiddleware, new()
        {
            return config.UseJournal(new T(), default);
        }

        /// <summary>
        /// Agrega un escritor de bitácora a la configuración de transacciones.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de escritor de bitácora a instanciar.
        /// </typeparam>
        /// <param name="config">
        /// Configuración de transacción a la cual agregar el Middleware.
        /// </param>
        /// <param name="journalSingleton">
        /// Instancia del escritor de bitácora a agregar.
        /// </param>
        /// <returns>
        /// La misma instancia que <paramref name="config"/>, permitiendo el
        /// uso de sintaxis Fluent.
        /// </returns>
        public static IMiddlewareConfigurator UseJournal<T>(this IMiddlewareConfigurator config, T journalSingleton) where T : notnull, IJournalMiddleware, new()
        {
            return config.UseJournal(journalSingleton, default);
        }

        /// <summary>
        /// Agrega un escritor de bitácora a la configuración de transacciones.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de escritor de bitácora a instanciar.
        /// </typeparam>
        /// <param name="config">
        /// Configuración de transacción a la cual agregar el Middleware.
        /// </param>
        /// <param name="configuration">
        /// Estructura que contiene una serie de objetos auxiliares de
        /// configuración que pueden ser utilizados por el escritor de
        /// bitácora.
        /// </param>
        /// <returns>
        /// La misma instancia que <paramref name="config"/>, permitiendo el
        /// uso de sintaxis Fluent.
        /// </returns>
        public static IMiddlewareConfigurator UseJournal<T>(this IMiddlewareConfigurator config, Settings configuration) where T : notnull, IJournalMiddleware, new()
        {
            return config.UseJournal(new T(), configuration);
        }

        /// <summary>
        /// Agrega un escritor de bitácora a la configuración de transacciones.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de escritor de bitácora a instanciar.
        /// </typeparam>
        /// <param name="config">
        /// Configuración de transacción a la cual agregar el Middleware.
        /// </param>
        /// <param name="journalSingleton">
        /// Instancia del escritor de bitácora a agregar.
        /// </param>
        /// <param name="configuration">
        /// Estructura que contiene una serie de objetos auxiliares de
        /// configuración que pueden ser utilizados por el escritor de
        /// bitácora.
        /// </param>
        /// <returns>
        /// La misma instancia que <paramref name="config"/>, permitiendo el
        /// uso de sintaxis Fluent.
        /// </returns>
        public static IMiddlewareConfigurator UseJournal<T>(this IMiddlewareConfigurator config, T journalSingleton, Settings configuration) where T : notnull, IJournalMiddleware
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