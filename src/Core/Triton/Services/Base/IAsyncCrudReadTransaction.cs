using System;
using System.Threading.Tasks;
using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Services.Base
{
    /// <summary>
    ///     Define una serie de miembros a implementar por un tipo que permita
    ///     realizar operaciones de lectura asíncrona sobre una base de datos.
    /// </summary>
    public interface IAsyncCrudReadTransaction : IDisposable
    {
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
