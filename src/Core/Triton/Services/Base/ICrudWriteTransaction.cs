#define PreferGenerics

using System.Collections;
using System.Runtime.CompilerServices;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Services.Base;

/// <summary>
/// Define una serie de miembros a implementar por un tipo que permita
/// realizar operaciones de escritura basadas en transacción sobre una
/// base de datos.
/// </summary>
public interface ICrudWriteTransaction : IDisposableEx, IAsyncDisposable
{
#if PreferGenerics

    private ServiceResult InvokeGeneric(Model[] entities, [CallerMemberName]string methodName = null!)
    {
        var genArg = Type.MakeGenericMethodParameter(0).MakeArrayType();
        var crudMethod = GetType().GetMethod(methodName, new Type[] { genArg }) ?? throw new MissingMethodException();
        var ofTypeMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.OfType), new Type[] { typeof(IEnumerable) }) ?? throw new MissingMethodException();
        var toArrayMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToArray), new Type[] { genArg }) ?? throw new MissingMethodException();
        foreach (var g in entities.GroupBy(p => p.GetType()))
        {
            var m = crudMethod.MakeGenericMethod(new[] { g.Key });
            var om = ofTypeMethod.MakeGenericMethod(new[] { g.Key });
            var tm = toArrayMethod.MakeGenericMethod(new[] { g.Key });
            var typedModelCollection = om.Invoke(null, new[] { g });
            var r = (ServiceResult)m.Invoke(this, new[] { tm.Invoke(null, new[] { typedModelCollection }) })!;
            if (!r.Success) return r;
        }
        return ServiceResult.Ok;
    }

    /// <summary>
    /// Crea un conjunto de entidades en la base de datos.
    /// </summary>
    /// <typeparam name="TModel">
    /// Modelo de las nuevas entidades.
    /// </typeparam>
    /// <param name="entities">
    /// Conjunto de entidades a ser agregadas a la base de datos.
    /// </param>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el
    /// servicio subyacente.
    /// </returns>
    ServiceResult Create<TModel>(params TModel[] entities) where TModel : Model;

    /// <summary>
    /// Crea un conjunto de entidades en la base de datos.
    /// </summary>
    /// <param name="entities">
    /// Conjunto de entidades a ser agregadas a la base de datos.
    /// </param>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el
    /// servicio subyacente.
    /// </returns>
    ServiceResult Create(params Model[] entities) => InvokeGeneric(entities);

    /// <summary>
    /// Crea o actualiza un conjunto de entidades en la base de datos.
    /// </summary>
    /// <typeparam name="TModel">
    /// Modelo de las entidades a crear o actualizar.
    /// </typeparam>
    /// <param name="entities">
    /// Conjunto de entidades a ser agregadas o actualizadas en la base de
    /// datos.
    /// </param>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el servicio
    /// subyacente.
    /// </returns>
    ServiceResult CreateOrUpdate<TModel>(params TModel[] entities) where TModel : Model;

    /// <summary>
    /// Crea o actualiza un conjunto de entidades en la base de datos.
    /// </summary>
    /// <param name="entities">
    /// Conjunto de entidades a ser agregadas o actualizadas en la base de
    /// datos.
    /// </param>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el servicio
    /// subyacente.
    /// </returns>
    ServiceResult CreateOrUpdate(params Model[] entities) => InvokeGeneric(entities);

    /// <summary>
    /// Actualiza los datos contenidos en una entidad dentro de la base
    /// de datos.
    /// </summary>
    /// <typeparam name="TModel">
    /// Modelo de la entidad a actualizar.
    /// </typeparam>
    /// <param name="entities">
    /// Entidades que contienen la nueva información a escribir.
    /// </param>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el
    /// servicio subyacente.
    /// </returns>
    ServiceResult Update<TModel>(params TModel[] entities) where TModel : Model;

    /// <summary>
    /// Actualiza los datos contenidos en una entidad dentro de la base
    /// de datos.
    /// </summary>
    /// <param name="entities">
    /// Entidades que contienen la nueva información a escribir.
    /// </param>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el
    /// servicio subyacente.
    /// </returns>
    ServiceResult Update(params Model[] entities) => InvokeGeneric(entities);

    /// <summary>
    /// Elimina a una entidad de la base de datos.
    /// </summary>
    /// <typeparam name="TModel">
    /// Modelo de la entidad a eliminar.
    /// </typeparam>
    /// <param name="entities">
    /// Entidad que deberá ser eliminada de la base de datos.
    /// </param>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el
    /// servicio subyacente.
    /// </returns>
    ServiceResult Delete<TModel>(params TModel[] entities) where TModel : Model;

    /// <summary>
    /// Elimina a una entidad de la base de datos.
    /// </summary>
    /// <param name="entities">
    /// Entidad que deberá ser eliminada de la base de datos.
    /// </param>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el
    /// servicio subyacente.
    /// </returns>
    ServiceResult Delete(params Model[] entities) => InvokeGeneric(entities);

    /// <summary>
    /// Elimina a una entidad de la base de datos.
    /// </summary>
    /// <typeparam name="TModel">
    /// Modelo de la entidad a eliminar.
    /// </typeparam>
    /// <typeparam name="TKey">
    /// Tipo del campo llave que identifica a la entidad.
    /// </typeparam>
    /// <param name="keys">
    /// Llaves de las entidades que deberán ser eliminadas de la base de datos.
    /// </param>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el
    /// servicio subyacente.
    /// </returns>
    ServiceResult Delete<TModel, TKey>(params TKey[] keys) where TModel : Model<TKey>, new() where TKey : IComparable<TKey>, IEquatable<TKey>;

    /// <summary>
    /// Elimina a una entidad de la base de datos.
    /// </summary>
    /// <typeparam name="TModel">
    /// Modelo de la entidad a eliminar.
    /// </typeparam>
    /// <param name="stringKeys">
    /// Llaves de las entidades que deberán ser eliminadas de la base de datos.
    /// </param>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el
    /// servicio subyacente.
    /// </returns>
    ServiceResult Delete<TModel>(params string[] stringKeys) where TModel : Model, new();

#else

    /// <summary>
    /// Crea un conjunto de entidades en la base de datos.
    /// </summary>
    /// <typeparam name="TModel">
    /// Modelo de las nuevas entidades.
    /// </typeparam>
    /// <param name="entities">
    /// Conjunto de entidades a ser agregadas a la base de datos.
    /// </param>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el
    /// servicio subyacente.
    /// </returns>
    ServiceResult Create<TModel>(params TModel[] entities) where TModel : Model => Create(entities.Cast<Model>().ToArray());

    /// <summary>
    /// Crea un conjunto de entidades en la base de datos.
    /// </summary>
    /// <param name="entities">
    /// Conjunto de entidades a ser agregadas a la base de datos.
    /// </param>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el
    /// servicio subyacente.
    /// </returns>
    ServiceResult Create(params Model[] entities);

    /// <summary>
    /// Crea o actualiza un conjunto de entidades en la base de datos.
    /// </summary>
    /// <typeparam name="TModel">
    /// Modelo de las entidades a crear o actualizar.
    /// </typeparam>
    /// <param name="entities">
    /// Conjunto de entidades a ser agregadas o actualizadas en la base de
    /// datos.
    /// </param>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el servicio
    /// subyacente.
    /// </returns>
    ServiceResult CreateOrUpdate<TModel>(params TModel[] entities) where TModel : Model => CreateOrUpdate(entities.Cast<Model>().ToArray());

    /// <summary>
    /// Crea o actualiza un conjunto de entidades en la base de datos.
    /// </summary>
    /// <param name="entities">
    /// Conjunto de entidades a ser agregadas o actualizadas en la base de
    /// datos.
    /// </param>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el servicio
    /// subyacente.
    /// </returns>
    ServiceResult CreateOrUpdate(params Model[] entities);

    /// <summary>
    /// Actualiza los datos contenidos en una entidad dentro de la base
    /// de datos.
    /// </summary>
    /// <typeparam name="TModel">
    /// Modelo de la entidad a actualizar.
    /// </typeparam>
    /// <param name="entities">
    /// Entidades que contienen la nueva información a escribir.
    /// </param>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el
    /// servicio subyacente.
    /// </returns>
    ServiceResult Update<TModel>(params TModel[] entities) where TModel : Model => Update(entities.Cast<Model>().ToArray());

    /// <summary>
    /// Actualiza los datos contenidos en una entidad dentro de la base
    /// de datos.
    /// </summary>
    /// <param name="entities">
    /// Entidades que contienen la nueva información a escribir.
    /// </param>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el
    /// servicio subyacente.
    /// </returns>
    ServiceResult Update(params Model[] entities);

    /// <summary>
    /// Elimina a una entidad de la base de datos.
    /// </summary>
    /// <typeparam name="TModel">
    /// Modelo de la entidad a eliminar.
    /// </typeparam>
    /// <param name="entities">
    /// Entidad que deberá ser eliminada de la base de datos.
    /// </param>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el
    /// servicio subyacente.
    /// </returns>
    ServiceResult Delete<TModel>(params TModel[] entities) where TModel : Model => Delete(entities.Cast<Model>().ToArray());

    /// <summary>
    /// Elimina a una entidad de la base de datos.
    /// </summary>
    /// <param name="entities">
    /// Entidad que deberá ser eliminada de la base de datos.
    /// </param>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el
    /// servicio subyacente.
    /// </returns>
    ServiceResult Delete(params Model[] entities);

    /// <summary>
    /// Elimina a una entidad de la base de datos.
    /// </summary>
    /// <typeparam name="TModel">
    /// Modelo de la entidad a eliminar.
    /// </typeparam>
    /// <typeparam name="TKey">
    /// Tipo del campo llave que identifica a la entidad.
    /// </typeparam>
    /// <param name="keys">
    /// Llaves de las entidades que deberán ser eliminadas de la base de datos.
    /// </param>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el
    /// servicio subyacente.
    /// </returns>
    ServiceResult Delete<TModel, TKey>(params TKey[] keys) where TModel : Model<TKey>, new() where TKey : IComparable<TKey>, IEquatable<TKey>;

    /// <summary>
    /// Elimina a una entidad de la base de datos.
    /// </summary>
    /// <typeparam name="TModel">
    /// Modelo de la entidad a eliminar.
    /// </typeparam>
    /// <param name="stringKeys">
    /// Llaves de las entidades que deberán ser eliminadas de la base de datos.
    /// </param>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el
    /// servicio subyacente.
    /// </returns>
    ServiceResult Delete<TModel>(params string[] stringKeys) where TModel : Model, new();

#endif

    /// <summary>
    /// Guarda todos los cambios pendientes de la transacción actual.
    /// </summary>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el
    /// servicio subyacente.
    /// </returns>
    ServiceResult Commit() => CommitAsync().ConfigureAwait(false).GetAwaiter().GetResult();

    /// <summary>
    /// Guarda todos los cambios realizados de forma asíncrona.
    /// </summary>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el
    /// servicio subyacente.
    /// </returns>
    Task<ServiceResult> CommitAsync();

    /// <summary>
    /// Descarta todos los cambios pendientes de ser guardados en la base de datos.
    /// </summary>
    /// <returns></returns>
    ServiceResult Discard();
}
