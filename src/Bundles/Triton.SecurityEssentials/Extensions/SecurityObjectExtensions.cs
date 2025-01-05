using TheXDS.Triton.Models;

namespace TheXDS.Triton.Extensions;

/// <summary>
/// Provides extension methods for working with security objects.
/// </summary>
public static class SecurityObjectExtensions
{
    /// <summary>
    /// Adds a security object to one or more user groups.
    /// </summary>
    /// <param name="obj">The security object to add.</param>
    /// <param name="groups">
    /// The user groups to which the security object will be added.
    /// </param>
    public static void AddToGroup(this SecurityObject obj, params UserGroup[] groups)
    {
        ArgumentNullException.ThrowIfNull(obj);
        foreach (var group in groups)
        {
            obj.Membership.Add(new() { Group = group, SecurityObject = obj });
        }
    }
}