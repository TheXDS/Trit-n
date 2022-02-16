using System;
using System.Collections.Generic;
using System.Linq;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;
using St = TheXDS.Triton.Diagnostics.Resources.Strings;

namespace TheXDS.Triton.Middleware
{
    /// <summary>
    /// Clase abstracta que define un escritor de bitácora basado en entradas
    /// de texto.
    /// </summary>
    public abstract class TextJournal : IJournalMiddleware
    {
        /// <inheritdoc/>
        public void Log(CrudAction action, Model? entity, JournalMiddleware.Settings settings)
        {
            string GetText() => $"{DateTime.Now:s}: {string.Format(St.XRanOperation, settings.ActorProvider?.GetCurrentActor() ?? St.NoActorProviderSubst, action)}";
            if (action == CrudAction.Read) return;
            List<string> lines = new();

            if (entity is null)
            {
                lines.Add(string.Format(St.XWithNoData, GetText()));
            }
            else
            {
                lines.Add(string.Format(St.XWithData, GetText(), entity.GetType().NameOf(), entity.IdAsString));
                switch (action)
                {
                    case CrudAction.Create:
                        AddNewValues(lines, entity);
                        break;
                    case CrudAction.Update:
                        AddUpdatedValues(lines, entity, settings.OldValueProvider);
                        break;
                }
            }
            WriteText(lines);
        }

        /// <summary>
        /// Implementa la funcionalidad de escritura de bitácora de esta
        /// instancia.
        /// </summary>
        /// <param name="lines">Líneas de texto a escribir.</param>
        protected abstract void WriteText(IEnumerable<string> lines);

        private static void AddUpdatedValues(List<string> lines, Model entity, IOldValueProvider? oldValueProvider)
        {
            var c = oldValueProvider?.GetOldValues(entity);
            if (c is null) return;
            foreach (var j in c)
            {
                lines.Add($"  - {j.Key.NameOf()}: {j.Value ?? "<null>"} -> {j.Key.GetValue(entity) ?? "<null>"}");
            }
        }

        private static void AddNewValues(List<string> lines, Model entity)
        {
            foreach (var j in entity.GetType().GetProperties().Where(p => p.CanRead))
            {
                lines.Add($"  - {j.NameOf()}: {j.GetValue(entity)}");
            }
        }
    }
}