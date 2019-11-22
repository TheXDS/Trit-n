using System;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.MCART.Types.Base;
using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Services.Base
{
    /// <summary>
    ///     Define una serie de miembros a implementar por un tipo que permita
    ///     realizar operaciones de lectura sobre una base de datos.
    /// </summary>
    public interface ICrudReadTransaction : IDisposableEx
    {
        /// <summary>
        ///     Obtiene una entidad cuyo campo llave sea igual al valor
        ///     especificado.
        /// </summary>
        /// <typeparam name="TModel">
        ///     Modelo de la entidad a obtener.
        /// </typeparam>
        /// <typeparam name="TKey">
        ///     Tipo de campo llave de la entidad a obtener.
        /// </typeparam>
        /// <param name="key">
        ///     Llave de la entidad a obtener.
        /// </param>
        /// <param name="entity">
        ///     Parámetro de salida. Entidad obtenida en la operación de
        ///     lectura. Si no existe una entidad con el campo llave
        ///     especificado, se devolverá <see langword="null"/>.
        /// </param>
        /// <returns>
        ///     El resultado reportado de la operación ejecutada por el
        ///     servicio subyacente.
        /// </returns>
        ServiceResult Read<TModel, TKey>(TKey key, out TModel? entity) where TModel : Model<TKey> where TKey : IComparable<TKey>, IEquatable<TKey>
        {
            var r = Read<TModel, TKey>(key);
            entity = r.ReturnValue;
            return r;
        }

        /// <summary>
        ///     Obtiene una entidad cuyo campo llave sea igual al valor
        ///     especificado.
        /// </summary>
        /// <typeparam name="TModel">
        ///     Modelo de la entidad a obtener.
        /// </typeparam>
        /// <typeparam name="TKey">
        ///     Tipo de campo llave de la entidad a obtener.
        /// </typeparam>
        /// <param name="key">
        ///     Llave de la entidad a obtener.
        /// </param>
        /// <returns>
        ///     El resultado reportado de la operación ejecutada por el
        ///     servicio subyacente, incluyendo como valor de resultado a la
        ///     entidad obtenida en la operación de lectura. Si no existe una
        ///     entidad con el campo llave especificado, el valor de resultado
        ///     será <see langword="null"/>.
        /// </returns>
        ServiceResult<TModel?> Read<TModel, TKey>(TKey key) where TModel : Model<TKey> where TKey : IComparable<TKey>, IEquatable<TKey>;

        /// <summary>
        ///     Obtiene la colección completa de entidades del modelo
        ///     especificado almacenadas en la base de datos.
        /// </summary>
        /// <typeparam name="TModel">
        ///     Modelo de las entidades a obtener.
        /// </typeparam>
        /// <returns></returns>
        QueryServiceResult<TModel> All<TModel>() where TModel : Model;

        /// <summary>
        ///     Obtiene de forma asíncrona una entidad cuyo campo llave sea
        ///     igual al valor especificado.
        /// </summary>
        /// <typeparam name="TModel">
        ///     Modelo de la entidad a obtener.
        /// </typeparam>
        /// <typeparam name="TKey">
        ///     Tipo de campo llave de la entidad a obtener.
        /// </typeparam>
        /// <param name="key">
        ///     Llave de la entidad a obtener.
        /// </param>
        /// <returns>
        ///     El resultado reportado de la operación ejecutada por el
        ///     servicio subyacente.
        /// </returns>
        Task<ServiceResult<TModel?>> ReadAsync<TModel, TKey>(TKey key) where TModel : Model<TKey> where TKey : IComparable<TKey>, IEquatable<TKey>;
    }
}
