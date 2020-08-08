using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace TheXDS.Triton.Services.Base
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que funcione
    /// como un Host para servicios de Tritón.
    /// </summary>
    public interface IServiceHost
    {
        /// <summary>
        /// Enumera los servicios existentes en este Host.
        /// </summary>
        IEnumerable<IService> Services { get; }

        /// <summary>
        /// Obtiene un servicio que administre el contexto solicitado.
        /// </summary>
        /// <typeparam name="TContext">
        /// Tipo de contexto a administrar.
        /// </typeparam>
        /// <returns>
        /// Un servicio registrado que administre un contexto del tipo
        /// solicitado, o un nuevo <see cref="SimpleService{TContext}"/> en
        /// caso que no exista un servicio registrado para administrar el
        /// contexto.
        /// </returns>
        /// <remarks>
        /// Implementaciones personalizadas de este método podrían generar
        /// una excepción en lugar de devolver un
        /// <see cref="SimpleService{TContext}"/> si no hay un servicio
        /// registrado para administrar el contexto solicitado.
        /// </remarks>
        IService ServiceForContext<TContext>() where TContext : DbContext, new()
        {
            return Services.FirstOrDefault(p => p.ContextType == typeof(TContext)) ?? new SimpleService<TContext>();
        }

        /// <summary>
        /// Obtiene un valor que determina si existe registrado un servicio
        /// que administre un contexto del tipo especificado.
        /// </summary>
        /// <typeparam name="TContext">
        /// Tipo de contexto a administrar.
        /// </typeparam>
        /// <returns>
        /// <see langword="true"/> si existe un servicio que sea capaz de
        /// administrar un contexto de datos del tipo especificado,
        /// <see langword="false"/> en caso contrario.
        /// </returns>
        bool AnyServiceForContext<TContext>() where TContext : DbContext, new()
        {
            return Services.Any(p => p.ContextType == typeof(TContext));
        }
    }
}