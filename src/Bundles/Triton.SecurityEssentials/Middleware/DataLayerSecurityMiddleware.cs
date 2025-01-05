using TheXDS.MCART.Resources;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Component;
using TheXDS.Triton.Models;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Middleware;

/// <summary>
/// Middleware that secures data access by enforcing system-based access rules.
/// </summary>
/// <param name="securityActorProvider">
/// The provider of the security actor attempting to execute an action.
/// </param>
/// <param name="userService">
/// The user service used to authenticate and authorize actions.
/// </param>
public class DataLayerSecurityMiddleware(ISecurityActorProvider securityActorProvider, IUserService userService) : ITransactionMiddleware
{
    private readonly IUserService _userService = userService;
    private readonly ISecurityActorProvider _securityActorProvider = securityActorProvider;

    ServiceResult? ITransactionMiddleware.PrologueAction(CrudAction action, IEnumerable<ChangeTrackerItem>? entities)
    {
        if (entities is null) return null;
        if (_securityActorProvider.GetCurrentActor() is not { } actor) return FailureReason.Tamper;

        var entityTypes = entities.Select(p => p.Model);

        return entityTypes.All(entityType => _userService.CheckAccess(actor, GetModelContextString(action, entityType), MapCrudActionToFlags(action)).Result == true)
            ? null
            : FailureReason.Forbidden;
    }

    /// <summary>
    /// Returns a security context string for the specified CRUD action on the
    /// given model.
    /// </summary>
    /// <param name="action">The CRUD action to execute.</param>
    /// <param name="model">
    /// The model that defines the security context.
    /// </param>
    /// <returns>
    /// A string representing the operation on the model in a security context.
    /// </returns>
    public static string GetModelContextString(CrudAction action, Type model)
    {
        if (!model.Implements<Model>()) throw Errors.UnexpectedType(model, typeof(Model));
        return $"{typeof(CrudAction).FullName}.{action};{model.CSharpName()}";
    }

    /// <summary>
    /// Returns a security context string for the specified CRUD action on the
    /// given model.
    /// </summary>
    /// <param name="action">The CRUD action to execute.</param>
    /// <typeparam name="TModel">
    /// The type of the model that defines the security context.
    /// </typeparam>
    /// <returns>
    /// A string representing the operation on the model in a security context.
    /// </returns>
    public static string GetModelContextString<TModel>(CrudAction action) where TModel : Model
    {
        return GetModelContextString(action, typeof(TModel));
    }

    private static PermissionFlags MapCrudActionToFlags(CrudAction action)
    {
        return action switch
        {
            CrudAction.Create => PermissionFlags.Create,
            CrudAction.Read => PermissionFlags.Read,
            CrudAction.Update => PermissionFlags.Update,
            CrudAction.Delete => PermissionFlags.Delete,
            CrudAction.Commit => PermissionFlags.None,
            _ => PermissionFlags.All
        };
    }
}
