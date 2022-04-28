﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.InMemory.Services
{
    /// <summary>
    /// Representa una transacción de prueba que almacena los datos guardados
    /// en la memoria de la aplicación. Los datos almacenados no se persistirán
    /// y serán borrados al finalizar la ejecución.
    /// </summary>
    public class InMemoryCrudTransaction : AsyncDisposable, ICrudReadWriteTransaction
    {
        private static readonly List<Model> Models = new();

        private readonly List<Model> _temp = new();

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="InMemoryCrudTransaction"/>.
        /// </summary>
        /// <param name="configuration">
        /// Configuración de transacciones a utilizar.
        /// </param>
        public InMemoryCrudTransaction(IMiddlewareRunner configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Obtiene la configuración de transacciones que ha sido establecida
        /// en esta transacción.
        /// </summary>
        public IMiddlewareRunner Configuration { get; }

        /// <summary>
        /// Obtiene un <see cref="ServiceResult"/> con un Query de todas las
        /// entidades de la base de datos que corresponden a un modelo
        /// específico.
        /// </summary>
        /// <typeparam name="TModel">
        /// Modelo a obtener desde la base de datos.
        /// </typeparam>
        /// <returns>
        /// Un <see cref="IQueryable{TModel}"/> que representa el Query de
        /// todas las entidades del modelo especificado desde la base de datos.
        /// </returns>
        public QueryServiceResult<TModel> All<TModel>() where TModel : Model
        {
            lock (((ICollection)Models).SyncRoot)
            {
                return new QueryServiceResult<TModel>(Models.Concat(_temp).Distinct().OfType<TModel>().AsQueryable());
            }
        }
        
        /// <summary>
        /// Guarda los cambios en la base de datos de forma asíncrona.
        /// </summary>
        /// <returns></returns>
        public Task<ServiceResult> CommitAsync()
        {
            Models.AddRange(_temp.Where(p => !Models.Contains(p)));
            _temp.Clear();
            return Task.FromResult(ServiceResult.Ok);
        }

        /// <summary>
        /// Crea una nueva entidad en la base de datos.
        /// </summary>
        /// <typeparam name="TModel">Modelo de la nueva entidad.</typeparam>
        /// <param name="newEntity">
        /// Nueva entidad a agregar a la base de datos.
        /// </param>
        /// <returns>
        /// Un <see cref="ServiceResult"/> que representa el éxito o fracaso de
        /// la operación.
        /// </returns>
        public ServiceResult Create<TModel>(TModel newEntity) where TModel : Model
        {
            if (Models.Concat(_temp).Contains(newEntity)) return FailureReason.EntityDuplication;
            _temp.Add(newEntity);
            return ServiceResult.Ok;
        }

        /// <summary>
        /// Elimina una entidad de la base de datos.
        /// </summary>
        /// <typeparam name="TModel">Modelo de la entidad.</typeparam>
        /// <param name="entity">
        /// Entidad a eliminar de la base de datos.
        /// </param>
        /// <returns>
        /// Un <see cref="ServiceResult"/> que representa el éxito o fracaso de
        /// la operación.
        /// </returns>
        public ServiceResult Delete<TModel>(TModel entity) where TModel : Model
        {
            return !Models.Remove(entity) || _temp.Remove(entity) ? new ServiceResult(FailureReason.NotFound) : ServiceResult.Ok;
        }

        /// <summary>
        /// Elimina una entidad de la base de datos.
        /// </summary>
        /// <typeparam name="TModel">Modelo de la entidad.</typeparam>
        /// <typeparam name="TKey">
        /// Tipo del campo llave utilizado para identificar a la entidad.
        /// </typeparam>
        /// <param name="key">
        /// Valor del campo llave utilizado para identificar a la entidad.
        /// </param>
        /// <returns>
        /// Un <see cref="ServiceResult"/> que representa el éxito o fracaso de
        /// la operación.
        /// </returns>
        public ServiceResult Delete<TModel, TKey>(TKey key)
            where TModel : Model<TKey>
            where TKey : IComparable<TKey>, IEquatable<TKey>
        {
            return Models.Concat(_temp).FirstOrDefault(p => p.IdAsString == key.ToString()) is TModel e
                ? Delete(e)
                : new ServiceResult(FailureReason.NotFound);
        }

        /// <summary>
        /// Lee una entidad desde la base de datos.
        /// </summary>
        /// <typeparam name="TModel">Modelo de la entidad.</typeparam>
        /// <typeparam name="TKey">
        /// Tipo del campo llave utilizado para identificar a la entidad.
        /// </typeparam>
        /// <param name="key">
        /// Valor del campo llave utilizado para identificar a la entidad.
        /// </param>
        /// <returns>
        /// Un <see cref="ServiceResult"/> que representa el éxito o fracaso de
        /// la operación, incluyendo a la entidad obtenida.
        /// </returns>
        public Task<ServiceResult<TModel?>> ReadAsync<TModel, TKey>(TKey key)
            where TModel : Model<TKey>
            where TKey : notnull, IComparable<TKey>, IEquatable<TKey>
        {
            return Task.FromResult(Models.Concat(_temp).FirstOrDefault(p => p.IdAsString == key.ToString()) is TModel e
                ? new ServiceResult<TModel?>(e)
                : new ServiceResult<TModel?>(FailureReason.NotFound));
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<TModel[]?>> SearchAsync<TModel>(Expression<Func<TModel, bool>> predicate) where TModel : Model
        {
            return (await Models.OfType<TModel>().AsQueryable().Where(predicate).ToListAsync()).ToArray();
        }

        /// <summary>
        /// Actualiza los datos de una entidad existente.
        /// </summary>
        /// <typeparam name="TModel">Modelo de la entidad.</typeparam>
        /// <param name="entity">
        /// Entidad con los nuevos datos a ser escritos. Debe existir en la
        /// base de datos una entidad con el mismo Id de este objeto.
        /// </param>
        /// <returns>
        /// Un <see cref="ServiceResult"/> que representa el éxito o fracaso de
        /// la operación.
        /// </returns>
        public ServiceResult Update<TModel>(TModel entity) where TModel : Model
        {
            return Models.Concat(_temp).Contains(entity) ? ServiceResult.Ok : new ServiceResult(FailureReason.NotFound);
        }

        /// <summary>
        /// Libera los recursos desechables utilizados por esta instancia.
        /// </summary>
        protected override void OnDispose()
        {
            if (_temp.Any()) CommitAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Libera de forma asíncrona los recursos desechables utilizados por
        /// esta instancia.
        /// </summary>
        /// <returns>
        /// Un objeto que puede ser utilizado para monitorear el estado de la
        /// tarea.
        /// </returns>
        protected override async ValueTask OnDisposeAsync()
        {
            if (_temp.Any()) await CommitAsync().ConfigureAwait(false);
        }
    }
}
