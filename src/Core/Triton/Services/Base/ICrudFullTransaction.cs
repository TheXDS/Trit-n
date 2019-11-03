using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Services.Base
{
    /// <summary>
    ///     Define una serie de miembros a implementar por un tipo que permita
    ///     realizar operaciones de lectura sobre una base de datos.
    /// </summary>
    public interface ICrudReadTransaction : IDisposable
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
    }

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

    /// <summary>
    ///     Define una serie de miembros a implementar por un tipo que permita
    ///     realizar operaciones de lectura y de escritura basadas en
    ///     transacción sobre una base de datos.
    /// </summary>
    public interface ICrudFullTransaction : ICrudReadTransaction, ICrudWriteTransaction, IDisposable
    {
    }

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
        /// <param name="entity">
        ///     Parámetro de salida. Entidad obtenida en la operación de
        ///     lectura. Si no existe una entidad con el campo llave
        ///     especificado, se devolverá <see langword="null"/>.
        /// </param>
        /// <returns>
        ///     El resultado reportado de la operación ejecutada por el
        ///     servicio subyacente.
        /// </returns>
        Task<ServiceResult<TModel?>> ReadAsync<TModel, TKey>(TKey key) where TModel : Model<TKey> where TKey : IComparable<TKey>, IEquatable<TKey>;
    }

    /// <summary>
    ///     Define una serie de miembros a implementar por un tipo que permita
    ///     realizar operaciones de escritura con efecto inmediato de forma
    ///     asíncrona basadas en transacción sobre una base de datos.
    /// </summary>
    public interface IAsyncCrudWriteTransaction : IDisposable
    {
        /// <summary>
        ///     Crea una nueva entidad en la base de datos de forma asíncrona.
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
        Task<ServiceResult> CreateAsync<TModel>(TModel newEntity) where TModel : Model;

        /// <summary>
        ///     Elimina a una entidad de la base de datos de forma asíncrona.
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
        Task<ServiceResult> DeleteAsync<TModel>(TModel entity) where TModel : Model;

        /// <summary>
        ///     Elimina a una entidad de la base de datos de forma asíncrona.
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
        Task<ServiceResult> DeleteAsync<TModel, TKey>(TKey key) where TModel : Model<TKey> where TKey : IComparable<TKey>, IEquatable<TKey>;

        /// <summary>
        ///     Actualiza de forma asíncrona los datos contenidos en una
        ///     entidad dentro de la base de datos.
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
        Task<ServiceResult> UpdateAsync<TModel>(TModel entity) where TModel : Model;
    }

    /// <summary>
    ///     Define una serie de miembros a implementar por un tipo que permita
    ///     realizar operaciones asíncronas de lectura y de escritura basadas
    ///     en transacción sobre una base de datos.
    /// </summary>
    public interface IAsyncCrudFullTransaction : IAsyncCrudWriteTransaction, IAsyncCrudReadTransaction
    {
    }


    public interface ICrudTransactionFactory
    {
        ICrudFullTransaction ManufactureTransaction<T>(IConnectionConfiguration configuration) where T : DbContext, new();

        ICrudReadTransaction ManufactureReadTransaction<T>(IConnectionConfiguration configuration) where T : DbContext, new()
        {
            return ManufactureTransaction<T>(configuration);
        }

        ICrudWriteTransaction ManufactureWriteTransaction<T>(IConnectionConfiguration configuration) where T : DbContext, new()
        {
            return ManufactureTransaction<T>(configuration);
        }
    }
}
