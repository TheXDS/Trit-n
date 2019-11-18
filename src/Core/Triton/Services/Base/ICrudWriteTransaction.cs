using System;
using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Services.Base
{
    /// <summary>
    ///     Define una serie de miembros a implementar por un tipo que permita
    ///     realizar operaciones de escritura basadas en transacción sobre una
    ///     base de datos.
    /// </summary>
    public interface ICrudWriteTransaction : IDisposable
    {
        /// <summary>
        ///     Crea una nueva entidad en la base de datos.
        /// </summary>
        /// <typeparam name="TModel">
        ///     Modelo de la nueva entidad.
        /// </typeparam>
        /// <param name="newEntity">
        ///     Nueva entidad a agregar a la base de datos.
        /// </param>
        /// <returns>
        ///     El resultado reportado de la operación ejecutada por el
        ///     servicio subyacente.
        /// </returns>
        ServiceResult Create<TModel>(TModel newEntity) where TModel : Model;

        /// <summary>
        ///     Elimina a una entidad de la base de datos.
        /// </summary>
        /// <typeparam name="TModel">
        ///     Modelo de la entidad a eliminar.
        /// </typeparam>
        /// <param name="entity">
        ///     Entidad que deberá ser eliminada de la base de datos.
        /// </param>
        /// <returns>
        ///     El resultado reportado de la operación ejecutada por el
        ///     servicio subyacente.
        /// </returns>
        ServiceResult Delete<TModel>(TModel entity) where TModel : Model;

        /// <summary>
        ///     Elimina a una entidad de la base de datos.
        /// </summary>
        /// <typeparam name="TModel">
        ///     Modelo de la entidad a eliminar.
        /// </typeparam>
        /// <typeparam name="TKey">
        ///     Tipo del campo llave que identifica a la entidad.
        /// </typeparam>
        /// <param name="key">
        ///     Llave de la entidad que deberá ser eliminada de la base de
        ///     datos.
        /// </param>
        /// <returns>
        ///     El resultado reportado de la operación ejecutada por el
        ///     servicio subyacente.
        /// </returns>
        ServiceResult Delete<TModel, TKey>(TKey key) where TModel : Model<TKey> where TKey : IComparable<TKey>, IEquatable<TKey>;

        /// <summary>
        ///     Actualiza los datos contenidos en una entidad dentro de la base
        ///     de datos.
        /// </summary>
        /// <typeparam name="TModel">
        ///     Modelo de la entidad a actualizar.
        /// </typeparam>
        /// <param name="entity">
        ///     Entidad que contiene la nueva información a escribir.
        /// </param>
        /// <returns>
        ///     El resultado reportado de la operación ejecutada por el
        ///     servicio subyacente.
        /// </returns>
        ServiceResult Update<TModel>(TModel entity) where TModel : Model;

        /// <summary>
        ///     Guarda todos los cambios pendientes de la transacción actual.
        /// </summary>
        /// <returns>
        ///     El resultado reportado de la operación ejecutada por el
        ///     servicio subyacente.
        /// </returns>
        ServiceResult Commit();
    }
}
