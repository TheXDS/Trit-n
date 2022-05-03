using TheXDS.Triton.Middleware;

namespace TheXDS.Triton.Services
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que permita
    /// configurar una colección de Middlewares.
    /// </summary>
    public interface IMiddlewareConfigurator
    {
        /// <summary>
        /// Agrega una acción a ejecutar durante el epílogo de una
        /// operación Crud.
        /// </summary>
        /// <param name="func">
        /// Función a agregar al epílogo.
        /// </param>
        /// <returns>
        /// La misma instancia de <see cref="TransactionConfiguration"/>
        /// para poder utilizar sintaxis Fluent.
        /// </returns>
        IMiddlewareConfigurator AddEpilog(MiddlewareAction func);

        /// <summary>
        /// Agrega una acción a ejecutar al inicio del epílogo de una
        /// operación Crud.
        /// </summary>
        /// <param name="func">
        /// Función a agregar al epílogo.
        /// </param>
        /// <returns>
        /// La misma instancia de <see cref="TransactionConfiguration"/>
        /// para poder utilizar sintaxis Fluent.
        /// </returns>
        IMiddlewareConfigurator AddFirstEpilog(MiddlewareAction func);

        /// <summary>
        /// Agrega una acción a ejecutar al inicio del prólogo de una
        /// operación Crud.
        /// </summary>
        /// <param name="func">
        /// Función a agregar al prólogo.
        /// </param>
        /// <returns>
        /// La misma instancia de <see cref="TransactionConfiguration"/>
        /// para poder utilizar sintaxis Fluent.
        /// </returns>
        IMiddlewareConfigurator AddFirstProlog(MiddlewareAction func);

        /// <summary>
        /// Agrega una acción a ejecutar al final del epílogo de una
        /// operación Crud.
        /// </summary>
        /// <param name="func">
        /// Función a agregar al epílogo.
        /// </param>
        /// <returns>
        /// La misma instancia de <see cref="TransactionConfiguration"/>
        /// para poder utilizar sintaxis Fluent.
        /// </returns>
        IMiddlewareConfigurator AddLastEpilog(MiddlewareAction func);

        /// <summary>
        /// Agrega una acción a ejecutar al final del prólogo de una
        /// operación Crud.
        /// </summary>
        /// <param name="func">
        /// Función a agregar al prólogo.
        /// </param>
        /// <returns>
        /// La misma instancia de <see cref="TransactionConfiguration"/>
        /// para poder utilizar sintaxis Fluent.
        /// </returns>
        IMiddlewareConfigurator AddLastProlog(MiddlewareAction func);

        /// <summary>
        /// Agrega una acción a ejecutar durante el prólogo de una
        /// operación Crud.
        /// </summary>
        /// <param name="func">
        /// Función a agregar al prólogo.
        /// </param>
        /// <returns>
        /// La misma instancia de <see cref="TransactionConfiguration"/>
        /// para poder utilizar sintaxis Fluent.
        /// </returns>
        IMiddlewareConfigurator AddProlog(MiddlewareAction func);

        /// <summary>
        /// Agrega las acciones de un Middleware a ejecutar durante una
        /// operación Crud.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de Middleware a agregar.
        /// </typeparam>
        /// <returns>
        /// La misma instancia de <see cref="TransactionConfiguration"/>
        /// para poder utilizar sintaxis Fluent.
        /// </returns>
        IMiddlewareConfigurator Attach<T>() where T : ITransactionMiddleware, new()
        {
            return AttachAt<T>(TransactionConfiguration.ActionPosition.Default);
        }

        /// <summary>
        /// Agrega las acciones de un Middleware a ejecutar durante una
        /// operación Crud.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de Middleware a agregar.
        /// </typeparam>
        /// <param name="middleware">Middleware que ha sido agregado.</param>
        /// <returns>
        /// La misma instancia de <see cref="TransactionConfiguration"/>
        /// para poder utilizar sintaxis Fluent.
        /// </returns>
        IMiddlewareConfigurator Attach<T>(out T middleware) where T : ITransactionMiddleware, new()
        {
            return AttachAt(out middleware, TransactionConfiguration.ActionPosition.Default);
        }

        /// <summary>
        /// Agrega las acciones de un Middleware a ejecutar durante una
        /// operación Crud.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de Middleware a agregar.
        /// </typeparam>
        /// <param name="middleware">Middleware que será agregado.</param>
        /// <returns>
        /// La misma instancia de <see cref="TransactionConfiguration"/>
        /// para poder utilizar sintaxis Fluent.
        /// </returns>
        IMiddlewareConfigurator Attach<T>(T middleware) where T : ITransactionMiddleware
        {
            return AttachAt(middleware, TransactionConfiguration.ActionPosition.Default);
        }

        /// <summary>
        /// Agrega con prioridad las acciones de un Middleware a ejecutar
        /// durante una operación Crud.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de Middleware a agregar.
        /// </typeparam>
        /// <param name="prologPosition">
        /// Posición en la cual agregar el prólogo del 
        /// <see cref="ITransactionMiddleware"/>.
        /// </param>
        /// <param name="epilogPosition">
        /// Posición en la cual agregar el epílogo del 
        /// <see cref="ITransactionMiddleware"/>.
        /// </param>
        /// <returns>
        /// La misma instancia de <see cref="TransactionConfiguration"/>
        /// para poder utilizar sintaxis Fluent.
        /// </returns>
        IMiddlewareConfigurator AttachAt<T>(in TransactionConfiguration.ActionPosition prologPosition,
            in TransactionConfiguration.ActionPosition epilogPosition) where T : ITransactionMiddleware, new()
        {
            return AttachAt(new T(), prologPosition, epilogPosition);
        }
        
        /// <summary>
        /// Agrega con prioridad las acciones de un Middleware a ejecutar
        /// durante una operación Crud.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de Middleware a agregar.
        /// </typeparam>
        /// <param name="position">
        /// Posición en la cual agregar el prólogo y epílogo del 
        /// <see cref="ITransactionMiddleware"/>.
        /// </param>
        /// <returns>
        /// La misma instancia de <see cref="TransactionConfiguration"/>
        /// para poder utilizar sintaxis Fluent.
        /// </returns>
        IMiddlewareConfigurator AttachAt<T>(in TransactionConfiguration.ActionPosition position) where T : ITransactionMiddleware, new()
        {
            return AttachAt<T>(position, position);
        }

        /// <summary>
        /// Agrega con prioridad las acciones de un Middleware a ejecutar
        /// durante una operación Crud.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de Middleware a agregar.
        /// </typeparam>
        /// <param name="middleware">Middleware que ha sido agregado.</param>
        /// <param name="prologPosition">
        /// Posición en la cual agregar el prólogo del 
        /// <see cref="ITransactionMiddleware"/>.
        /// </param>
        /// <param name="epilogPosition">
        /// Posición en la cual agregar el epílogo del 
        /// <see cref="ITransactionMiddleware"/>.
        /// </param>
        /// <returns>
        /// La misma instancia de <see cref="TransactionConfiguration"/>
        /// para poder utilizar sintaxis Fluent.
        /// </returns>
        IMiddlewareConfigurator AttachAt<T>(out T middleware, in TransactionConfiguration.ActionPosition prologPosition,
            in TransactionConfiguration.ActionPosition epilogPosition) where T : ITransactionMiddleware, new()
        {
            middleware = new();
            return AttachAt(middleware, prologPosition, epilogPosition);
        }

        /// <summary>
        /// Agrega con prioridad las acciones de un Middleware a ejecutar
        /// durante una operación Crud.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de Middleware a agregar.
        /// </typeparam>
        /// <param name="middleware">Middleware que ha sido agregado.</param>
        /// <param name="position">
        /// Posición en la cual agregar el prólogo y epílogo del 
        /// <see cref="ITransactionMiddleware"/>.
        /// </param>
        /// <returns>
        /// La misma instancia de <see cref="TransactionConfiguration"/>
        /// para poder utilizar sintaxis Fluent.
        /// </returns>
        IMiddlewareConfigurator AttachAt<T>(out T middleware, in TransactionConfiguration.ActionPosition position)
            where T : ITransactionMiddleware, new()
        {
            return AttachAt(out middleware, position, position);
        }
        
        /// <summary>
        /// Agrega las acciones de un Middleware a ejecutar durante una
        /// operación Crud, especificando la posición en la cual cada acción
        /// deberá insertarse.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de Middleware a agregar.
        /// </typeparam>
        /// <param name="middleware">Middleware que será agregado.</param>
        /// <param name="prologPosition">
        /// Posición en la cual agregar el prólogo del 
        /// <see cref="ITransactionMiddleware"/>.
        /// </param>
        /// <param name="epilogPosition">
        /// Posición en la cual agregar el epílogo del 
        /// <see cref="ITransactionMiddleware"/>.
        /// </param>
        /// <returns>
        /// La misma instancia de <see cref="TransactionConfiguration"/>
        /// para poder utilizar sintaxis Fluent.
        /// </returns>
        IMiddlewareConfigurator AttachAt<T>(T middleware, in TransactionConfiguration.ActionPosition prologPosition, in TransactionConfiguration.ActionPosition epilogPosition) where T : ITransactionMiddleware;

        /// <summary>
        /// Agrega las acciones de un Middleware a ejecutar durante una
        /// operación Crud, especificando la posición en la cual cada acción
        /// deberá insertarse.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de Middleware a agregar.
        /// </typeparam>
        /// <param name="middleware">Middleware que será agregado.</param>
        /// <param name="position">
        /// Posición en la cual agregar el prólogo y epílogo del 
        /// <see cref="ITransactionMiddleware"/>.
        /// </param>
        /// <returns>
        /// La misma instancia de <see cref="TransactionConfiguration"/>
        /// para poder utilizar sintaxis Fluent.
        /// </returns>
        IMiddlewareConfigurator AttachAt<T>(T middleware, in TransactionConfiguration.ActionPosition position)
            where T : ITransactionMiddleware
        {
            return AttachAt(middleware, position, position);
        }
        
        /// <summary>
        /// Obtiene una referencia a un <see cref="IMiddlewareRunner"/> que
        /// ejecutará una colección de Middlewares.
        /// </summary>
        /// <returns>
        /// El <see cref="IMiddlewareRunner"/> utilizado para ejecutar
        /// Middlewares.
        /// </returns>
        IMiddlewareRunner GetRunner();
    }
}