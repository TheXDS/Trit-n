using System.Reflection;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;
using St = TheXDS.Triton.Diagnostics.Resources.Strings;

namespace TheXDS.Triton.Diagnostics.Middleware;

/// <summary>
/// Abstract class that defines a text-based log writer.
/// </summary>
public abstract class TextJournal : IJournalMiddleware
{
    private static readonly string[] forbidden = ["idasstring"];
    private static readonly string[] knownCensoredprops = 
    [
        "password", "key", "secret", "token", "hash",
        "salt", "iv", "key", "cipher", "signature",
        "digest", "hmac", "pin", "2fa", "otp"
    ];

    /// <inheritdoc/>
    public void Log(CrudAction action, IEnumerable<ChangeTrackerItem>? changeSet, JournalSettings settings)
    {
        string GetText() => $"{DateTime.Now:s}: {string.Format(St.XRanOperation, settings.ActorProvider?.GetCurrentActor() ?? St.NoActorProviderSubst, action)}";
        List<string> lines = [];
        if (changeSet is null)
        {
            lines.Add($"{GetText()}.");
        }
        else
        {
            foreach (var change in changeSet)
            {
                lines.Add(string.Format(St.XWithData, GetText(), change.NewEntity!.GetType().NameOf()));
                switch (change.ChangeType)
                {
                    case ChangeTrackerChangeType.Create:
                        AddNewValues(lines, change.NewEntity!);
                        break;
                    case ChangeTrackerChangeType.Update:
                        AddUpdatedValues(lines, change);
                        break;
                    case ChangeTrackerChangeType.Delete:
                        lines.Add($"  - Id: {change.OldEntity!.IdAsString}");
                        break;
                }
            }
        }
        WriteText(lines);
    }

    /// <summary>
    /// Abstract method to implement log writing functionality.
    /// </summary>
    /// <param name="lines">Lines of text to write.</param>
    protected abstract void WriteText(IEnumerable<string> lines);

    private static void AddUpdatedValues(List<string> lines, ChangeTrackerItem changes)
    {
        foreach (var j in GetProperties(changes.OldEntity!))
        {
            var oldValue = GetPropValue(j, changes.OldEntity!) ?? "<null>";
            var newValue = GetPropValue(j, changes.NewEntity!) ?? "<null>";
            if (oldValue != newValue)
            {
                lines.Add($"  - {j.NameOf()}: {oldValue} -> {newValue}");
            }
        }
    }

    private static void AddNewValues(List<string> lines, Model entity)
    {
        foreach (var j in GetProperties(entity!))
        {
            lines.Add($"  - {j.NameOf()}: {GetPropValue(j, entity)}");
        }
    }

    private static IEnumerable<PropertyInfo> GetProperties(Model entity)
    {
        return entity.GetType().GetProperties()
            .Where(p => p.CanRead && !forbidden.Contains(p.Name.ToLower()));
    }

    private static string GetPropValue(PropertyInfo prop, Model entity)
    {

        return prop.GetValue(entity) switch
        {
            string s when knownCensoredprops.Contains(prop.Name.ToLower()) => new string('*', s.Length),
            string s => s,
            byte[] when knownCensoredprops.Contains(prop.Name.ToLower()) => $"<Censored security blob>",
            byte[] b => $"byte[] ({b.LongLength.ByteUnits()})",
            IEnumerable<Model> e => TruncatedCollection(e),
            { } x => x.ToString() ?? string.Empty,
            null => "<null>"
        };
    }

    private static string TruncatedCollection(IEnumerable<Model> entities)
    {
        int count = 0;
        return $"[ {string.Join(", ", entities.Take(6).Select(p => { count++; return p.IdAsString; }).Take(5))}{(count == 6 ? "..." : null)} ]";
    }
}