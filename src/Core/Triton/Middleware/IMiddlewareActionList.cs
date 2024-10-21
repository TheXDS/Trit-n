namespace TheXDS.Triton.Middleware;

internal interface IMiddlewareActionList
{
    void Add(MiddlewareAction item);

    void AddFirst(MiddlewareAction item);

    void AddLast(MiddlewareAction item);
}