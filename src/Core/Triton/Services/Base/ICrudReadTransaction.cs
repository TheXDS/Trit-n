using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TheXDS.MCART.Exceptions;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Services.Base
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que permita
    /// realizar operaciones de lectura sobre una base de datos.
    /// </summary>
    public interface ICrudReadTransaction : IDisposableEx, IAsyncDisposable
    {
        /// <summary>
        /// Obtiene una entidad cuyo campo llave sea igual al valor
        /// especificado.
        /// </summary>
        /// <typeparam name="TModel">
        /// Modelo de la entidad a obtener.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// Tipo de campo llave de la entidad a obtener.
        /// </typeparam>
        /// <param name="key">
        /// Llave de la entidad a obtener.
        /// </param>
        /// <param name="entity">
        /// Parámetro de salida. Entidad obtenida en la operación de
        /// lectura. Si no existe una entidad con el campo llave
        /// especificado, se devolverá <see langword="null"/>.
        /// </param>
        /// <returns>
        /// El resultado reportado de la operación ejecutada por el
        /// servicio subyacente.
        /// </returns>
        ServiceResult Read<TModel, TKey>(TKey key, out TModel? entity) where TModel : Model<TKey> where TKey : notnull, IComparable<TKey>, IEquatable<TKey>
        {
            var r = Read<TModel, TKey>(key);
            entity = r.ReturnValue;
            return r;
        }

        /// <summary>
        /// Obtiene una entidad cuyo campo llave sea igual al valor
        /// especificado.
        /// </summary>
        /// <typeparam name="TModel">
        /// Modelo de la entidad a obtener.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// Tipo de campo llave de la entidad a obtener.
        /// </typeparam>
        /// <param name="key">
        /// Llave de la entidad a obtener.
        /// </param>
        /// <returns>
        /// El resultado reportado de la operación ejecutada por el
        /// servicio subyacente, incluyendo como valor de resultado a la
        /// entidad obtenida en la operación de lectura. Si no existe una
        /// entidad con el campo llave especificado, el valor de resultado
        /// será <see langword="null"/>.
        /// </returns>
        ServiceResult<TModel?> Read<TModel, TKey>(TKey key) where TModel : Model<TKey> where TKey : notnull, IComparable<TKey>, IEquatable<TKey>
        {
            return ReadAsync<TModel, TKey>(key).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Obtiene una entidad cuyo campo llave sea igual al valor
        /// especificado.
        /// </summary>
        /// <typeparam name="TModel">
        /// Modelo de la entidad a obtener.
        /// </typeparam>
        /// <param name="key">
        /// Llave de la entidad a obtener.
        /// </param>
        /// <returns>
        /// El resultado reportado de la operación ejecutada por el
        /// servicio subyacente, incluyendo como valor de resultado a la
        /// entidad obtenida en la operación de lectura. Si no existe una
        /// entidad con el campo llave especificado, el valor de resultado
        /// será <see langword="null"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Se produce si <paramref name="key"/> es <see langword="null"/>.
        /// </exception>
        ServiceResult<TModel?> Read<TModel>(object key) where TModel : Model
        {
            return DynamicRead<TModel, ServiceResult<TModel?>>(key, p => p);
        }

        /// <summary>
        /// Obtiene la colección completa de entidades del modelo
        /// especificado almacenadas en la base de datos.
        /// </summary>
        /// <typeparam name="TModel">
        /// Modelo de las entidades a obtener.
        /// </typeparam>
        /// <returns></returns>
        QueryServiceResult<TModel> All<TModel>() where TModel : Model;

        /// <summary>
        /// Obtiene de forma asíncrona una entidad cuyo campo llave sea
        /// igual al valor especificado.
        /// </summary>
        /// <typeparam name="TModel">
        /// Modelo de la entidad a obtener.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// Tipo de campo llave de la entidad a obtener.
        /// </typeparam>
        /// <param name="key">
        /// Llave de la entidad a obtener.
        /// </param>
        /// <returns>
        /// El resultado reportado de la operación ejecutada por el
        /// servicio subyacente.
        /// </returns>
        Task<ServiceResult<TModel?>> ReadAsync<TModel, TKey>(TKey key) where TModel : Model<TKey> where TKey : notnull, IComparable<TKey>, IEquatable<TKey>;

        /// <summary>
        /// Obtiene una entidad cuyo campo llave sea igual al valor
        /// especificado.
        /// </summary>
        /// <typeparam name="TModel">
        /// Modelo de la entidad a obtener.
        /// </typeparam>
        /// <param name="key">
        /// Llave de la entidad a obtener.
        /// </param>
        /// <returns>
        /// Una tarea que, al finalizar, contiene el resultado reportado de la
        /// operación ejecutada por el servicio subyacente, incluyendo como
        /// valor de resultado a la entidad obtenida en la operación de
        /// lectura. Si no existe una entidad con el campo llave especificado,
        /// el valor de resultado será <see langword="null"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Se produce si <paramref name="key"/> es <see langword="null"/>.
        /// </exception>
        Task<ServiceResult<TModel?>> ReadAsync<TModel>(object key) where TModel : Model
        {
            return DynamicRead<TModel, Task<ServiceResult<TModel?>>>(key, p => Task.FromResult(p));
        }

        /// <summary>
        /// Ejecuta una consulta que obtendrá un arreglo de entidades que 
        /// coinciden con el predicado especificado en
        /// <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TModel">Tipo de entidades a obtener.</typeparam>
        /// <param name="predicate">
        /// Función de filtro a aplicar al buscar entidades.
        /// </param>
        /// <returns>
        /// Una tarea que, al finalizar, contiene el resultado reportado de la
        /// operación ejecutada por el servicio subyacente, incluyendo como
        /// valor de resultado a las entidades obtenidas en la operación de
        /// lectura. Si no existen entidades que coincidan con el predicado
        /// espeficicado en <paramref name="predicate"/>, se devolverá un
        /// arreglo de tipo <typeparamref name="TModel"/> vacío.
        /// </returns>
        async Task<ServiceResult<TModel[]?>> SearchAsync<TModel>(Expression<Func<TModel, bool>> predicate) where TModel : Model
        {
            return (await All<TModel>().Where(predicate).ToListAsync()).ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ChkIdType<T>(Type idType)
        {
            return typeof(Model<>).MakeGenericType(idType).IsAssignableFrom(typeof(T));
        }

        [DebuggerNonUserCode]
        private TResult DynamicRead<TModel, TResult>(object key, Func<ServiceResult<TModel?>, TResult> failureTransform, [CallerMemberName]string name = null!) where TModel : Model
        {
            var t = key?.GetType() ?? throw new ArgumentNullException(nameof(key));
            if (!ChkIdType<TModel>(t)) return failureTransform.Invoke(new ServiceResult<TModel?>(FailureReason.BadQuery));
            foreach (var j in GetType().GetMethods().Where(p => p.Name == name))
            {
                var args = j.GetGenericArguments();
                var para = j.GetParameters();
                if (para.Length == 1 && !para[0].IsOut && args.Length == 2 && args[0].BaseType!.Implements(typeof(Model<>)) && !args[1].IsByRef)
                    return (TResult)j.MakeGenericMethod(typeof(TModel), t).Invoke(this, new[] { key })!;
            }
            throw new TamperException();
        }
    }
}
