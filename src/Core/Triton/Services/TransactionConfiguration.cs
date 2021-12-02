﻿using System.Collections;
using System.Collections.Generic;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Services
{
    /// <summary>
    /// Objeto que provee de configuración y otros servicios a las
    /// transacciones Crud.
    /// </summary>
    public class TransactionConfiguration : IMiddlewareConfigurator
    {
        private class MiddlewareActionList : IMiddlewareActionList, IEnumerable<MiddlewareAction>
        {
            private readonly List<MiddlewareAction> _list = new();
            private int _tail = 0;

            public void AddFirst(MiddlewareAction item)
            {
                _list.Insert(1, item);
                _tail++;
            }

            public void Add(MiddlewareAction item)
            {
                _list.Insert(_tail++, item);
            }

            public void AddLast(MiddlewareAction item)
            {
                _list.Add(item);
            }

            public IEnumerator<MiddlewareAction> GetEnumerator() => _list.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_list).GetEnumerator();
        }

        /// <summary>
        /// Enumeración que describe la posición en la cual se agregará una
        /// acción de Middleware de Crud.
        /// </summary>
        public enum ActionPosition : byte
        {
            /// <summary>
            /// Posición predeterminada.
            /// </summary>
            Default,
            /// <summary>
            /// Asegurar primera acción.
            /// </summary>
            Early,
            /// <summary>
            /// Asegurar última acción.
            /// </summary>
            Late
        }

        private readonly MiddlewareActionList _prologs = new();
        private readonly MiddlewareActionList _epilogs = new();

        /// <inheritdoc/>
        public IMiddlewareConfigurator Attach<T>(T middleware) where T : ITransactionMiddleware
        {
            _prologs.Add(middleware.PrologAction);
            _epilogs.Add(middleware.EpilogAction);
            return this;
        }

        /// <inheritdoc/>
        public IMiddlewareConfigurator Attach<T>(out T middleware) where T : ITransactionMiddleware, new()
        {
            return Attach(middleware = new T());
        }

        /// <inheritdoc/>
        public IMiddlewareConfigurator Attach<T>() where T : ITransactionMiddleware, new()
        {
            return Attach<T>(out _);
        }

        /// <inheritdoc/>
        public IMiddlewareConfigurator AttachAt<T>(T middleware, in ActionPosition prologPosition = ActionPosition.Default, in ActionPosition epilogPosition = ActionPosition.Default) where T : ITransactionMiddleware
        {
            AttachAt(_prologs, middleware.PrologAction, prologPosition);
            AttachAt(_epilogs, middleware.EpilogAction, epilogPosition);
            return this;
        }

        /// <inheritdoc/>
        public IMiddlewareConfigurator AttachAt<T>(out T middleware, in ActionPosition prologPosition = ActionPosition.Default, in ActionPosition epilogPosition = ActionPosition.Default) where T : ITransactionMiddleware, new()
        {
            return AttachAt(middleware = new T(), prologPosition, epilogPosition);
        }

        /// <inheritdoc/>
        public IMiddlewareConfigurator AttachAt<T>(in ActionPosition prologPosition = ActionPosition.Default, in ActionPosition epilogPosition = ActionPosition.Default) where T : ITransactionMiddleware, new()
        {
            return AttachAt<T>(out _, prologPosition, epilogPosition);
        }

        /// <inheritdoc/>
        public IMiddlewareConfigurator AddProlog(MiddlewareAction func)
        {
            _prologs.Add(func);
            return this;
        }

        /// <inheritdoc/>
        public IMiddlewareConfigurator AddFirstProlog(MiddlewareAction func)
        {
            _prologs.AddFirst(func);
            return this;
        }

        /// <inheritdoc/>
        public IMiddlewareConfigurator AddLastProlog(MiddlewareAction func)
        {
            _prologs.AddLast(func);
            return this;
        }

        /// <inheritdoc/>
        public IMiddlewareConfigurator AddEpilog(MiddlewareAction func)
        {
            _epilogs.Add(func);
            return this;
        }

        /// <inheritdoc/>
        public IMiddlewareConfigurator AddFirstEpilog(MiddlewareAction func)
        {
            _epilogs.AddFirst(func);
            return this;
        }

        /// <inheritdoc/>
        public IMiddlewareConfigurator AddLastEpilog(MiddlewareAction func)
        {
            _epilogs.AddLast(func);
            return this;
        }

        /// <summary>
        /// Realiza comprobaciones adicionales antes de ejecutar una acción
        /// de crud, devolviendo <see langword="null"/> si la operación 
        /// puede continuar.
        /// </summary>
        /// <param name="action">
        /// Acción Crud a intentar realizar.
        /// </param>
        /// <param name="entity">
        /// Entidad sobre la cual se ejecutará la acción.
        /// </param>
        /// <returns>
        /// Un <see cref="ServiceResult"/> con el resultado del prólogo que ha,
        /// fallado o <see langword="null"/> si la operación puede continuar.
        /// </returns>
        public ServiceResult? RunProlog(in CrudAction action, Model? entity) => Run(_prologs, action, entity);

        /// <summary>
        /// Realiza comprobaciones adicionales después de ejecutar una
        /// acción de Crud, devolviendo <see langword="null"/> si la
        /// operación puede continuar.
        /// </summary>
        /// <param name="action">
        /// Acción Crud que se ha realizado.
        /// </param>
        /// <param name="entity">
        /// Entidad sobre la cual se ha ejecutado la acción.
        /// </param>
        /// <returns>
        /// Un <see cref="ServiceResult"/> con el resultado del epílogo que ha,
        /// fallado o <see langword="null"/> si la operación puede continuar.
        /// </returns>
        public ServiceResult? RunEpilog(in CrudAction action, Model? entity) => Run(_epilogs, action, entity);

        private static void AttachAt(IMiddlewareActionList list, MiddlewareAction action, in ActionPosition position)
        {
            switch (position)
            {
                case ActionPosition.Default:
                    list.Add(action);
                    break;
                case ActionPosition.Early:
                    list.AddFirst(action);
                    break;
                case ActionPosition.Late:
                    list.AddLast(action);
                    break;
            }
        }
        private static ServiceResult? Run(IEnumerable<MiddlewareAction> collection, in CrudAction action, Model? entity)
        {
            foreach (var j in collection)
            {
                if (j.Invoke(action, entity) is { } r) return r;
            }
            return null;
        }
    }
}