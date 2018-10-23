using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Threading.Tasks;
using TheXDS.MCART.Attributes;
using TheXDS.Triton.Core.Component.Base;
using St=TheXDS.Triton.Core.Resources.Strings;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Core.Models.Base;

namespace TheXDS.Triton.Core.Component
{
    public abstract class Service
    {
        /// <summary>
        ///     Enumera los posibles resultados de una operación provista por un servicio.
        /// </summary>
        [Flags]
        public enum Result
        {
            /// <summary>
            ///     Operación completada correctamente.
            /// </summary>
            Ok,

            /// <summary>
            ///     La operación falló debido a un error de validación.
            /// </summary>
            ValidationFail,

            /// <summary>
            ///     La tarea no tuvo permisos para ejecutarse.
            /// </summary>
            Forbidden,

            /// <summary>
            ///     No fue posible contactar con el servidor.
            /// </summary>
            Unreachable = 4,
            
            /// <summary>
            ///     Error del servidor.
            /// </summary>
            ServerFault = 8
        }
       
        /// <summary>
        ///     Obtiene un nombre amigable para un servicio.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string FriendlyName<T>() where T : Service
        {
            return string.Format(St.Service_Friendly_Name, typeof(T).TypeName());
        }
        
        /// <summary>
        ///     Agrega una nueva entidad de forma genérica al contexto de base de datos realizando la operación de forma asíncrona.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="newEntity"></param>
        /// <returns>
        ///     <see cref="Result.Ok" /> si la operación finalizó correctamente,
        ///     <see cref="Result.Fail"/> si ocurre un problema al procesar la solicitud,
        ///     <see cref="Result.Unreachable"/> si no ha sido posible contactar al
        ///     servidor durante el tiempo se espera definido.
        /// </returns>
        public abstract Task<Result> AddAsync<TModel>(TModel newEntity) where TModel : class, new();
        public abstract Task<Result> AddAsync(object newEntity);
        
        /// <summary>
        ///     Obtiene todos los registros de una tabla.
        /// </summary>
        /// <typeparam name="TEntity">
        ///     Tipo de tabla de entidades a devolver.
        /// </typeparam>
        /// <returns>
        ///     Un <see cref="IQueryable{T}" /> que devolverá la tabla que contiene
        ///     las entidades de tipo <typeparamref name="TEntity" />.
        /// </returns>
        /// <param name="showDeleted">
        ///     Si es <see langword="true" />, se incluyen los registros borrados
        ///     que no han sido purgados.
        /// </param>
        public abstract IQueryable<TModel> All<TModel>(bool showDeleted) where TModel : class, new();

        /// <summary>
        ///     Prepara un Query que obtendrá todos los registros de una tabla.
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="showDeleted"></param>
        /// <returns></returns>
        public abstract IQueryable<ModelBase> All(Type entityType, bool showDeleted);

        /// <summary>
        ///     Obtiene todos los elementos de una tabla genérica de forma asíncrona.
        /// </summary>
        /// <typeparam name="TEntity">Tipo de elementos a obtener.</typeparam>
        /// <param name="showDeleted">Si es <see langword="true" />, se incluirán los elementos marcados como eliminados.</param>
        /// <returns>Una lista que contiene todos los elementos cargados desde la base de datos de forma asíncrona.</returns>
        public abstract Task<IList<TEntity>> AllAsync<TEntity>(bool showDeleted) where TEntity : ModelBase, new();

        /// <summary>
        ///     Obtiene todos los elementos de una tabla genérica de forma asíncrona.
        /// </summary>
        /// <param name="entityType">Tipo de elementos a obtener.</param>
        /// <param name="showDeleted">Si es <see langword="true" />, se incluirán los elementos marcados como eliminados.</param>
        /// <returns>Una lista que contiene todos los elementos cargados desde la base de datos de forma asíncrona.</returns>
        public abstract Task<IEnumerable<ModelBase>> AllAsync(Type entityType, bool showDeleted);

        /// <summary>
        ///     Obtiene todas las entidades que tengan como clase base el tipo especificado.
        /// </summary>
        /// <typeparam name="TEntity">Tipo de entidad base.</typeparam>
        /// <param name="showDeleted">
        ///     Si se establece en <see langword="true" />, se incluirán los
        ///     elementos marcados como borrados.
        /// </param>
        /// <returns>
        ///     Una enumeración con todas las entidades que tienen como clase
        ///     base a <typeparamref name="TEntity" />.
        /// </returns>
        public abstract IEnumerable<TEntity> AllBase<TEntity>(bool showDeleted) where TEntity : ModelBase;

    }
}