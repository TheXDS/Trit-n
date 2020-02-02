using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TheXDS.MCART;
using TheXDS.MCART.Types.Extensions;
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
        /// Vista vacía. Aplica únicamente a acciones perzonalizadas y a
        /// propiedades de un ViewModel.
        /// </summary>
        Empty = 1,
        /// <summary>
        /// Vista autogenerada.
        /// </summary>
        View = 2,
        /// <summary>
        /// Editor autogenerado.
        /// </summary>
        Editor = 4
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
        /// <summary>
        /// Obtiene la información por reflexión de la propiedad descrita.
        /// </summary>
        PropertyInfo Property { get; }

        /// <summary>
        /// Obtiene un diccionario que contiene todos los valores configurados en la descripción de la propiedad.
        /// </summary>
        public IDictionary<Guid, object?> Configurations { get; }
    }



    /// <summary>
    /// Describe el modo de nulabilidad a aplicar a una propiedad descrita.
    /// </summary>
    public enum NullabilityMode : byte
    {
        /// <summary>
        /// Infiere el modo de nulabilidad de acuerdo al tipo de objeto y a los
        /// atributos de nulabilidad establecidos por la sintaxis de C# 8.
        /// </summary>
        Infer,
        /// <summary>
        /// La propiedad puede aceptar <see langword="null"/>.
        /// </summary>
        Nullable,
        /// <summary>
        /// La propiedad no deberá aceptar <see langword="null"/>.
        /// </summary>
        NonNullable
    }

    /// <summary>
    /// Contiene métodos de descripción comunes.
    /// </summary>
    public static class PropertyDescriptorCommonExtensions
    {
        private static readonly Dictionary<MethodInfo, Guid> _registeredGuids = new Dictionary<MethodInfo, Guid>();

        private static Guid GetGuid(MethodInfo? m = null)
        {
            m ??= ReflectionHelpers.GetCallingMethod()!;
            return _registeredGuids.ContainsKey(m) ? _registeredGuids[m] : new Guid().PushInto(m, _registeredGuids);
        }

        /// <summary>
        /// Marca una propiedad de forma explícita para aceptar valores
        /// <see langword="null"/>.
        /// </summary>
        /// <typeparam name="TModel">Tipo de modelo descrito.</typeparam>
        /// <typeparam name="TProperty">
        /// Información de tipo devuelto por la propiedad descrita.
        /// </typeparam>
        /// <param name="descriptor">
        /// Instancia de descriptor de propiedad sobre la cual aplicar la
        /// configuración.
        /// </param>
        /// <returns>
        /// La misma instancia que <paramref name="descriptor"/>, permitiendo
        /// el uso de sintaxis Fluent.
        /// </returns>
        public static IPropertyDescriptor<TModel, TProperty> Nullable<TModel, TProperty>(this IPropertyDescriptor<TModel, TProperty> descriptor) where TModel : Model where TProperty : class
        {
            return NullMode(descriptor, NullabilityMode.Nullable);
        }

        /// <summary>
        /// Marca una propiedad de forma explícita para inferir la posibilidad
        /// de aceptar valores <see langword="null"/>. Este es el valor 
        /// predeterminado para todas las propiedades.
        /// </summary>
        /// <typeparam name="TModel">Tipo de modelo descrito.</typeparam>
        /// <typeparam name="TProperty">
        /// Información de tipo devuelto por la propiedad descrita.
        /// </typeparam>
        /// <param name="descriptor">
        /// Instancia de descriptor de propiedad sobre la cual aplicar la
        /// configuración.
        /// </param>
        /// <returns>
        /// La misma instancia que <paramref name="descriptor"/>, permitiendo
        /// el uso de sintaxis Fluent.
        /// </returns>
        public static IPropertyDescriptor<TModel, TProperty> InferNullability<TModel, TProperty>(this IPropertyDescriptor<TModel, TProperty> descriptor) where TModel : Model where TProperty : class
        {
            return NullMode(descriptor, NullabilityMode.Infer);
        }

        public static IPropertyDescriptor<TModel, TProperty> NullMode<TModel, TProperty>(this IPropertyDescriptor<TModel, TProperty> descriptor, NullabilityMode mode) where TModel : Model where TProperty : class
        {
            return descriptor.SetCustomConfigurationValue(GetGuid(), mode);
        }

        /// <summary>
        /// Marca una propiedad de forma explícita para no aceptar valores
        /// <see langword="null"/>.
        /// </summary>
        /// <typeparam name="TModel">Tipo de modelo descrito.</typeparam>
        /// <typeparam name="TProperty">
        /// Información de tipo devuelto por la propiedad descrita.
        /// </typeparam>
        /// <param name="descriptor">
        /// Instancia de descriptor de propiedad sobre la cual aplicar la
        /// configuración.
        /// </param>
        /// <returns>
        /// La misma instancia que <paramref name="descriptor"/>, permitiendo
        /// el uso de sintaxis Fluent.
        /// </returns>
        public static IPropertyDescriptor<TModel, TProperty> NonNullable<TModel, TProperty>(this IPropertyDescriptor<TModel, TProperty> descriptor) where TModel : Model where TProperty : class
        {
            return NullMode(descriptor, NullabilityMode.NonNullable);
        }

    }
}
