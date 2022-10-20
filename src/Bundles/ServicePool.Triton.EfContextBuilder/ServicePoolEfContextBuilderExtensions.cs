using Microsoft.EntityFrameworkCore;
using TheXDS.MCART.Helpers;
using TheXDS.Triton.EfContextBuilder;
using TheXDS.Triton.Models.Base;

namespace TheXDS.ServicePool.Triton.EfContextBuilder;

/// <summary>
/// Contiene métodos de extensión que permiten configurar Tritón para
/// utilizarse en conjunto con
/// <see cref="ServicePool"/>.
/// </summary>
public static class ServicePoolEfContextBuilderExtensions
{
    /// <summary>
    /// Agrega un servicio de datos generado dinámicamente a la colección
    /// de servicios hosteados dentro de un <see cref="ServicePool"/>,
    /// descubriendo automáticamente los modelos a incluir en el contexto.
    /// </summary>
    /// <param name="configurable">
    /// Instancia de configuración de Tritón a utilizar para registrar un
    /// nuevo contexto dinámico.
    /// </param>
    /// <param name="optionsCallback">
    /// Llamada de configuración a utilizar cuando el contexto dinámico
    /// solicite configurarse. Puede omitirse o establecerse en
    /// <see langword="null"/> en caso de no requerir que el contexto
    /// dinámico invalide el método
    /// <see cref="DbContext.OnConfiguring(DbContextOptionsBuilder)"/>.
    /// </param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    public static ITritonConfigurable UseDynamicContext(this ITritonConfigurable configurable, Action<DbContextOptionsBuilder>? optionsCallback = null)
    {
        var t = ContextBuilder.Build(optionsCallback);
        configurable.UseContext(t.Builder.CreateType()!);
        return configurable;
    }

    /// <summary>
    /// Agrega un servicio de datos generado dinámicamente a la colección
    /// de servicios hosteados dentro de un <see cref="ServicePool"/>,
    /// especificando explícitamente los modelos a incluir en el contexto.
    /// </summary>
    /// <param name="configurable">
    /// Instancia de configuración de Tritón a utilizar para registrar un
    /// nuevo contexto dinámico.
    /// </param>
    /// <param name="models">
    /// Colección de modelos a incluir en el contexto generado dinámicamente.
    /// </param>
    /// <param name="optionsCallback">
    /// Llamada de configuración a utilizar cuando el contexto dinámico
    /// solicite configurarse. Puede omitirse o establecerse en
    /// <see langword="null"/> en caso de no requerir que el contexto
    /// dinámico invalide el método
    /// <see cref="DbContext.OnConfiguring(DbContextOptionsBuilder)"/>.
    /// </param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    public static ITritonConfigurable UseDynamicContext(this ITritonConfigurable configurable, Type[] models, Action<DbContextOptionsBuilder>? optionsCallback = null)
    {
        var t = ContextBuilder.Build(models, optionsCallback);
        configurable.UseContext(t.Builder.CreateType()!);
        return configurable;
    }

    /// <summary>
    /// Agrega un servicio de datos generado dinámicamente a la colección
    /// de servicios hosteados dentro de un <see cref="ServicePool"/>,
    /// filtrando los modelos a incluir en el contexto.
    /// </summary>
    /// <param name="configurable">
    /// Instancia de configuración de Tritón a utilizar para registrar un
    /// nuevo contexto dinámico.
    /// </param>
    /// <param name="modelFilter">
    /// Función de filtro a utilizar para seleccionar los modelos de datos
    /// específicos a incluir en el contexto generado.
    /// </param>
    /// <param name="optionsCallback">
    /// Llamada de configuración a utilizar cuando el contexto dinámico
    /// solicite configurarse. Puede omitirse o establecerse en
    /// <see langword="null"/> en caso de no requerir que el contexto
    /// dinámico invalide el método
    /// <see cref="DbContext.OnConfiguring(DbContextOptionsBuilder)"/>.
    /// </param>
    /// <returns>
    /// La misma instancia del objeto utilizado para configurar Tritón.
    /// </returns>
    public static ITritonConfigurable UseDynamicContext(this ITritonConfigurable configurable, Func<Type, bool> modelFilter, Action<DbContextOptionsBuilder>? optionsCallback = null)
    {
        return UseDynamicContext(configurable, Objects.PublicTypes<Model>().Where(modelFilter).ToArray(), optionsCallback);            
    }
}