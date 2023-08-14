using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Middleware;

/// <summary>
/// Define una serie de miembros a implementar por un tipo que funcione
/// como el Middleware de una operación Crud durante una transacción.
/// </summary>
public interface ITransactionMiddleware
{
    /// <summary>
    /// Define una serie de acciones a realizar antes de la operación
    /// Crud.
    /// </summary>
    /// <param name="action">
    /// Acción Crud que se realizará.
    /// </param>
    /// <param name="entities">
    /// Entidades sobre las cual se ejecutará una operación Crud. Para
    /// operaciones de lectura o de Query, este parámetro puede ser
    /// <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="null"/> si el Middleware se ha ejecutado
    /// correctamente, o un <see cref="ServiceResult"/> que describa
    /// una falla en caso que esta ocurra.
    /// </returns>
    ServiceResult? PrologAction(CrudAction action, IEnumerable<Model>? entities) => null;

    /// <summary>
    /// Define una serie de acciones a realizar después de la operación
    /// Crud.
    /// </summary>
    /// <param name="action">
    /// Acción Crud que se realizará.
    /// </param>
    /// <param name="entities">
    /// Entidades sobre al cual se ha ejecutado una operación Crud. Para
    /// operaciones de lectura o de Query, este parámetro puede ser
    /// <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="null"/> si el Middleware se ha ejecutado
    /// correctamente, o un <see cref="ServiceResult"/> que describa
    /// una falla en caso que esta ocurra.
    /// </returns>
    ServiceResult? EpilogAction(CrudAction action, IEnumerable<Model>? entities) => null;
}
