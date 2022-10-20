namespace TheXDS.Triton.Middleware;

/// <summary>
/// Define una serie de miembros a implementar por un tipo que exponga
/// métodos de control para las listas de acciones de Middleware.
/// </summary>
public interface IMiddlewareActionList
{
    /// <summary>
    /// Agrega una acción de Middleware en la posición predeterminada de la lista.
    /// </summary>
    /// <param name="item">Acción a agregar.</param>
    void Add(MiddlewareAction item);

    /// <summary>
    /// Agrega una acción de Middleware al principio de la lista.
    /// </summary>
    /// <param name="item">Acción a agregar.</param>
    void AddFirst(MiddlewareAction item);

    /// <summary>
    /// Agrega una acción de Middleware al final de la lista.
    /// </summary>
    /// <param name="item">Acción a agregar.</param>
    void AddLast(MiddlewareAction item);
}