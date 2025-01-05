using TheXDS.Triton.Middleware;

namespace TheXDS.Triton.Services;

/// <summary>
/// Defines a set of members that must be implemented by a type that allows
/// configuring a collection of middlewares.
/// </summary>
public interface IMiddlewareConfigurator
{
    /// <summary>
    /// Adds an action to execute during the epilogue of a Crud operation.
    /// </summary>
    /// <param name="func">
    /// Function to add to the epilogue.
    /// </param>
    /// <returns>
    /// The same instance of <see cref="IMiddlewareConfigurator"/>
    /// in order to use Fluent syntax.
    /// </returns>
    IMiddlewareConfigurator AddEpilogue(MiddlewareAction func);

    /// <summary>
    /// Adds an action to execute at the beginning of the epilogue of a Crud
    /// operation.
    /// </summary>
    /// <param name="func">
    /// Function to add to the epilogue.
    /// </param>
    /// <returns>
    /// The same instance of <see cref="IMiddlewareConfigurator"/>
    /// in order to use Fluent syntax.
    /// </returns>
    IMiddlewareConfigurator AddEarlyEpilogue(MiddlewareAction func);

    /// <summary>
    /// Adds an action to execute at the beginning of the prologue of a Crud
    /// operation.
    /// </summary>
    /// <param name="func">
    /// Function to add to the prologue.
    /// </param>
    /// <returns>
    /// The same instance of <see cref="IMiddlewareConfigurator"/>
    /// in order to use Fluent syntax.
    /// </returns>
    IMiddlewareConfigurator AddEarlyPrologue(MiddlewareAction func);

    /// <summary>
    /// Adds an action to execute at the end of the epilogue of a Crud
    /// operation.
    /// </summary>
    /// <param name="func">
    /// Function to add to the epilogue.
    /// </param>
    /// <returns>
    /// The same instance of <see cref="IMiddlewareConfigurator"/>
    /// in order to use Fluent syntax.
    /// </returns>
    IMiddlewareConfigurator AddLateEpilogue(MiddlewareAction func);

    /// <summary>
    /// Adds an action to execute at the end of the prologue of a Crud
    /// operation.
    /// </summary>
    /// <param name="func">
    /// Function to add to the prologue.
    /// </param>
    /// <returns>
    /// The same instance of <see cref="IMiddlewareConfigurator"/>
    /// in order to use Fluent syntax.
    /// </returns>
    IMiddlewareConfigurator AddLatePrologue(MiddlewareAction func);

    /// <summary>
    /// Adds an action to execute during the prologue of a Crud operation.
    /// </summary>
    /// <param name="func">
    /// Function to add to the prologue.
    /// </param>
    /// <returns>
    /// The same instance of <see cref="IMiddlewareConfigurator"/>
    /// in order to use Fluent syntax.
    /// </returns>
    IMiddlewareConfigurator AddPrologue(MiddlewareAction func);

    /// <summary>
    /// Adds the actions of a Middleware to execute during a Crud operation.
    /// </summary>
    /// <typeparam name="T">
    /// The type of Middleware to add.
    /// </typeparam>
    /// <returns>
    /// The same instance of <see cref="IMiddlewareConfigurator"/>
    /// in order to use Fluent syntax.
    /// </returns>
    IMiddlewareConfigurator Attach<T>() where T : ITransactionMiddleware, new()
    {
        return AttachAt<T>(ActionPosition.Default);
    }

    /// <summary>
    /// Adds the actions of a Middleware to execute during a Crud operation.
    /// </summary>
    /// <typeparam name="T">
    /// The type of Middleware to add.
    /// </typeparam>
    /// <param name="middleware">The added Middleware.</param>
    /// <returns>
    /// The same instance of <see cref="IMiddlewareConfigurator"/>
    /// in order to use Fluent syntax.
    /// </returns>
    IMiddlewareConfigurator Attach<T>(out T middleware) where T : ITransactionMiddleware, new()
    {
        return AttachAt(out middleware, ActionPosition.Default);
    }

    /// <summary>
    /// Adds the actions of a Middleware to execute during a Crud operation.
    /// </summary>
    /// <typeparam name="T">
    /// The type of Middleware to add.
    /// </typeparam>
    /// <param name="middleware">The Middleware that will be added.</param>
    /// <returns>
    /// The same instance of <see cref="IMiddlewareConfigurator"/>
    /// in order to use Fluent syntax.
    /// </returns>
    IMiddlewareConfigurator Attach<T>(T middleware) where T : ITransactionMiddleware
    {
        return AttachAt(middleware, ActionPosition.Default);
    }

    /// <summary>
    /// Adds with priority the actions of a Middleware to execute during a Crud
    /// operation.
    /// </summary>
    /// <typeparam name="T">
    /// The type of Middleware to add.
    /// </typeparam>
    /// <param name="prologPosition">
    /// The position at which to add the prologue of the
    /// <see cref="ITransactionMiddleware"/>.
    /// </param>
    /// <param name="epilogPosition">
    /// The position at which to add the epilogue of the
    /// <see cref="ITransactionMiddleware"/>.
    /// </param>
    /// <returns>
    /// The same instance of <see cref="IMiddlewareConfigurator"/>
    /// in order to use Fluent syntax.
    /// </returns>
    IMiddlewareConfigurator AttachAt<T>(in ActionPosition prologPosition,
        in ActionPosition epilogPosition) where T : ITransactionMiddleware, new()
    {
        return AttachAt(new T(), prologPosition, epilogPosition);
    }

    /// <summary>
    /// Adds with priority the actions of a Middleware to execute during a Crud
    /// operation.
    /// </summary>
    /// <typeparam name="T">
    /// The type of Middleware to add.
    /// </typeparam>
    /// <param name="position">
    /// The position at which to add both the prologue and epilogue of the
    /// <see cref="ITransactionMiddleware"/>.
    /// </param>
    /// <returns>
    /// The same instance of <see cref="IMiddlewareConfigurator"/>
    /// in order to use Fluent syntax.
    /// </returns>
    IMiddlewareConfigurator AttachAt<T>(in ActionPosition position) where T : ITransactionMiddleware, new()
    {
        return AttachAt<T>(position, position);
    }

    /// <summary>
    /// Adds with priority the actions of a Middleware to execute during a Crud
    /// operation.
    /// </summary>
    /// <typeparam name="T">
    /// The type of Middleware to add.
    /// </typeparam>
    /// <param name="middleware">The added Middleware.</param>
    /// <param name="prologPosition">
    /// The position at which to add the prologue of the
    /// <see cref="ITransactionMiddleware"/>.
    /// </param>
    /// <param name="epilogPosition">
    /// The position at which to add the epilogue of the
    /// <see cref="ITransactionMiddleware"/>.
    /// </param>
    /// <returns>
    /// The same instance of <see cref="IMiddlewareConfigurator"/>
    /// in order to use Fluent syntax.
    /// </returns>
    IMiddlewareConfigurator AttachAt<T>(out T middleware, in ActionPosition prologPosition,
        in ActionPosition epilogPosition) where T : ITransactionMiddleware, new()
    {
        middleware = new();
        return AttachAt(middleware, prologPosition, epilogPosition);
    }

    /// <summary>
    /// Adds with priority the actions of a Middleware to execute during a Crud
    /// operation.
    /// </summary>
    /// <typeparam name="T">
    /// The type of Middleware to add.
    /// </typeparam>
    /// <param name="middleware">The added Middleware.</param>
    /// <param name="position">
    /// The position at which to add both the prologue and epilogue of the
    /// <see cref="ITransactionMiddleware"/>.
    /// </param>
    /// <returns>
    /// The same instance of <see cref="IMiddlewareConfigurator"/>
    /// in order to use Fluent syntax.
    /// </returns>
    IMiddlewareConfigurator AttachAt<T>(out T middleware, in ActionPosition position)
        where T : ITransactionMiddleware, new()
    {
        return AttachAt(out middleware, position, position);
    }

    /// <summary>
    /// Adds the actions of a Middleware to execute during a Crud operation,
    /// specifying the position at which each action should be inserted.
    /// </summary>
    /// <typeparam name="T">
    /// The type of Middleware to add.
    /// </typeparam>
    /// <param name="middleware">The Middleware that will be added.</param>
    /// <param name="prologPosition">
    /// The position at which to add the prologue of the
    /// <see cref="ITransactionMiddleware"/>.
    /// </param>
    /// <param name="epilogPosition">
    /// The position at which to add the epilogue of the
    /// <see cref="ITransactionMiddleware"/>.
    /// </param>
    /// <returns>
    /// The same instance of <see cref="IMiddlewareConfigurator"/>
    /// in order to use Fluent syntax.
    /// </returns>
    IMiddlewareConfigurator AttachAt<T>(T middleware, in ActionPosition prologPosition, in ActionPosition epilogPosition) where T : ITransactionMiddleware;

    /// <summary>
    /// Adds the actions of a Middleware to execute during a Crud operation,
    /// specifying the position at which both the prologue and epilogue will be
    /// inserted.
    /// </summary>
    /// <typeparam name="T">
    /// The type of Middleware to add.
    /// </typeparam>
    /// <param name="middleware">The Middleware that will be added.</param>
    /// <param name="position">
    /// The position at which to insert both the prologue and epilogue of the
    /// <see cref="ITransactionMiddleware"/>.
    /// </param>
    /// <returns>
    /// The same instance of <see cref="IMiddlewareConfigurator"/>
    /// in order to use Fluent syntax.
    /// </returns>
    IMiddlewareConfigurator AttachAt<T>(T middleware, in ActionPosition position) where T : ITransactionMiddleware
    {
        return AttachAt(middleware, position, position);
    }

    /// <summary>
    /// Removes a previously registered <see cref="ITransactionMiddleware"/>.
    /// </summary>
    /// <param name="middleware">
    /// The <see cref="ITransactionMiddleware"/> to remove.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="ITransactionMiddleware"/> was
    /// removed successfully, <see langword="false"/> otherwise.
    /// </returns>
    bool Detach(ITransactionMiddleware middleware);

    /// <summary>
    /// Removes a previously registered <see cref="MiddlewareAction"/> that was
    /// previously added as a prologue to a transaction.
    /// </summary>
    /// <param name="action">
    /// The previously registered delegate to remove.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the delegate was successfully removed from
    /// the collection of prologues, <see langword="false"/> otherwise.
    /// </returns>
    bool DetachPrologue(MiddlewareAction action);

    /// <summary>
    /// Removes a previously registered <see cref="MiddlewareAction"/> that was
    /// previously added as an epilogue to a transaction.
    /// </summary>
    /// <param name="action">
    /// The previously registered delegate to remove.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the delegate was successfully removed from
    /// the collection of epilogues, <see langword="false"/> otherwise.
    /// </returns>
    bool DetachEpilogue(MiddlewareAction action);

    /// <summary>
    /// Gets a reference to an <see cref="IMiddlewareRunner"/> that will
    /// execute a collection of Middlewares.
    /// </summary>
    /// <returns>
    /// The <see cref="IMiddlewareRunner"/> used to execute the Middlewares.
    /// </returns>
    IMiddlewareRunner GetRunner();
}