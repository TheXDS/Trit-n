using System;
using System.Collections.Generic;
using System.Linq;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Services
{
    public class TransactionConfiguration
    {
        private readonly List<Func<CrudAction, Model?, ServiceResult?>> _prologs = new List<Func<CrudAction, Model?, ServiceResult?>>();
        private readonly List<Func<CrudAction, Model?, ServiceResult?>> _epiloges = new List<Func<CrudAction, Model?, ServiceResult?>>();

        /// <summary>
        ///     Realiza comprobaciones adicionales antes de ejecutar una acción
        ///     de crud, devolviendo <see langword="null"/> si la operación 
        ///     puede continuar.
        /// </summary>
        /// <param name="action">
        ///     Acción Crud a intentar realizar.
        /// </param>
        /// <param name="entity">
        ///     Entidad sobre la cual se ejecutará la acción.
        /// </param>
        /// <returns>
        ///     Un <see cref="ServiceResult"/> con el resultado del preámbulo,
        ///     o <see langword="null"/> si la operación puede continuar.
        /// </returns>
        public ServiceResult? Prolog(CrudAction action, Model? entity) => Run(_prologs, action, entity);

        /// <summary>
        ///     Realiza comprobaciones adicionales después de ejecutar una
        ///     acción de Crud, devolviendo <see langword="null"/> si la
        ///     operación puede continuar.
        /// </summary>
        /// <param name="action">
        ///     Acción Crud que se ha realizado.
        /// </param>
        /// <param name="entity">
        ///     Entidad sobre la cual se ha ejecutado la acción.
        /// </param>
        /// <returns>
        ///     Un <see cref="ServiceResult"/> con el resultado del epílogo,
        ///     o <see langword="null"/> si la operación puede continuar.
        /// </returns>
        public ServiceResult? Epilog(CrudAction action, Model? entity) => Run(Enumerable.Reverse(_epiloges), action, entity);

        public TransactionConfiguration AddProlog(Func<CrudAction, Model?, ServiceResult?> function)
        {
            return AddTo(_prologs,function);            
        }
        public TransactionConfiguration AddPrologs(IEnumerable<Func<CrudAction, Model?, ServiceResult?>> functions)
        {
            return AddTo(_prologs, functions);
        }
        public TransactionConfiguration AddPrologs(params Func<CrudAction, Model?, ServiceResult?>[] functions)
        {
            return AddTo(_prologs, functions);
        }
        public TransactionConfiguration RemoveProlog(Func<CrudAction, Model?, ServiceResult?> function)
        {
            return RemoveFrom(_prologs, function);
        }
        public TransactionConfiguration RemovePrologs(IEnumerable<Func<CrudAction, Model?, ServiceResult?>> functions)
        {
            return RemoveFrom(_prologs, functions);
        }
        public TransactionConfiguration RemovePrologs(params Func<CrudAction, Model?, ServiceResult?>[] functions)
        {
            return RemoveFrom(_prologs, functions);
        }
        public TransactionConfiguration InsertProlog(int index, Func<CrudAction, Model?, ServiceResult?> function)
        {
            return InsertInto(_prologs, index, function);
        }
        public TransactionConfiguration InsertPrologs(int index, IEnumerable<Func<CrudAction, Model?, ServiceResult?>> functions)
        {
            return InsertInto(_prologs, index, functions);
        }
        public TransactionConfiguration InsertPrologs(int index, params Func<CrudAction, Model?, ServiceResult?>[] functions)
        {
            return InsertInto(_prologs, index, functions);
        }
        public TransactionConfiguration AddEpilog(Func<CrudAction, Model?, ServiceResult?> function)
        {
            return AddTo(_epiloges, function);
        }
        public TransactionConfiguration AddEpiloges(IEnumerable<Func<CrudAction, Model?, ServiceResult?>> functions)
        {
            return AddTo(_epiloges, functions);
        }
        public TransactionConfiguration AddEpiloges(params Func<CrudAction, Model?, ServiceResult?>[] functions)
        {
            return AddTo(_epiloges, functions);
        }
        public TransactionConfiguration RemoveEpilog(Func<CrudAction, Model?, ServiceResult?> function)
        {
            return RemoveFrom(_epiloges, function);
        }
        public TransactionConfiguration RemoveEpiloges(IEnumerable<Func<CrudAction, Model?, ServiceResult?>> functions)
        {
            return RemoveFrom(_epiloges, functions);
        }
        public TransactionConfiguration RemoveEpiloges(params Func<CrudAction, Model?, ServiceResult?>[] functions)
        {
            return RemoveFrom(_epiloges, functions);
        }
        public TransactionConfiguration InsertEpilog(int index, Func<CrudAction, Model?, ServiceResult?> function)
        {
            return InsertInto(_epiloges, index, function);
        }
        public TransactionConfiguration InsertEpiloges(int index, IEnumerable<Func<CrudAction, Model?, ServiceResult?>> functions)
        {
            return InsertInto(_epiloges, index, functions);
        }
        public TransactionConfiguration InsertEpiloges(int index, params Func<CrudAction, Model?, ServiceResult?>[] functions)
        {
            return InsertInto(_epiloges, index, functions);
        }


        public TransactionConfiguration Attach<T>(out T middleware) where T : ITransactionMiddleware, new()
        {
            middleware = new T();
            AddProlog(middleware.BeforeAction);
            AddEpilog(middleware.AfterAction);
            return this;
        }
        public TransactionConfiguration PriorityAttach<T>(out T middleware) where T : ITransactionMiddleware, new()
        {
            middleware = new T();
            InsertProlog(1, middleware.BeforeAction);
            InsertEpilog(1, middleware.AfterAction);
            return this;
        }
        public TransactionConfiguration Attach<T>() where T : ITransactionMiddleware, new()
        {            
            return Attach<T>(out _);
        }
        public TransactionConfiguration PriorityAttach<T>() where T : ITransactionMiddleware, new()
        {
            return PriorityAttach<T>(out _);
        }


        private static ServiceResult? Run(IEnumerable<Func<CrudAction,Model?,ServiceResult?>> collection, CrudAction action, Model? entity)
        {
            foreach (var j in collection)
            {
                if (j.Invoke(action, entity) is { } r) return r;
            }
            return null;
        }
        private TransactionConfiguration AddTo(List<Func<CrudAction, Model?, ServiceResult?>> collection, Func<CrudAction, Model?, ServiceResult?> function)
        {
            collection.Add(function ?? throw new ArgumentNullException(nameof(function)));
            return this;
        }
        private TransactionConfiguration AddTo(List<Func<CrudAction, Model?, ServiceResult?>> collection, IEnumerable<Func<CrudAction, Model?, ServiceResult?>> functions)
        {
            collection.AddRange(functions.NotNull() ?? throw new ArgumentNullException(nameof(functions)));
            return this;
        }
        private TransactionConfiguration RemoveFrom(List<Func<CrudAction, Model?, ServiceResult?>> collection, Func<CrudAction, Model?, ServiceResult?> function)
        {
            collection.Remove(function ?? throw new ArgumentNullException(nameof(function)));
            return this;
        }
        private TransactionConfiguration RemoveFrom(List<Func<CrudAction, Model?, ServiceResult?>> collection, IEnumerable<Func<CrudAction, Model?, ServiceResult?>> functions)
        {
            foreach (var j in functions ?? throw new ArgumentNullException(nameof(functions)))
            {
                collection.Remove(j ?? throw new NullReferenceException());
            }
            return this;
        }
        private TransactionConfiguration InsertInto(List<Func<CrudAction, Model?, ServiceResult?>> collection, int index, Func<CrudAction, Model?, ServiceResult?> function)
        {
            collection.Insert(index, function ?? throw new ArgumentNullException(nameof(function)));
            return this;
        }
        private TransactionConfiguration InsertInto(List<Func<CrudAction, Model?, ServiceResult?>> collection, int index, IEnumerable<Func<CrudAction, Model?, ServiceResult?>> functions)
        {
            collection.InsertRange(index, functions.NotNull() ?? throw new ArgumentNullException(nameof(functions)));
            return this;
        }
    }
}