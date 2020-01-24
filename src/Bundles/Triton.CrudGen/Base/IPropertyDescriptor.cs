using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.CrudGen.Base
{
    /// <summary>
    /// Para los métodos que lo requieran, indica el objetivo de generación
    /// dinámica de UI.
    /// </summary>
    [Flags]
    public enum GenerationTarget : byte
    {
        /// <summary>
        /// Vista autogenerada.
        /// </summary>
        View = 1,
        /// <summary>
        /// Editor autogenerado.
        /// </summary>
        Editor = 2
    }

    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que permita
    /// describir una propiedad dentro de un modelo o un ViewModel para generar
    /// páginas de operacioes Crud automáticamente.
    /// </summary>
    /// <typeparam name="TModel">Modelo de la entidad a describir.</typeparam>
    /// <typeparam name="TProperty">Tipo de propiedad seleccionada.</typeparam>
    public interface IPropertyDescriptor<in TModel, in TProperty> where TModel : Model
    {
        /// <summary>
        /// Obtiene la información por reflexión de la propiedad descrita.
        /// </summary>
        PropertyInfo Property { get; }

        /// <summary>
        /// Indica el valor predeterminado al cual establecer la propiedad al
        /// crear una nueva entidad.
        /// </summary>
        /// <param name="value">
        /// Valor predeterminado al cual establecer la propiedad al crear una 
        /// nueva entidad.
        /// </param>
        /// <returns>
        /// La misma instancia sobre la cual se ha llamado al método.
        /// </returns>
        IPropertyDescriptor<TModel, TProperty> DefaultValue(TProperty value);

        /// <summary>
        /// Indica que el campo se debe ocultar del objetivo de generación
        /// dinámica de UI.
        /// </summary>
        /// <param name="target">
        /// Banderas que indican el objetivo de generación de UI en el cual se
        /// debe ocultar la propiedad.
        /// </param>
        /// <returns>
        /// La misma instancia sobre la cual se ha llamado al método.
        /// </returns>
        IPropertyDescriptor<TModel, TProperty> HideIn(GenerationTarget target);

        /// <summary>
        /// Indica que un campo será de sólo lectura en el editor autogenerado.
        /// </summary>
        /// <returns>
        /// La misma instancia sobre la cual se ha llamado al método.
        /// </returns>
        IPropertyDescriptor<TModel, TProperty> ReadOnly();

        /// <summary>
        /// Establece una etiqueta descriptiva corta del campo representado por
        /// la propiedad descrita.
        /// </summary>
        /// <returns>
        /// La misma instancia sobre la cual se ha llamado al método.
        /// </returns>
        IPropertyDescriptor<TModel, TProperty> Label(string label);

        /// <summary>
        /// Establece un valor personalizado dentro del descriptor.
        /// </summary>
        /// <param name="guid">
        /// Identificador global del tipo de valor personalizado.
        /// </param>
        /// <param name="value">Valor personalizado a establecer.</param>
        /// <returns>
        /// La misma instancia sobre la cual se ha llamado al método.
        /// </returns>
        IPropertyDescriptor<TModel, TProperty> SetCustomConfigurationValue(Guid guid, object? value);
    }

    public interface IPropertyDescription
    {

    }

    public static class PropertyDescriptorCommonExtensions
    {
        public static IPropertyDescriptor<TModel, TProperty> Nullable<TModel, TProperty>(this IPropertyDescriptor<TModel, TProperty> descriptor) where TModel : Model where TProperty : class
        {
            return descriptor;
        }
    }
}
