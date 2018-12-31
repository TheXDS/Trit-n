using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheXDS.MCART;
using TheXDS.Triton.Annotations;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Component.Base;
using static TheXDS.MCART.Types.Extensions.EnumExtensions;
using static TheXDS.MCART.Types.Extensions.MemberInfoExtensions;
using static TheXDS.MCART.Types.Extensions.TaskExtensions;
using static TheXDS.MCART.Types.Extensions.TypeExtensions;
using St = TheXDS.Triton.Resources.Strings;

namespace TheXDS.Triton.Component
{
    public static class Configuration
    {
        public static int ServerTimeout = 15000;
    }

    /// <summary>
    ///     Clase base que provee de toda la funcionalidad de acceso a un
    ///     contexto de base de datos.
    /// </summary>
    public abstract class Service
    {
        /// <summary>
        ///     Enumera los posibles resultados de una operación provista por un servicio.
        /// </summary>
        [Flags]
        public enum Result
        {
            /// <summary>
            ///     Operación completada correctamente.
            /// </summary>
            Ok,

            /// <summary>
            ///     La operación falló debido a un error de validación.
            /// </summary>
            ValidationFault = 1,

            /// <summary>
            ///     La tarea no tuvo permisos para ejecutarse.
            /// </summary>
            Forbidden = 2,

            /// <summary>
            ///     No fue posible contactar con el servidor.
            /// </summary>
            Unreachable = 4,
            
            /// <summary>
            ///     Error del servidor.
            /// </summary>
            ServerFault = 8,

            /// <summary>
            ///     Error de la aplicación.
            /// </summary>
            AppFault = 16,

            /// <summary>
            ///     No se encontró ningún elemento a afectar por la operación.
            /// </summary>
            NoMatch = 32
        }

        /// <summary>
        ///     Obtiene un nombre amigable para un servicio.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [NotNull]
        public static string FriendlyName<T>() where T : Service
        {
            return string.Format(St.Service_Friendly_Name, typeof(T).NameOf());
        }

        /// <summary>
        ///     Contexto de datos asociado a esta instancia de la clase
        ///     <see cref="Service"/>.
        /// </summary>
        [NotNull] protected DbContext Context { get; }

        /// <summary>
        ///     Objeto de escritura de bitácora asociado a esta instancia.
        /// </summary>
        [CanBeNull] protected IDbLogger Logger { get; }

        /// <summary>
        ///     Objeto de elevación de seguridad asociado a esta instancia.
        /// </summary>
        [CanBeNull] protected ISecurityElevator Elevator { get; }

        /// <summary>
        ///     Realiza comprobaciones estandar sobre las llamadas regulares a
        ///     las funciones de este servicio.
        /// </summary>
        /// <typeparam name="TModel">
        ///     Tipo de entidad a manejar por el método. Puede omitirse si el
        ///     método no requiere manipular modelos de datos.
        /// </typeparam>
        /// <param name="failingResult">
        ///     Resultado fallido de la comprobación. Se establece en
        ///     <see langword="null"/> si la comprobación finaliza
        ///     correctamente.
        /// </param>
        /// <param name="caller">
        ///     Nombre del método que ha realizado la llamada. Se obtendrá un
        ///     <see cref="MethodInfo"/> que representa al método especificado.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> si la comprobación ha fallado, 
        ///     <see langword="false"/> en caso contrario. 
        /// </returns>
        protected bool PreChecksFail<TModel>([CanBeNull] out OperationResult failingResult, [CallerMemberName] string caller = null)
        {
            return PreChecksFail(out failingResult, typeof(TModel), caller);
        }

        /// <summary>
        ///     Realiza comprobaciones estandar sobre las llamadas regulares a
        ///     las funciones de este servicio.
        /// </summary>
        /// <typeparam name="TModel">
        ///     Tipo de entidad a manejar por el método. Puede omitirse si el
        ///     método no requiere manipular modelos de datos.
        /// </typeparam>
        /// <param name="failingResult">
        ///     Resultado fallido de la comprobación. Se establece en
        ///     <see langword="null"/> si la comprobación finaliza
        ///     correctamente.
        /// </param>
        /// <param name="caller">
        ///     Método que ha realizado la llamada.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> si la comprobación ha fallado, 
        ///     <see langword="false"/> en caso contrario. 
        /// </returns>
        protected bool PreChecksFail<TModel>([CanBeNull] out OperationResult failingResult, [NotNull] MethodBase caller)
        {
            return PreChecksFail(out failingResult, typeof(TModel), caller);
        }

        /// <summary>
        ///     Realiza comprobaciones estandar sobre las llamadas regulares a
        ///     las funciones de este servicio.
        /// </summary>
        /// <param name="failingResult">
        ///     Resultado fallido de la comprobación. Se establece en
        ///     <see langword="null"/> si la comprobación finaliza
        ///     correctamente.
        /// </param>
        /// <param name="objType">
        ///     Tipo de entidad a manejar por el método. Puede omitirse si el
        ///     método no requiere manipular modelos de datos.
        /// </param>
        /// <param name="caller">
        ///     Nombre del método que ha realizado la llamada. Se obtendrá un
        ///     <see cref="MethodInfo"/> que representa al método especificado.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> si la comprobación ha fallado, 
        ///     <see langword="false"/> en caso contrario. 
        /// </returns>
        protected bool PreChecksFail([CanBeNull] out OperationResult failingResult, [CanBeNull] Type objType, [CallerMemberName] string caller = null)
        {
            return PreChecksFail(out failingResult, objType, GetType().GetMethods().FirstOrDefault(p => p.Name == caller));
        }

        /// <summary>
        ///     Realiza comprobaciones estandar sobre las llamadas regulares a
        ///     las funciones de este servicio.
        /// </summary>
        /// <param name="failingResult">
        ///     Resultado fallido de la comprobación. Se establece en
        ///     <see langword="null"/> si la comprobación finaliza
        ///     correctamente.
        /// </param>
        /// <param name="caller">
        ///     Nombre del método que ha realizado la llamada. Se obtendrá un
        ///     <see cref="MethodInfo"/> que representa al método especificado.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> si la comprobación ha fallado, 
        ///     <see langword="false"/> en caso contrario. 
        /// </returns>
        protected bool PreChecksFail([CanBeNull] out OperationResult failingResult, [NotNull] MethodBase caller)
        {
            return PreChecksFail(out failingResult, null, caller);
        }

        /// <summary>
        ///     Realiza comprobaciones estandar sobre las llamadas regulares a
        ///     las funciones de este servicio.
        /// </summary>
        /// <param name="failingResult">
        ///     Resultado fallido de la comprobación. Se establece en
        ///     <see langword="null"/> si la comprobación finaliza
        ///     correctamente.
        /// </param>
        /// <param name="objType">
        ///     Tipo de entidad a manejar por el método. Puede omitirse si el
        ///     método no requiere manipular modelos de datos.
        /// </param>
        /// <param name="caller">
        ///     Método que ha realizado la llamada.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> si la comprobación ha fallado, 
        ///     <see langword="false"/> en caso contrario. 
        /// </returns>
        protected bool PreChecksFail([CanBeNull] out OperationResult failingResult, [CanBeNull] Type objType, [NotNull] MethodBase caller)
        {
            if (!Elevator?.Elevate(caller) ?? false)
            {
                failingResult = Result.Forbidden;
                return true;
            }
            if (!(objType is null || Handles(objType)))
            {
                failingResult = Result.AppFault;
                return true;
            }
            failingResult = null;
            return false;
        }
        
        #region Operaciones CRUD

        /// <summary>
        ///     Permite agregar una nueva entidad al contexto de datos de forma
        ///     asíncrona.
        /// </summary>
        /// <typeparam name="TModel">Tipo del modelo de datos.</typeparam>
        /// <param name="newEntity">Nueva entidad a añadir.</param>
        /// <returns>
        ///     Una tarea que devuelve un <see cref="OperationResult"/> que
        ///     indica el resultado de la operación.
        /// </returns>
        [MethodCategory(MethodCategory.New)]
        public Task<OperationResult> AddAsync<TModel>([NotNull] TModel newEntity) where TModel : ModelBase, new()
        {
            if (PreChecksFail<TModel>(out var fails, MethodBase.GetCurrentMethod())) return fails;
            Set<TModel>().Add(newEntity);
            return SaveAsync();
        }

        [MethodCategory(MethodCategory.New)]
        public OperationResult Add<TModel>([NotNull] TModel newEntity) where TModel : ModelBase, new()
        {
            return AddAsync(newEntity).Yield();
        }

        [MethodCategory(MethodCategory.New)]
        public Task<OperationResult> BulkAddAsync<TModel>([NotNull] [ItemNotNull] IEnumerable<TModel> entities) where TModel : ModelBase, new()
        {
            if (PreChecksFail<TModel>(out var fails, MethodBase.GetCurrentMethod())) return fails;
            Context.AddRange(entities);
            return SaveAsync();
        }

        [MethodCategory(MethodCategory.New)]
        public OperationResult BulkAdd<TModel>([NotNull] [ItemNotNull] IEnumerable<TModel> entities)
            where TModel : ModelBase, new()
        {
            return BulkAddAsync(entities).Yield();
        }

        [MethodCategory(MethodCategory.New)]
        public Task<OperationResult> BulkAddAsync([NotNull] [ItemNotNull] IEnumerable<object> entities)
        {
            return BulkOp(entities, (c, e) => c.AddRange(e), MethodBase.GetCurrentMethod());
        }

        [MethodCategory(MethodCategory.New)]
        public OperationResult BulkAdd([NotNull] [ItemNotNull] IEnumerable<object> entities)
        {
            return BulkAddAsync(entities).Yield();
        }

        [DebuggerStepThrough, CanBeNull]
        [MethodCategory(MethodCategory.View)]
        public IQueryable<TModel> All<TModel>(out OperationResult fails) where TModel : ModelBase, new()
        {
            if (PreChecksFail<TModel>(out fails, MethodBase.GetCurrentMethod())) return null;            

            var q = Set<TModel>() as IQueryable<TModel>;
            if (typeof(TModel).Implements<ISoftDeletable>())
            {
                q = q.OfType<ISoftDeletable>().Where(p => !p.IsDeleted).OfType<TModel>();
            }
            return q;
        }

        [DebuggerStepThrough, CanBeNull]
        [MethodCategory(MethodCategory.View)]
        public IQueryable<TModel> All<TModel>() where TModel : ModelBase, new() => All<TModel>(out _);

        [DebuggerStepThrough, CanBeNull]
        [MethodCategory(MethodCategory.View)]
        public IQueryable All(Type model, out OperationResult fails)
        {
            if (PreChecksFail(out fails, model, MethodBase.GetCurrentMethod()))
            {
                return null;
            }
            return Set<IQueryable>(model);
        }

        [DebuggerStepThrough, CanBeNull]
        [MethodCategory(MethodCategory.View)]
        public IQueryable All(Type model) => All(model, out _);

        [MethodCategory(MethodCategory.View), CanBeNull]
        public IQueryable<TModel> AllBase<TModel>(out OperationResult fails) where TModel : class
        {
            if (PreChecksFail<TModel>(out fails, MethodBase.GetCurrentMethod())) return null;
            return Context.GetType().GetProperties().Where(p => p.PropertyType.Implements(typeof(DbSet<TModel>)))
                .SelectMany(p => p.GetMethod.Invoke(Context, new object[0]) as IQueryable<TModel>) as IQueryable<TModel>;
        }

        [MethodCategory(MethodCategory.View), CanBeNull]
        public IQueryable<TModel> AllBase<TModel>() where TModel : class => AllBase<TModel>(out _);

        [MethodCategory(MethodCategory.View), NotNull]
        public async Task<OperationResult<List<TModel>>> AllAsync<TModel>() where TModel : ModelBase, new()
        {
            if (PreChecksFail<TModel>(out var fails, MethodBase.GetCurrentMethod())) return new OperationResult<List<TModel>>(fails);
            return await All<TModel>().ToListAsync();
        }

        [MethodCategory(MethodCategory.View)]
        public async Task<OperationResult<IQueryable>> AllAsync(Type model)
        {
            if (PreChecksFail(out var fails, model, MethodBase.GetCurrentMethod())) return new OperationResult<IQueryable>(fails);
            var l = new List<object>();
            void LoadToList()
            {
                foreach (var j in All(model)) l.Add(j);
            }
            await Task.Run(LoadToList);
            return (OperationResult<IQueryable>) l.AsQueryable();
        }

        [MethodCategory(MethodCategory.View)]
        public async Task<OperationResult<TModel>> GetAsync<TModel, TKey>(TKey id) 
            where TModel : ModelBase<TKey>, new() 
            where TKey : struct, IComparable<TKey>
        {
            if (PreChecksFail<TModel>(out var fails, MethodBase.GetCurrentMethod())) return new OperationResult<TModel>(fails);
            var entity = await Context.FindAsync<TModel>(id);
            if (entity is null || ((entity as ISoftDeletable)?.IsDeleted ?? false)) return new OperationResult<TModel>(Result.NoMatch);
            return entity;
        }

        [MethodCategory(MethodCategory.View)]
        public OperationResult<TModel> Get<TModel, TKey>(in TKey id)
            where TModel : ModelBase<TKey>, new()
            where TKey : struct, IComparable<TKey>
        {
            if (PreChecksFail<TModel>(out var fails, MethodBase.GetCurrentMethod())) return new OperationResult<TModel>(fails);
            var entity = Context.Find<TModel>(id);
            if (entity is null || ((entity as ISoftDeletable)?.IsDeleted ?? false)) return new OperationResult<TModel>(Result.NoMatch);
            return entity;
        }

        [MethodCategory(MethodCategory.View)]
        public async Task<OperationResult<ModelBase<TKey>>> GetAsync<TKey>(Type tModel, TKey id)
            where TKey : struct, IComparable<TKey>
        {
            if (PreChecksFail(out var fails, tModel, MethodBase.GetCurrentMethod())) return new OperationResult<ModelBase<TKey>>(fails);
            var entity = await Context.FindAsync(tModel, id) as ModelBase<TKey>;
            if (entity is null || ((entity as ISoftDeletable)?.IsDeleted ?? false)) return new OperationResult<ModelBase<TKey>>(Result.NoMatch);
            return entity;
        }

        [MethodCategory(MethodCategory.View)]
        public OperationResult<ModelBase<TKey>> Get<TKey>(Type tModel, in TKey id)
            where TKey : struct, IComparable<TKey>
        {
            if (PreChecksFail(out var fails, tModel, MethodBase.GetCurrentMethod())) return new OperationResult<ModelBase<TKey>>(fails);
            var entity = Context.Find(tModel, id) as ModelBase<TKey>;
            if (entity is null || ((entity as ISoftDeletable)?.IsDeleted ?? false)) return new OperationResult<ModelBase<TKey>>(Result.NoMatch);
            return entity;
        }

        [MethodCategory(MethodCategory.Delete)]
        public Task<OperationResult> DeleteAsync<TModel>([NotNull] TModel entity) where TModel : class, ISoftDeletable, new()
        {
            if (PreChecksFail<TModel>(out var fails, MethodBase.GetCurrentMethod())) return Task.FromResult(fails);
            entity.IsDeleted = true;
            return SaveAsync();
        }

        [MethodCategory(MethodCategory.Delete)]
        public virtual OperationResult Delete<TModel>([NotNull] TModel entity) where TModel : class, ISoftDeletable, new()
        {
            return DeleteAsync(entity).Yield();
        }

        [MethodCategory(MethodCategory.Delete)]
        public Task<OperationResult> BulkDeleteAsync<TModel>([NotNull] IEnumerable<TModel> entities)
            where TModel : class, ISoftDeletable, new()
        {
            if (PreChecksFail<TModel>(out var fails, MethodBase.GetCurrentMethod())) return Task.FromResult(fails);
            foreach (var j in entities)
            {
                j.IsDeleted = true;
            }
            return SaveAsync();
        }

        [MethodCategory(MethodCategory.Delete)]
        public OperationResult BulkDelete<TModel>([NotNull] IEnumerable<TModel> entities) 
            where TModel : class, ISoftDeletable, new()
        {
            return BulkDeleteAsync(entities).Yield();
        }

        [MethodCategory(MethodCategory.Delete)]
        public Task<OperationResult> PurgeAsync<TModel>([NotNull] TModel entity) where TModel : ModelBase, new()
        {
            if (PreChecksFail<TModel>(out var fails, MethodBase.GetCurrentMethod())) return Task.FromResult(fails);
            Set<TModel>().Remove(entity);
            return SaveAsync();
        }

        [MethodCategory(MethodCategory.Delete)]
        public virtual OperationResult Purge<TModel>([NotNull] TModel entity) where TModel : ModelBase, new()
        {
            return PurgeAsync(entity).Yield();
        }

        [MethodCategory(MethodCategory.Delete)]
        public Task<OperationResult> BulkPurgeAsync<TModel>([NotNull] IEnumerable<TModel> entities)
            where TModel : ModelBase, new()
        {
            if (PreChecksFail<TModel>(out var fails, MethodBase.GetCurrentMethod())) return fails;
            Set<TModel>().RemoveRange(entities);
            return SaveAsync();
        }

        [MethodCategory(MethodCategory.Delete)]
        public OperationResult BulkPurge<TModel>([NotNull] IEnumerable<TModel> entities)
            where TModel : ModelBase, new()
        {
            return BulkPurgeAsync(entities).Yield();
        }

        [MethodCategory(MethodCategory.Delete)]
        public Task<OperationResult> BulkPurgeAsync([NotNull] IEnumerable<object> entities)
        {
            return BulkOp(entities, (c, e) => c.RemoveRange(e), MethodBase.GetCurrentMethod());
        }

        [MethodCategory(MethodCategory.Delete)]
        public OperationResult BulkPurge([NotNull] IEnumerable<object> entities)
        {
            return BulkPurgeAsync(entities).Yield();
        }

        [MethodCategory(MethodCategory.Admin)]
        public Task<OperationResult> UndeleteAsync<TModel, TKey>(in TKey id) 
            where TModel : ModelBase<TKey>, ISoftDeletable, new()
            where TKey : struct, IComparable<TKey>
        {
            if (PreChecksFail<TModel>(out var fails, MethodBase.GetCurrentMethod())) return fails;
            var entity = Context.Find<TModel>(id);
            if (entity is null) return (OperationResult) Result.NoMatch;
            if (!entity.IsDeleted) return (OperationResult) Result.ValidationFault;
            entity.IsDeleted = false;
            return SaveAsync();
        }

        [MethodCategory(MethodCategory.Admin)]
        public virtual OperationResult Undelete<TModel, TKey>(in TKey id)
            where TModel : ModelBase<TKey>, ISoftDeletable, new()
            where TKey : struct, IComparable<TKey>
        {
            return UndeleteAsync<TModel,TKey>(id).Yield();
        }

        private Task<OperationResult> BulkOp([NotNull, ItemNotNull] IEnumerable<object> entities, [NotNull] Action<DbContext, IEnumerable<object>> action, [NotNull] MethodBase callee)
        {
            if (PreChecksFail(out var fails, callee)) return fails;

            var ents = entities as object[] ?? entities.ToArray();
            var tpes = ents.ToTypes().Distinct().ToArray();

            if (tpes.Any(j => !Handles(j)))
                return (OperationResult) Result.ValidationFault;

            try
            {
                foreach (var j in tpes)
                    action(Context, ents.Where(p => p.GetType() == j));
            }
            catch (Exception e)
            {
                return new OperationResult(Result.AppFault, e.Message);
            }

            return SaveAsync();
        }
        
        #endregion
        
        public bool ChangesPending()
        {
            return Context.ChangeTracker.Entries().Any(p => p.State != EntityState.Unchanged);
        }

        public bool ChangesPending<TModel>([NotNull]TModel entity) where TModel : ModelBase, new()
        {
            return Context.ChangeTracker.Entries().Any(p => p.Entity == entity && p.State != EntityState.Unchanged);
        }
        
        public async Task<OperationResult> SaveAsync()
        {
            var cs = new CancellationTokenSource(Configuration.ServerTimeout);
            try
            {
                if (!ChangesPending()) return Result.Ok;
                await (Logger?.LogAsync(Context.ChangeTracker) ?? Task.CompletedTask);
                await Context.SaveChangesAsync(cs.Token);
                return !cs.IsCancellationRequested ? Result.Ok : Result.Unreachable;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return new OperationResult(Result.ValidationFault, ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return new OperationResult(Result.ServerFault, ex.Message);
            }
            catch (Exception ex)
            {
                return new OperationResult(Result.AppFault, ex.Message);
            }
        }

        public bool Handles([NotNull]Type tModel)
        {
            var dbType = typeof(DbSet<>).MakeGenericType(tModel);
            return Context.GetType().GetProperties().Any(p => dbType.IsAssignableFrom(p.PropertyType));
        }

        public bool Handles<TModel>() where TModel : class
        {
            return Handles(typeof(TModel));
        }

        [NotNull]
        private DbSet<TModel> Set<TModel>() where TModel : ModelBase, new()
        {
            return Context.Set<TModel>() ?? throw new InvalidOperationException(St.Service_doesnt_handle_model);
        }
        [NotNull]
        private T Set<T>([NotNull]Type model) where T:class
        {
            var t = typeof(DbSet<>).MakeGenericType(model);

            var prop = Context.GetType().GetProperties().FirstOrDefault(p => p.PropertyType.Implements(t))
                   ?? throw new InvalidOperationException(St.Service_doesnt_handle_model);
            return (prop.GetMethod?.Invoke(Context, new object[0]) as T)
                   ?? throw new InvalidOperationException(St.Service_doesnt_handle_model);
        }
    }

    [Serializable]
    public class OperationResultException : Exception
    {
        /// <summary>
        ///     Resultado arrojado por la operación
        /// </summary>
        public OperationResult Result { get; }

        /// <inheritdoc />
        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="T:TheXDS.Triton.Component.OperationResultException" />
        /// </summary>
        /// <param name="result">
        ///     Resultado de la operación.
        /// </param>
        public OperationResultException(OperationResult result) : base(result.Message)
        {
            Result = result;
        }

        public OperationResultException(OperationResult result, Exception inner) : base(result.Message, inner)
        {
            Result = result;
        }

        public OperationResultException(Service.Result result) : this(result, result.NameOf())
        {
        }

        public OperationResultException(Service.Result result, string message) : base(message)
        {
            Result = new OperationResult(result, message);
        }

        public OperationResultException(Service.Result result, string message, Exception inner) : base(message, inner)
        {
            Result = new OperationResult(result, message);
        }

        /// <inheritdoc />
        protected OperationResultException(SerializationInfo info,StreamingContext context) : base(info, context)
        {
        }
    }

    public class OperationResult : IEquatable<OperationResult>
    {
        public OperationResult(Service.Result result) : this(result, St.Service_Result_Ok)
        {
        }

        public OperationResult(Service.Result result, string message)
        {
            Result = result;
            Message = message;
        }

        public Service.Result Result { get; }

        public string Message { get; }

        public static implicit operator OperationResult(Service.Result result)
        {
            string msj;
            switch (result) {
                case Service.Result.Ok:
                    msj = St.Service_Result_Ok;
                    break;
                case Service.Result.ValidationFault:
                    msj = St.Service_Result_ValidationFault;
                    break;
                case Service.Result.Forbidden:
                    msj = St.Service_Result_Forbidden;
                    break;
                case Service.Result.Unreachable:
                    msj = St.Service_Result_Unreachable;
                    break;
                case Service.Result.ServerFault:
                    msj = St.Service_Result_ServerFault;
                    break;
                case Service.Result.AppFault:
                    msj = St.Service_Result_AppFault;
                    break;
                case Service.Result.NoMatch:
                    msj = St.Service_Result_NoMatch;
                    break;
                default:
                    msj = string.Format(St.Service_Result_Unk, result.NameOf());
                    break;
            }
            return new OperationResult(result,msj);
        }

        public static implicit operator Service.Result(OperationResult result) => result.Result;

        public static implicit operator Task<OperationResult>(OperationResult result) => Task.FromResult(result);

        public static implicit operator OperationResultException(OperationResult result) => new OperationResultException(result);

        public static explicit operator OperationResult(OperationResultException result) => result.Result;

        /// <inheritdoc />
        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
        public bool Equals(OperationResult other)
        {
            return Result == other.Result;
        }

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>true if <paramref name="obj">obj</paramref> and this instance are the same type and represent the same value; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj is OperationResult other && Equals(other);
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return (int) Result;
        }
    }

    public class OperationResult<T> : OperationResult
    {
        public OperationResult(OperationResult result) : this(result, default)
        {
        }

        public OperationResult() : this(default(T))
        {
        }

        public OperationResult(T relatedData) : this(Service.Result.Ok, relatedData)
        {
        }

        public OperationResult(OperationResult result, T relatedData) : base(result.Result)
        {
            RelatedData = relatedData;
        }

        public OperationResult(Service.Result result, T relatedData) : base(result)
        {
            RelatedData = relatedData;
        }

        public OperationResult(Service.Result result, string message, T relatedData) : base(result, message)
        {
            RelatedData = relatedData;
        }

        public T RelatedData { get; }

        public static implicit operator Service.Result(OperationResult<T> result)
        {
            return result.Result;
        }

        public static implicit operator OperationResult<T>(Service.Result result)
        {
            return new OperationResult<T>(result, default);
        }

        public static implicit operator T(OperationResult<T> result)
        {
            return result.RelatedData;
        }

        public static implicit operator OperationResult<T>(T relatedData)
        {
            return new OperationResult<T>(Service.Result.Ok, relatedData);
        }

        public static implicit operator Task<OperationResult<T>>(OperationResult<T> result) => Task.FromResult(result);

    }
}