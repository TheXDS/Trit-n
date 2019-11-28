using System;
using System.Collections.Generic;
using TheXDS.MCART.Types;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Services
{
    /// <summary>
    ///     Objeto que provee de configuración y otros servicios a las
    ///     transacciones Crud.
    /// </summary>
    public class TransactionConfiguration
    {
        private readonly OpenList<Func<CrudAction, Model?, ServiceResult?>> _prologs = new OpenList<Func<CrudAction, Model?, ServiceResult?>>();
        private readonly OpenList<Func<CrudAction, Model?, ServiceResult?>> _epilogs = new OpenList<Func<CrudAction, Model?, ServiceResult?>>();

        /// <summary>
        ///     Agrega las acciones de un Middleware a ejecutar durante una
        ///     operación Crud.
        /// </summary>
        /// <typeparam name="T">
        ///     Tipo de Middleware a agregar.
        /// </typeparam>
        /// <param name="middleware">Middleware que ha sido agregado.</param>
        /// <returns>
        ///     La misma instancia de <see cref="TransactionConfiguration"/>
        ///     para poder utilizar sintaxis Fluent.
        /// </returns>
        public TransactionConfiguration Attach<T>(out T middleware) where T : ITransactionMiddleware, new()
        {
            middleware = new T();
            _prologs.Add(middleware.BeforeAction);
            _epilogs.Add(middleware.AfterAction);
            return this;
        }

        /// <summary>
        ///     Agrega con prioridad las acciones de un Middleware a ejecutar
        ///     durante una operación Crud.
        /// </summary>
        /// <typeparam name="T">
        ///     Tipo de Middleware a agregar.
        /// </typeparam>
        /// <param name="middleware">Middleware que ha sido agregado.</param>
        /// <returns>
        ///     La misma instancia de <see cref="TransactionConfiguration"/>
        ///     para poder utilizar sintaxis Fluent.
        /// </returns>
        public TransactionConfiguration PriorityAttach<T>(out T middleware) where T : ITransactionMiddleware, new()
        {
            middleware = new T();
            _prologs.Insert(1, middleware.BeforeAction);
            _epilogs.AddTail(middleware.AfterAction);
            return this;
        }

        /// <summary>
        ///     Agrega las acciones de un Middleware a ejecutar durante una
        ///     operación Crud.
        /// </summary>
        /// <typeparam name="T">
        ///     Tipo de Middleware a agregar.
        /// </typeparam>
        /// <returns>
        ///     La misma instancia de <see cref="TransactionConfiguration"/>
        ///     para poder utilizar sintaxis Fluent.
        /// </returns>
        public TransactionConfiguration Attach<T>() where T : ITransactionMiddleware, new()
        {            
            return Attach<T>(out _);
        }

        /// <summary>
        ///     Agrega con prioridad las acciones de un Middleware a ejecutar
        ///     durante una operación Crud.
        /// </summary>
        /// <typeparam name="T">
        ///     Tipo de Middleware a agregar.
        /// </typeparam>
        /// <returns>
        ///     La misma instancia de <see cref="TransactionConfiguration"/>
        ///     para poder utilizar sintaxis Fluent.
        /// </returns>
        public TransactionConfiguration PriorityAttach<T>() where T : ITransactionMiddleware, new()
        {
            return PriorityAttach<T>(out _);
        }

        /// <summary>
        ///     Agrega una acción a ejecutar durante el prólogo de una
        ///     operación Crud.
        /// </summary>
        /// <param name="func">
        ///     Función a agregar al prólogo.
        /// </param>
        /// <returns>
        ///     La misma instancia de <see cref="TransactionConfiguration"/>
        ///     para poder utilizar sintaxis Fluent.
        /// </returns>
        public TransactionConfiguration AddProlog(Func<CrudAction, Model?, ServiceResult?> func)
        {
            _prologs.Add(func);
            return this;
        }

        /// <summary>
        ///     Agrega una acción a ejecutar al inicio del prólogo de una
        ///     operación Crud.
        /// </summary>
        /// <param name="func">
        ///     Función a agregar al prólogo.
        /// </param>
        /// <returns>
        ///     La misma instancia de <see cref="TransactionConfiguration"/>
        ///     para poder utilizar sintaxis Fluent.
        /// </returns>
        public TransactionConfiguration AddFirstProlog(Func<CrudAction, Model?, ServiceResult?> func)
        {
            _prologs.Insert(1, func);
            return this;
        }

        /// <summary>
        ///     Agrega una acción a ejecutar al final del prólogo de una
        ///     operación Crud.
        /// </summary>
        /// <param name="func">
        ///     Función a agregar al prólogo.
        /// </param>
        /// <returns>
        ///     La misma instancia de <see cref="TransactionConfiguration"/>
        ///     para poder utilizar sintaxis Fluent.
        /// </returns>
        public TransactionConfiguration AddLastProlog(Func<CrudAction, Model?, ServiceResult?> func)
        {
            _prologs.AddTail(func);
            return this;
        }

        /// <summary>
        ///     Agrega una acción a ejecutar durante el epílogo de una
        ///     operación Crud.
        /// </summary>
        /// <param name="func">
        ///     Función a agregar al epílogo.
        /// </param>
        /// <returns>
        ///     La misma instancia de <see cref="TransactionConfiguration"/>
        ///     para poder utilizar sintaxis Fluent.
        /// </returns>
        public TransactionConfiguration AddEpilog(Func<CrudAction, Model?, ServiceResult?> func)
        {
            _epilogs.Add(func);
            return this;
        }

        /// <summary>
        ///     Agrega una acción a ejecutar al inicio del epílogo de una
        ///     operación Crud.
        /// </summary>
        /// <param name="func">
        ///     Función a agregar al epílogo.
        /// </param>
        /// <returns>
        ///     La misma instancia de <see cref="TransactionConfiguration"/>
        ///     para poder utilizar sintaxis Fluent.
        /// </returns>
        public TransactionConfiguration AddFirstEpilog(Func<CrudAction, Model?, ServiceResult?> func)
        {
            _epilogs.Insert(1, func);
            return this;
        }

        /// <summary>
        ///     Agrega una acción a ejecutar al final del epílogo de una
        ///     operación Crud.
        /// </summary>
        /// <param name="func">
        ///     Función a agregar al epílogo.
        /// </param>
        /// <returns>
        ///     La misma instancia de <see cref="TransactionConfiguration"/>
        ///     para poder utilizar sintaxis Fluent.
        /// </returns>
        public TransactionConfiguration AddLastEpilog(Func<CrudAction, Model?, ServiceResult?> func)
        {
            _epilogs.AddTail(func);
            return this;
        }

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
        public ServiceResult? Epilog(CrudAction action, Model? entity) => Run(_epilogs, action, entity);

        private static ServiceResult? Run(IEnumerable<Func<CrudAction,Model?,ServiceResult?>> collection, CrudAction action, Model? entity)
        {
            foreach (var j in collection)
            {
                if (j.Invoke(action, entity) is { } r) return r;
            }
            return null;
        }
    }
}