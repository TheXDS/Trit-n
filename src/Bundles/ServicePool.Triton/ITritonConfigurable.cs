﻿using Microsoft.EntityFrameworkCore;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Services;

namespace TheXDS.ServicePool.Triton;

/// <summary>
/// Define una serie de miembros a implementar por un tipo que exponga
/// funciones de configuración para Tritón cuando se utiliza en conjunto
/// con un <see cref="ServicePool"/>.
/// </summary>
public interface ITritonConfigurable
{
    /// <summary>
    /// Obtiene una referencia al repositorio de servicios en el cual se ha
    /// registrado esta instancia.
    /// </summary>
    ServicePool Pool { get; }

    /// <summary>
    /// Descubre automáticamente todos los servicios y contextos de datos a
    /// exponer por medio de <see cref="ServicePool"/>.
    /// </summary>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    ITritonConfigurable DiscoverContexts();

    /// <summary>
    /// Agrega un <see cref="DbContext"/> a la colección de servicios
    /// hosteados dentro de un
    /// <see cref="ServicePool"/>, envolviendolo en un 
    /// <see cref="TritonService"/>.
    /// </summary>
    /// <typeparam name="T">Tipo de contexto a registrar.</typeparam>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    ITritonConfigurable UseContext<T>() where T : DbContext, new();

    /// <summary>
    /// Agrega un servicio a la colección de servicios hosteados dentro de
    /// un <see cref="ServicePool"/>.
    /// </summary>
    /// <typeparam name="T">Tipo de servicio a registrar.</typeparam>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    ITritonConfigurable UseService<T>() where T : TritonService;

    /// <summary>
    /// Agrega un Middleware a la configuración de transacciones a utilizar
    /// por los servicios de Tritón.
    /// </summary>
    /// <typeparam name="T">Tipo de Middleware a agregar.</typeparam>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    /// <seealso cref="ConfigureMiddlewares(Action{IMiddlewareConfigurator})"/>.
    ITritonConfigurable UseMiddleware<T>() where T : ITransactionMiddleware, new() => UseMiddleware<T>(out _);
    
    /// <summary>
    /// Agrega un Middleware a la configuración de transacciones a utilizar
    /// por los servicios de Tritón.
    /// </summary>
    /// <param name="newMiddleware">
    /// Parámetro de salida. Middleware que ha sido creado y registrado.
    /// </param>
    /// <typeparam name="T">Tipo de Middleware a agregar.</typeparam>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    /// <seealso cref="ConfigureMiddlewares(Action{IMiddlewareConfigurator})"/>.
    ITritonConfigurable UseMiddleware<T>(out T newMiddleware) where T : ITransactionMiddleware, new();

    /// <summary>
    /// Ejecuta un método de configuración de middlewares para la instancia
    /// configurada.
    /// </summary>
    /// <param name="configuratorCallback">
    /// Método a utilizar para configurar los Middlewares a utilizar en las
    /// transacciones de la instancia de Tritón configurada.
    /// </param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    /// <remarks>
    /// Para objetos que implementan <see cref="ITransactionMiddleware"/>,
    /// puede utilizar el método <see cref="UseMiddleware{T}(out T)"/> o
    /// <see cref="UseMiddleware{T}()"/> en su lugar.
    /// </remarks>
    /// <seealso cref="UseMiddleware{T}(out T)"/>.
    /// <seealso cref="UseMiddleware{T}()"/>.
    ITritonConfigurable ConfigureMiddlewares(Action<IMiddlewareConfigurator> configuratorCallback);

    /// <summary>
    /// Agrega un <see cref="DbContext"/> a la colección de servicios
    /// hosteados dentro de un
    /// <see cref="ServicePool"/>, envolviendolo en un 
    /// <see cref="TritonService"/>.
    /// </summary>
    /// <param name="context">
    /// Tipo de contexto a registrar. Debe implementar
    /// <see cref="DbContext"/>.
    /// </param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    ITritonConfigurable UseContext(Type context);

    /// <summary>
    /// Agrega una colección de acciones de Middleware de prólogo a la
    /// configuración de transacciones registrada.
    /// </summary>
    /// <param name="actions">Acciones de prólogo a agregar.</param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    ITritonConfigurable UseTransactionPrologs(params MiddlewareAction[] actions);

    /// <summary>
    /// Agrega una colección de acciones de Middleware de epílogo a la
    /// configuración de transacciones registrada.
    /// </summary>
    /// <param name="actions">Acciones de epílogo a agregar.</param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    ITritonConfigurable UseTransactionEpilogs(params MiddlewareAction[] actions);
}