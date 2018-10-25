using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TheXDS.MCART;
using TheXDS.MCART.Attributes;
using TheXDS.Triton.Annotations;
using TheXDS.Triton.Models.Base;
using Ee= TheXDS.MCART.Types.Extensions.EnumExtensions;
using MemberInfoExtensions = TheXDS.MCART.Types.Extensions.MemberInfoExtensions;
using St = TheXDS.Triton.Resources.Strings;
using TaskExtensions = TheXDS.MCART.Types.Extensions.TaskExtensions;

namespace TheXDS.Triton.Component
{
    public static class Configuration
    {
        public static int ServerTimeout = 15000;
    }
    public interface IDbLogger
    {
        void Log(ChangeTracker changes);
        Task LogAsync(ChangeTracker changes);
    }
    public interface ISecurityElevator
    {
        bool Elevate(MethodInfo method);
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MethodCategoryAttribute : Attribute, IValueAttribute<FunctionRegistry.MethodCategory>
    {
        public MethodCategoryAttribute(FunctionRegistry.MethodCategory value)
        {
            Value = value;
        }

        /// <inheritdoc />
        /// <summary>Obtiene el valor de este atributo.</summary>
        public FunctionRegistry.MethodCategory Value { get; }
    }
    public static class FunctionRegistry
    {
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
            Write=New|Edit|Delete,

            /// <summary>
            ///     Método genérico de lectura/escritura.
            /// </summary>
            ReadWrite = Read | Write,

            /// <summary>
            ///     Método de función adicional (herramienta)
            /// </summary>
            Tool = 32,

            /// <summary>
            ///     Método de función administrativa.
            /// </summary>
            Admin = ReadWrite | Tool,

            /// <summary>
            ///     Método restringido a su invocación por superusuarios
            /// </summary>
            Root = -1
        }

        public static IReadOnlyDictionary<MethodInfo, MethodCategory> Functions => Funcs;

        private static readonly HashSet<Type> RegisteredTypes = new HashSet<Type>();
        private static readonly Dictionary<MethodInfo, MethodCategory> Funcs = new Dictionary<MethodInfo, MethodCategory>();

        public static void Register<T>()
        {
            var t = typeof(T);
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
        public static void UnRegister<T>()
        {
            var t = typeof(T);
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
            return string.Format(St.Service_Friendly_Name, MemberInfoExtensions.NameOf(typeof(T)));
        }

        [NotNull] protected abstract DbContext Context { get; }
        protected abstract IDbLogger Logger { get; }
        protected abstract ISecurityElevator Elevator { get; }

        private bool PreChecksFail(out OperationResult failingResult, Type objType = null, [CallerMemberName] string caller = null)
        {
            if (!Elevator?.Elevate(GetType().GetMethods().FirstOrDefault(p=>p.Name==caller)) ?? false)
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

        private bool PreChecksFail<TModel>(out OperationResult failingResult, [CallerMemberName] string caller = null)
        {
            return PreChecksFail(out failingResult, typeof(TModel),caller);
        }

        #region Operaciones CRUD

        [MethodCategory(FunctionRegistry.MethodCategory.New)]
        public Task<OperationResult> AddAsync<TModel>([NotNull] TModel newEntity) where TModel : class, new()
        {
            if (PreChecksFail<TModel>(out var fails)) return Task.FromResult(fails);
            Set<TModel>().Add(newEntity);
            return SaveAsync();
        }
        [MethodCategory(FunctionRegistry.MethodCategory.New)]
        public OperationResult Add<TModel>([NotNull] TModel newEntity) where TModel : class, new()
        {
            return TaskExtensions.Yield(AddAsync(newEntity));
        }
        [MethodCategory(FunctionRegistry.MethodCategory.New)]
        public Task<OperationResult> BulkAddAsync<TModel>([NotNull] [ItemNotNull] IEnumerable<TModel> entities) where TModel : class, new()
        {
            if (PreChecksFail<TModel>(out var fails)) return Task.FromResult(fails);
            Context.AddRange(entities);
            return SaveAsync();
        }
        [MethodCategory(FunctionRegistry.MethodCategory.New)]
        public OperationResult BulkAdd<TModel>([NotNull] [ItemNotNull] IEnumerable<TModel> entities)
            where TModel : class, new()
        {
            return TaskExtensions.Yield(BulkAddAsync(entities));
        }
        [MethodCategory(FunctionRegistry.MethodCategory.New)]
        public Task<OperationResult> BulkAddAsync([NotNull] [ItemNotNull] IEnumerable<object> entities)
        {
            var ents = entities as object[] ?? entities.ToArray();
            var tpes = ents.ToTypes().Distinct().ToArray();

            foreach (var j in tpes)
                if (PreChecksFail(out var fails,j)) return Task.FromResult(fails);

            foreach (var j in tpes)
                Context.AddRange(ents.Where(p => p.GetType() == j));

            return SaveAsync();
        }
        [MethodCategory(FunctionRegistry.MethodCategory.New)]
        public OperationResult BulkAdd([NotNull] [ItemNotNull] IEnumerable<object> entities)
        {
            return TaskExtensions.Yield(BulkAddAsync(entities));
        }






        public Task<OperationResult> UpdateAsync<TModel>([NotNull] TModel entity) where TModel : class, new()
        {
            if (PreChecksFail<TModel>(out var fails)) return Task.FromResult(fails);
            return !ChangesPending(entity) ? Task.FromResult((OperationResult) Result.NoMatch) : SaveAsync();
        }
        public virtual OperationResult Update<TModel>([NotNull] TModel entity) where TModel : class, new()
        {
            return TaskExtensions.Yield(UpdateAsync(entity));
        }

        public Task<OperationResult> DeleteAsync<TModel>([NotNull] TModel entity) where TModel : class, ISoftDeletable, new()
        {
            if (PreChecksFail<TModel>(out var fails)) return Task.FromResult(fails);
            entity.IsDeleted = true;
            return UpdateAsync(entity);
        }
        public virtual OperationResult Delete<TModel>([NotNull] TModel entity) where TModel : class, ISoftDeletable, new()
        {
            return TaskExtensions.Yield(DeleteAsync(entity));
        }

        public Task<OperationResult> PurgeAsync<TModel>([NotNull] TModel entity) where TModel : class, new()
        {
            if (PreChecksFail<TModel>(out var fails)) return Task.FromResult(fails);
            Set<TModel>().Remove(entity);
            return SaveAsync();
        }


        public virtual OperationResult Purge<TModel>([NotNull] TModel entity) where TModel : class, new()
        {
            return TaskExtensions.Yield(PurgeAsync(entity));
        }

        public Task<OperationResult> UndeleteAsync<TModel, TKey>(TKey id) 
            where TModel : ModelBase<TKey>, ISoftDeletable, new()
            where TKey : struct, IComparable<TKey>
        {
            TModel entity= null;

            if (PreChecksFail<TModel>(out var fails)) return Task.FromResult(fails);
            if (entity is null) return Task.FromResult((OperationResult)Result.NoMatch);
            if (!entity.IsDeleted) return Task.FromResult((OperationResult)Result.ValidationFault);
            entity.IsDeleted = false;
            return UpdateAsync(entity);
        }
        public virtual OperationResult Undelete<TModel, TKey>(TKey id)
            where TModel : ModelBase<TKey>, ISoftDeletable, new()
            where TKey : struct, IComparable<TKey>
        {
            return TaskExtensions.Yield(UndeleteAsync<TModel,TKey>(id));
        }

        #endregion
        public bool ChangesPending()
        {
            return Context.ChangeTracker.Entries().Any(p => p.State != EntityState.Unchanged);
        }
        public bool ChangesPending<TModel>(TModel entity) where TModel : class, new()
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
        private DbSet<TModel> Set<TModel>() where TModel : class, new()
        {
            return Context.Set<TModel>() ?? throw new InvalidOperationException(St.Service_doesnt_handle_model);
        }

        public abstract IQueryable<TModel> All<TModel>() where TModel : class, new();
        public abstract IQueryable All(Type model);

        public Task<List<TModel>> AllAsync<TModel>() where TModel : class, new()
        {
            return All<TModel>().ToListAsync();
        }
        public async Task<IList> AllAsync(Type model)
        {
            var l = new List<object>();

            void LoadToList()
            {
                foreach (var j in All(model)) l.Add(j);
            }

            await Task.Run(LoadToList);
            return l;
        }

        public abstract IQueryable<TModel> AllBase<TModel>() where TModel : class;

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
                    msj = string.Format(St.Service_Result_Unk, Ee.NameOf(result));
                    break;
            }
            return new OperationResult(result,msj);
        }

        public static implicit operator Service.Result(OperationResult result) => result.Result;

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
    }
}