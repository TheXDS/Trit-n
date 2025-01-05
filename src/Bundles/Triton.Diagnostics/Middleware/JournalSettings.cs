namespace TheXDS.Triton.Diagnostics.Middleware;

/// <summary>
/// A structure that contains the configuration values to be used for each
/// journal writer.
/// </summary>
public readonly struct JournalSettings
{
    /// <summary>
    /// Gets an object that can be used to identify the actor who executed the
    /// CRUD action.
    /// </summary>
    public IActorProvider? ActorProvider { get; init; }
}