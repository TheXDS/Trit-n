using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using TheXDS.MCART.Exceptions;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Dynamic.Types;

/// <summary>
/// Interface that implements dynamic methods for reading data through reflection.
/// </summary>
public interface IDynamicCrudReadTransaction : ICrudReadTransaction
{
    ServiceResult<Model?> ICrudReadTransaction.Read(Type model, object key)
    {
        return DynamicRead(model, key, p => p);
    }

    ServiceResult<TModel?> ICrudReadTransaction.Read<TModel>(object key) where TModel : class
    {
        return DynamicRead<TModel, ServiceResult<TModel?>>(key, p => p);
    }

    Task<ServiceResult<TModel?>> ICrudReadTransaction.ReadAsync<TModel>(object key) where TModel : class
    {
        return DynamicRead<TModel, Task<ServiceResult<TModel?>>>(key, p => Task.FromResult(p));
    }

    QueryServiceResult<Model> ICrudReadTransaction.All(Type model)
    {
        var m = (GetType().GetMethod(nameof(All), 1, BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null) ??
                    typeof(ICrudReadTransaction).GetMethod(nameof(All), 1, BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null) ??
                    throw new TamperException()).MakeGenericMethod(model);
        object o = m.Invoke(this, [])!;
        ServiceResult r = (ServiceResult)o;
        if (r.Success)
        {
            return new QueryServiceResult<Model>((IQueryable<Model>)o);
        }
        else
        {
            return new QueryServiceResult<Model>(r.Reason ?? FailureReason.Unknown, r.Message);
        }
    }
    
    async Task<ServiceResult<TModel[]?>> ICrudReadTransaction.SearchAsync<TModel>(Expression<Func<TModel, bool>> predicate) where TModel : class
    {
        return (await All<TModel>().Where(predicate).ToListAsync()).ToArray();
    }

    private static bool ChkIdType(Type model, Type idType)
    {
        return typeof(Model<>).MakeGenericType(idType).IsAssignableFrom(model);
    }

    [DebuggerNonUserCode]
    private TResult DynamicRead<TModel, TResult>(object key, Func<ServiceResult<TModel?>, TResult> failureTransform, [CallerMemberName] string name = null!) where TModel : Model
    {
        return DynamicRead(typeof(TModel), key, m => failureTransform.Invoke(m.CastUp<ServiceResult<TModel?>>()), name);
    }

    [DebuggerNonUserCode]
    private TResult DynamicRead<TResult>(Type model, object key, Func<ServiceResult<Model?>, TResult> failureTransform, [CallerMemberName] string name = null!)
    {
        var t = key?.GetType() ?? throw new ArgumentNullException(nameof(key));
        if (!ChkIdType(model ?? throw new ArgumentNullException(nameof(model)), t)) return failureTransform.Invoke(new ServiceResult<Model?>(FailureReason.BadQuery));
        foreach (var j in GetType().GetMethods().Concat(typeof(ICrudReadTransaction).GetMethods()).Where(p => p.Name == name))
        {
            var args = j.GetGenericArguments();
            var para = j.GetParameters();
            if (para.Length == 1 && !para[0].IsOut && args.Length == 2 && args[0].BaseType!.Implements(typeof(Model<>)) && !args[1].IsByRef)
                return (TResult)j.MakeGenericMethod(model, t).Invoke(this, [key])!;
        }
        throw new TamperException();
    }
}
