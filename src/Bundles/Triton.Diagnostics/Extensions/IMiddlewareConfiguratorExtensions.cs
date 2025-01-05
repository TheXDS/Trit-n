using System.Collections.ObjectModel;
using TheXDS.Triton.Diagnostics.Middleware;
using TheXDS.Triton.Diagnostics.Resources;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Diagnostics.Extensions;

/// <summary>
/// Contains extensions that facilitate the registration of certain diagnostic
/// middlewares in an <see cref="IMiddlewareConfigurator"/>.
/// </summary>
public static class MiddlewareConfiguratorExtensions
{
    /// <summary>
    /// Registers a middleware that observes the current state of changes
    /// programmed to be made in a transaction.
    /// </summary>
    /// <param name="config">
    /// Transaction configuration to which the middleware will be added.
    /// </param>
    /// <param name="observer">
    /// A collection observable that will contain the changes programmed for
    /// writing in the active transaction.
    /// </param>
    /// <returns>
    /// The same instance as <paramref name="config"/>, allowing the use of
    /// fluent syntax.
    /// </returns>
    public static IMiddlewareConfigurator UseChangeTrackerObserver(this IMiddlewareConfigurator config, out ReadOnlyObservableCollection<ChangeTrackerItem> observer)
    {
        ChangeTrackerObserverMiddleware changeTracker = new();
        observer = changeTracker.Changes;
        return config.Attach(changeTracker);
    }

    /// <summary>
    /// Adds a journal writer to the transaction configuration.
    /// </summary>
    /// <typeparam name="T">
    /// Type of journal writer to instantiate.
    /// </typeparam>
    /// <param name="config">
    /// Transaction configuration to which the middleware will be added.
    /// </param>
    /// <returns>
    /// The same instance as <paramref name="config"/>, allowing the use of
    /// fluent syntax.
    /// </returns>
    public static IMiddlewareConfigurator UseJournal<T>(this IMiddlewareConfigurator config) where T : IJournalMiddleware, new()
    {
        return config.UseJournal(new T(), default);
    }

    /// <summary>
    /// Adds a journal writer to the transaction configuration.
    /// </summary>
    /// <typeparam name="T">
    /// Type of journal writer to instantiate.
    /// </typeparam>
    /// <param name="config">
    /// Transaction configuration to which the middleware will be added.
    /// </param>
    /// <param name="journalSingleton">
    /// Instance of the journal writer to add.
    /// </param>
    /// <returns>
    /// The same instance as <paramref name="config"/>, allowing the use of
    /// fluent syntax.
    /// </returns>
    public static IMiddlewareConfigurator UseJournal<T>(this IMiddlewareConfigurator config, T journalSingleton) where T : IJournalMiddleware
    {
        return config.UseJournal(journalSingleton, default);
    }

    /// <summary>
    /// Adds a journal writer to the transaction configuration.
    /// </summary>
    /// <typeparam name="T">
    /// Type of journal writer to instantiate.
    /// </typeparam>
    /// <param name="config">
    /// Transaction configuration to which the middleware will be added.
    /// </param>
    /// <param name="configuration">
    /// Structure containing a series of auxiliary configuration objects that
    /// can be used by the journal writer.
    /// </param>
    /// <returns>
    /// The same instance as <paramref name="config"/>, allowing the use of
    /// fluent syntax.
    /// </returns>
    public static IMiddlewareConfigurator UseJournal<T>(this IMiddlewareConfigurator config, JournalSettings configuration) where T : IJournalMiddleware, new()
    {
        return config.UseJournal(new T(), configuration);
    }

    /// <summary>
    /// Adds a journal writer to the transaction configuration.
    /// </summary>
    /// <typeparam name="T">
    /// Type of journal writer to instantiate.
    /// </typeparam>
    /// <param name="config">
    /// Transaction configuration to which the middleware will be added.
    /// </param>
    /// <param name="journalSingleton">
    /// Instance of the journal writer to add.
    /// </param>
    /// <param name="configuration">
    /// Structure containing a series of auxiliary configuration objects that
    /// can be used by the journal writer.
    /// </param>
    /// <returns>
    /// The same instance as <paramref name="config"/>, allowing the use of
    /// fluent syntax.
    /// </returns>
    public static IMiddlewareConfigurator UseJournal<T>(this IMiddlewareConfigurator config, T journalSingleton, JournalSettings configuration) where T : IJournalMiddleware
    {
        return config.AddLateEpilogue((a, m) =>
        {
            try
            {
                journalSingleton.Log(a, m, configuration);
            }
            catch (Exception ex)
            {
                return new ServiceResult(string.Format(Strings.JournalError, ex.GetType(), ex.Message));
            }
            return null;
        });
    }

    /// <summary>
    /// Configures the transaction to simulate operations without performing
    /// any action.
    /// </summary>
    /// <param name="config">
    /// Transaction configuration to which the middleware will be added.
    /// </param>
    /// <param name="runEpilogs">
    /// Indicates whether the epilogues of generated transactions should be
    /// executed.
    /// </param>
    /// <returns>
    /// The same instance as <paramref name="config"/>, allowing the use of
    /// fluent syntax.
    /// </returns>
    public static IMiddlewareConfigurator UseSimulation(this IMiddlewareConfigurator config, bool runEpilogs = true)
    {
        var _ = new ReadOnlySimulator(config, runEpilogs);
        return config;
    }
}
