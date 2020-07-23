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
    /// <summary>
    /// Delegado que describe una acción a ejecutar al recibirse una notificación de una acción Crud sobre un modelo.
    /// </summary>
    /// <param name="Id">
    /// Id de la entidad afectada por la acción Crud. El modelo y la acción se
    /// conocen con anterioridad.
    /// </param>
    public delegate void NotificationCallback(string Id);

    /// <summary>
    /// Cliente de notificaciones Crud que permite registrar callbacks a
    /// ejecutar al recibirse una notificación de una acción Crud desde la red.
    /// </summary>
    public class BaronNotifier : ManagedCommandClient<CrudAction, CrudAction>, ICrudNotifier
    {
        private readonly Dictionary<(string Model, CrudAction Action), List<NotificationCallback>> crudActionRegistry = new Dictionary<(string, CrudAction), List<NotificationCallback>>();

        /// <summary>
        /// Inicializa la clase <see cref="BaronNotifier"/>.
        /// </summary>
        static BaronNotifier()
        {
            ScanTypeOnCtor = false;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="BaronNotifier"/>.
        /// </summary>
        public BaronNotifier()
        {
            WireUp(Create, ReadPack);
            WireUp(Read, ReadPack);
            WireUp(Update, ReadPack);
            WireUp(Delete, ReadPack);
        }

        /// <summary>
        /// Envía una notificación de acción Crud al servidor para su
        /// distribución a los respectivos pares suscritos.
        /// </summary>
        /// <param name="action">Acción de Crud realizada.</param>
        /// <param name="entity">
        /// Entidad sobre la cual se ha realizado una acción Crud.
        /// </param>
        /// <returns>
        /// <see cref="ServiceResult.Ok"/> si la notificación ha sido exitosa,
        /// o un <see cref="ServiceResult"/> fallido en caso contrario.
        /// </returns>
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

        /// <summary>
        /// Registra una función a ser llamada al recibirse una notificación de
        /// acción crud correspondiente.
        /// </summary>
        /// <typeparam name="T">
        /// Modelo para el cual registrar la llamada.
        /// </typeparam>
        /// <param name="crudAction">
        /// Acción para la cual registrar la llamada.
        /// </param>
        /// <param name="action">
        /// Acción a llamar al recibir una notificación para el modelo y acción
        /// especificadas.
        /// </param>
        public void Register<T>(CrudAction crudAction, NotificationCallback action) where T : Model
        {
            var t = typeof(T).ResolveToDefinedType()!.FullName!;
            if (!crudActionRegistry.ContainsKey((t, crudAction)))
            {
                crudActionRegistry.Add((t, crudAction), new List<NotificationCallback>());
            }
            crudActionRegistry[(t, crudAction)].Add(action);
        }

        /// <summary>
        /// Registra una función a ser llamada al recibirse una notificación de
        /// acción crud correspondiente.
        /// </summary>
        /// <typeparam name="TModel">
        /// Modelo para el cual registrar la llamada.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// Tipo de campo llave del modelo.
        /// </typeparam>
        /// <param name="crudAction">
        /// Acción para la cual registrar la llamada.
        /// </param>
        /// <param name="action">
        /// Acción a llamar al recibir una notificación para el modelo y acción
        /// especificadas.
        /// </param>
        public void Register<TModel, TKey>(CrudAction crudAction, Action<TKey> action) where TModel : Model<TKey> where TKey : notnull, IComparable<TKey>, IEquatable<TKey>
        {
            Register<TModel>(crudAction, (NotificationCallback)(k => action((TKey)Common.FindConverter<TKey>()?.ConvertFromString(k) ?? default!)));
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

        private ServiceResult OnResponse(CrudAction arg1, BinaryReader arg2)
        {
            return arg1 == Commit;
        }
    }
}
