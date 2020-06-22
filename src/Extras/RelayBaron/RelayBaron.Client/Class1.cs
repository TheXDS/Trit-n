using System;
using System.Collections.Generic;
using System.IO;
using TheXDS.MCART;
using TheXDS.MCART.Networking.Legacy.Client;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;
using static TheXDS.Triton.Services.CrudAction;

namespace RelayBaron.Client
{
    public class BaronNotifier : ManagedCommandClient<CrudAction, CrudAction>, ICrudNotifier
    {
        private readonly Dictionary<(string Model, CrudAction Action), List<Action<string>>> crudActionRegistry = new Dictionary<(string, CrudAction), List<Action<string>>>();

        /// <summary>
        ///     Inicializa la clase <see cref="BaronNotifier"/>
        /// </summary>
        static BaronNotifier()
        {
            ScanTypeOnCtor = false;
        }

        public BaronNotifier()
        {
            WireUp(Create, ReadPack);
            WireUp(Read, ReadPack);
            WireUp(Update, ReadPack);
            WireUp(Delete, ReadPack);
        }

        private void ReadPack(CrudAction response, BinaryReader br)
        {
            var t = br.ReadString();
            var i = br.ReadString();
            if (crudActionRegistry.TryGetValue((t, response), out var l))
            {
                foreach (var j in l)
                {
                    j.Invoke(i);
                }
            }
        }

        public ServiceResult NotifyPeers(CrudAction action, Model? entity)
        {
            using var ms = new MemoryStream();
            using var sw = new StreamWriter(ms);

            if (entity is { } e)
            {
                sw.Write(e.GetType().ResolveToDefinedType()!.FullName!);
                sw.Write(e.IdAsString);
            }
            else
            {
                sw.Write(string.Empty);
            }

            return Send(action, ms, OnResponse);
        }

        private ServiceResult OnResponse(CrudAction arg1, BinaryReader arg2)
        {
            return arg1 == Commit;
        }

        public void Register<T>(CrudAction crudAction, Action<string> action) where T : Model
        {
            var t = typeof(T).ResolveToDefinedType()!.FullName!;
            if (!crudActionRegistry.ContainsKey((t, crudAction)))
            {
                crudActionRegistry.Add((t, crudAction), new List<Action<string>>());
            }
            crudActionRegistry[(t, crudAction)].Add(action);
        }

        public void Register<TModel, TKey>(CrudAction crudAction, Action<TKey> action) where TModel : Model<TKey> where TKey : notnull, IComparable<TKey>, IEquatable<TKey>
        {
            Register<TModel>(crudAction, k => action((TKey)Common.FindConverter<TKey>()?.ConvertFromString(k) ?? default!));
        }
    }
}
