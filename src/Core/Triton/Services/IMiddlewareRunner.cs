using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Services;

/// <summary>
/// Define una serie de miembros a implementar por un tipo que tenga la 
/// capacidad de ejecutar Middlewares antes y despúes de iniciar acciones
/// CRUD.
/// </summary>
public interface IMiddlewareRunner
{
    /// <summary>
    /// Ejecuta los Middlewares asociados al epílogo de una operación CRUD.
    /// </summary>
    /// <param name="action">Acción CRUD ejecutada.</param>
    /// <param name="entity">
    /// Entidad sobre la cual se ha ejecutado la operación.
    /// </param>
    /// <returns>
    /// Un <see cref="ServiceResult"/> si un middleware indica que la
    /// operación debe detenerse, o <see langword="null"/> para indicar que
    /// la ejecución de la cadena de Middlewares se completó
    /// satisfactoriamente.
    /// </returns>
    ServiceResult? RunEpilog(in CrudAction action, Model? entity);

    /// <summary>
    /// Ejecuta los Middlewares asociados al prólogo de una operación CRUD.
    /// </summary>
    /// <param name="action">Acción CRUD ejecutada.</param>
    /// <param name="entity">
    /// Entidad sobre la cual se ejecutará la operación.
    /// </param>
    /// <returns>
    /// Un <see cref="ServiceResult"/> si un middleware indica que la
    /// operación debe detenerse, o <see langword="null"/> para indicar que
    /// la ejecución de la cadena de Middlewares se completó
    /// satisfactoriamente.
    /// </returns>
    ServiceResult? RunProlog(in CrudAction action, Model? entity);
}