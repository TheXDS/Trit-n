using TheXDS.Triton.Exceptions;
using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Services;

/// <summary>
/// Represents an entry in a collection that contains information about changes
/// made during a transaction.
/// </summary>
public class ChangeTrackerItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChangeTrackerItem"/>
    /// class.
    /// </summary>
    /// <param name="oldEntity">Old value of the entity.</param>
    /// <param name="newEntity">New value of the entity.</param>
    public ChangeTrackerItem(Model? oldEntity, Model? newEntity)
    {
        if (oldEntity is not null && newEntity is not null && oldEntity.GetType() != newEntity.GetType())
        {
            throw new ModelTypeMismatchException();
        }
        OldEntity = oldEntity;
        NewEntity = newEntity;
    }

    /// <summary>
    /// Gets a value that infers the change represented by this instance.
    /// </summary>
    public ChangeTrackerChangeType ChangeType => (OldEntity, NewEntity) switch
    {
        (null, null) => ChangeTrackerChangeType.NoChange,
        (null, { }) => ChangeTrackerChangeType.Create,
        ({ }, { }) => ChangeTrackerChangeType.Update,
        ({ }, null) => ChangeTrackerChangeType.Delete,
    };

    /// <summary>
    /// Gets a reference to the model of the entity represented by this
    /// change-tracking entry.
    /// </summary>
    public Type Model => (OldEntity ?? NewEntity)?.GetType() ?? typeof(Model);

    /// <summary>
    /// Gets a reference to the old entity for this change.
    /// </summary>
    /// <remarks>
    /// This value may be <see langword="null"/> when this instance represents
    /// either no change or a new entity.
    /// </remarks>
    public Model? OldEntity { get; }

    /// <summary>
    /// Gets a reference to the new entity for this change.
    /// </summary>
    /// <remarks>
    /// This value may be <see langword="null"/> when this instance represents
    /// either no change or the deletion of an entity.
    /// </remarks>
    public Model? NewEntity { get; }
}