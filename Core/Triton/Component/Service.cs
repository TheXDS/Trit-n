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
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TheXDS.MCART;
using TheXDS.MCART.Attributes;
using static TheXDS.MCART.Types.Extensions.MemberInfoExtensions;
using static TheXDS.MCART.Types.Extensions.TaskExtensions;
using TheXDS.Triton.Annotations;
using TheXDS.Triton.Models.Base;
using static TheXDS.MCART.Types.Extensions.EnumExtensions;
using static TheXDS.MCART.Types.Extensions.TypeExtensions;
using St = TheXDS.Triton.Resources.Strings;

namespace TheXDS.Triton.Component
{
    public static class Configuration
    {
        public static int ServerTimeout = 15000;
    }

    /// <summary>
    ///     Define una serie de miembros a implementar por una clase que
    ///     permita el registro de información de bitácora sobre los cambios
    ///     realizados en una base de datos.
    /// </summary>
    public interface IDbLogger
    {
        /// <summary>
        ///     Escribe una nueva entrada de bitácora con la información sobre
        ///     los cambios realizados en una base de datos desde el último
        ///     guardado.
        /// </summary>
        /// <param name="changes">
        ///     Objeto que contiene una colección de seguimiento de los cambios
        ///     realizados en la base de datos.
        /// </param>
        void Log(ChangeTracker changes);
        /// <summary>
        ///     Escribe de forma asíncrona una nueva entrada de bitácora con la
        ///     información sobre los cambios realizados en una base de datos
        ///     desde el último guardado.
        /// </summary>
        /// <param name="changes">
        ///     Objeto que contiene una colección de seguimiento de los cambios
        ///     realizados en la base de datos.
        /// </param>
        /// <returns>
        ///     Una tarea que permite observar el estado de la operación.
        /// </returns>
        Task LogAsync(ChangeTracker changes);
    }

    /// <summary>
    ///     Define una serie de miembros a implementar por una clase que pueda
    ///     otorgar o denegar solicitudes de elevación de permisos para
    ///     ejecutar un método específico.
    /// </summary>
    public interface ISecurityElevator
    {
        bool Elevate(MethodBase method);
    }

    /// <inheritdoc />
    /// <summary>
    ///     Atributo que anota un valor que describe la categoría a la que un
    ///     método pertenece, permitiendo o denegando la ejecución del mismo.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MethodCategoryAttribute : Attribute, IValueAttribute<MethodCategory>
    {
        /// <inheritdoc />
        /// <summary>
        ///     Marca un método con el atributo de categoría de seguridad
        ///     especificado.
        /// </summary>
        /// <param name="value">
        ///     Categoría de seguridad a la que el método pertenece.
        /// </param>
        public MethodCategoryAttribute(MethodCategory value)
        {
            Value = value;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Obtiene el valor de este atributo.
        /// </summary>
        public MethodCategory Value { get; }
    }

    /// <summary>
    ///     Contiene un registro de los métodos con atributos de seguridad
    ///     registrados por la aplicación.
    /// </summary>
    public static class FunctionRegistry
    {
        private static readonly HashSet<Type> RegisteredTypes = new HashSet<Type>();
        private static readonly Dictionary<MethodInfo, MethodCategory> Funcs = new Dictionary<MethodInfo, MethodCategory>();

        /// <summary>
        ///     Diccionario de sólo lectura que contiene a todos los métodos
        ///     registrados para participar el el mecanismo de seguridad junto
        ///     a un valor que describe la categoría de seguridad del mismo.
        /// </summary>
        public static IReadOnlyDictionary<MethodInfo, MethodCategory> Functions => Funcs;
        
        /// <summary>
        ///     Registra los métodos del tipo especificado para participar en
        ///     el mecanismo de seguridad de <see cref="TheXDS.Triton"/>.
        /// </summary>
        /// <typeparam name="T">Tipo a registrar.</typeparam>
        [Thunk]
        public static void Register<T>()
        {
            Register(typeof(T));
        }

        /// <summary>
        ///     Registra los métodos del tipo especificado para participar en
        ///     el mecanismo de seguridad de <see cref="TheXDS.Triton"/>.
        /// </summary>
        /// <param name="t">Tipo a registrar.</param>
        public static void Register(Type t)
        {
            lock (RegisteredTypes)
            lock (Funcs)
            {
                    if (RegisteredTypes.Contains(t)) return;
                RegisteredTypes.Add(t);
                foreach (var j in t.GetMethods())
                {
                    Funcs.Add(j,j.GetAttr<MethodCategoryAttribute>()?.Value ?? MethodCategory.Unspecified);
                }
            }
        }

        /// <summary>
        ///     Quita del registro del mecanismo de seguridad a los métodos del
        ///     tipo especificado.
        /// </summary>
        /// <typeparam name="T">Tipo a remover del registro.</typeparam>
        [Thunk]
        public static void UnRegister<T>()
        {
            UnRegister(typeof(T));
        }

        /// <summary>
        ///     Quita del registro del mecanismo de seguridad a los métodos del
        ///     tipo especificado.
        /// </summary>
        /// <param name="t">Tipo a remover del registro.</param>
        public static void UnRegister(Type t)
        {
            lock (RegisteredTypes)
            lock (Funcs)
            {
                if (!RegisteredTypes.Contains(t)) return;
                RegisteredTypes.Remove(t);
                foreach (var j in t.GetMethods())
                {
                    Funcs.Remove(j);
                }
            }
        }
    }

    /// <summary>
    ///     Describe la categoría a la cual un método pertenece.
    /// </summary>
    [Flags]
    public enum MethodCategory
    {
        /// <summary>
        ///     Método cuya familia no ha sido especificada, o método de funcionalidad genérica.
        /// </summary>
        Unspecified,

        /// <summary>
        ///     Método de apertura de vista.
        /// </summary>
        Show,

        /// <summary>
        ///     Método de lectura de datos.
        /// </summary>
        View,

        /// <summary>
        ///     Método genérico de lectura.
        /// </summary>
        Read,

        /// <summary>
        ///     Método de creación.
        /// </summary>
        New,

        /// <summary>
        ///     Método de edición.
        /// </summary>
        Edit = 8,

        /// <summary>
        ///     Método de borrado.
        /// </summary>
        Delete = 16,

        /// <summary>
        ///     Método genérico de escritura.
        /// </summary>
        Write = New | Edit | Delete,

        /// <summary>
        ///     Método genérico de lectura/escritura.
        /// </summary>
        ReadWrite = Read | Write,

        /// <summary>
        ///     Método de función adicional (herramienta)
        /// </summary>
        Tool = 32,

        /// <summary>
        ///     Método de función de configuración.
        /// </summary>
        Config = 64,

        /// <summary>
        ///     Método de permisos de supervisor.
        /// </summary>
        Manager = ReadWrite | Tool,

        /// <summary>
        ///     Método de función administrativa.
        /// </summary>
        Admin = Manager | Config,
        /// <summary>
        ///     Método restringido a su invocación por super-usuarios
        /// </summary>
        Root = -1
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

        protected bool PreChecksFail(out OperationResult failingResult, Type objType = null, [CallerMemberName] string caller = null)
        {
            return PreChecksFail(out failingResult, objType, GetType().GetMethods().FirstOrDefault(p=>p.Name==caller));
        }

        protected bool PreChecksFail<TModel>(out OperationResult failingResult, [CallerMemberName] string caller = null)
        {
            return PreChecksFail(out failingResult, typeof(TModel), caller);
        }

        protected bool PreChecksFail<TModel>(out OperationResult failingResult, MethodBase caller)
        {
            return PreChecksFail(out failingResult, typeof(TModel), caller);
        }

        protected bool PreChecksFail(out OperationResult failingResult, MethodBase caller)
        {
            return PreChecksFail(out failingResult, null, caller);
        }

        protected bool PreChecksFail(out OperationResult failingResult, Type objType, MethodBase caller)
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
        /// <returns></returns>
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

        [DebuggerStepThrough]
        [MethodCategory(MethodCategory.View)]
        public IQueryable<TModel> All<TModel>() where TModel : ModelBase, new()
        {
            if (PreChecksFail<TModel>(out var fails, MethodBase.GetCurrentMethod())) return new OperationResult<TModel>(fails);

            var q = Set<TModel>() as IQueryable<TModel>;
            if (typeof(TModel).Implements<ISoftDeletable>())
            {
                q = q.OfType<ISoftDeletable>().Where(p => !p.IsDeleted).OfType<TModel>();
            }
            return q;
        }

        [DebuggerStepThrough]
        [MethodCategory(MethodCategory.View)]
        public IQueryable All(Type model)
        {
            if (PreChecksFail(out var fails, model, MethodBase.GetCurrentMethod())) return new OperationResult<TModel>(fails);
            return Set<IQueryable>(model);
        }

        [MethodCategory(MethodCategory.View)]
        public IQueryable<TModel> AllBase<TModel>() where TModel : class
        {
            if (PreChecksFail<TModel>(out var fails, MethodBase.GetCurrentMethod())) return new OperationResult<TModel>(fails);
            return Context.GetType().GetProperties().Where(p => p.PropertyType.Implements(typeof(DbSet<TModel>)))
                .SelectMany(p => p.GetMethod.Invoke(Context, new object[0]) as IQueryable<TModel>) as IQueryable<TModel>;
        }

        [MethodCategory(MethodCategory.View)]
        public async Task<OperationResult<List<TModel>>> AllAsync<TModel>() where TModel : ModelBase, new()
        {
            if (PreChecksFail<TModel>(out var fails, MethodBase.GetCurrentMethod())) return new OperationResult<List<TModel>>(fails);
            return await All<TModel>().ToListAsync();
        }

        [MethodCategory(MethodCategory.View)]
        public async Task<IQueryable> AllAsync(Type model)
        {
            if (PreChecksFail(out var fails, model, MethodBase.GetCurrentMethod())) return new OperationResult<TModel>(fails);
            var l = new List<object>();
            void LoadToList()
            {
                foreach (var j in All(model)) l.Add(j);
            }
            await Task.Run(LoadToList);
            return l.AsQueryable();
        }

        [MethodCategory(MethodCategory.View)]
        public async Task<OperationResult<TModel>> Get<TModel, TKey>(TKey id) 
            where TModel : ModelBase<TKey>, new() 
            where TKey : struct, IComparable<TKey>
        {
            if (PreChecksFail<TModel>(out var fails, MethodBase.GetCurrentMethod())) return new OperationResult<TModel>(fails);
            var entity = await Context.FindAsync<TModel>(id);
            if (entity is null || ((entity as ISoftDeletable)?.IsDeleted ?? false)) return new OperationResult<TModel>(Result.NoMatch);
            return entity;
        }
        [MethodCategory(MethodCategory.View)]
        public async Task<OperationResult<ModelBase<TKey>>> Get<TKey>(Type tModel, TKey id)
            where TKey : struct, IComparable<TKey>
        {
            if (PreChecksFail(out var fails, tModel, MethodBase.GetCurrentMethod())) return new OperationResult<ModelBase<TKey>>(fails);


            var entity = await Context.FindAsync<TModel>(id);
            if (entity is null || ((entity as ISoftDeletable)?.IsDeleted ?? false)) return new OperationResult<TModel>(Result.NoMatch);
            return entity;
        }






        //public Task<OperationResult> UpdateAsync<TModel>([NotNull] TModel entity) where TModel : ModelBase, new()
        //{
        //    if (PreChecksFail<TModel>(out var fails)) return Task.FromResult(fails);
        //    return !ChangesPending(entity) ? Task.FromResult((OperationResult) Result.NoMatch) : SaveAsync();
        //}
        //public virtual OperationResult Update<TModel>([NotNull] TModel entity) where TModel : ModelBase, new()
        //{
        //    return UpdateAsync(entity).Yield();
        //}

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

        private Task<OperationResult> BulkOp(IEnumerable<object> entities, Action<DbContext,IEnumerable<object>> action, MethodBase callee)
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

        [MethodCategory(MethodCategory.Admin)]
        public Task<OperationResult> UndeleteAsync<TModel, TKey>(TKey id) 
            where TModel : ModelBase<TKey>, ISoftDeletable, new()
            where TKey : struct, IComparable<TKey>
        {
            if (PreChecksFail<TModel>(out var fails, MethodBase.GetCurrentMethod())) return fails;
            var entity = Context.Find<TModel>(id);
            if (entity is null) return (OperationResult)Result.NoMatch;
            if (!entity.IsDeleted) return (OperationResult)Result.ValidationFault;
            entity.IsDeleted = false;
            return SaveAsync();
        }
        [MethodCategory(MethodCategory.Admin)]
        public virtual OperationResult Undelete<TModel, TKey>(TKey id)
            where TModel : ModelBase<TKey>, ISoftDeletable, new()
            where TKey : struct, IComparable<TKey>
        {
            return UndeleteAsync<TModel,TKey>(id).Yield();
        }














        #endregion
        
        public bool ChangesPending()
        {
            return Context.ChangeTracker.Entries().Any(p => p.State != EntityState.Unchanged);
        }
        public bool ChangesPending<TModel>(TModel entity) where TModel : ModelBase, new()
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

        public bool Handles(Type tModel)
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





    [Serializable]
    public class OperationResultException : Exception
    {
        /// <summary>
        ///     Resultado arrojado por la operación
        /// </summary>
        public OperationResult Result { get; }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="OperationResultException"/>
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