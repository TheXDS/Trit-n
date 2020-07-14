using System;
using System.Collections.Generic;
using System.Reflection;

namespace TheXDS.Triton.CrudGen.Base
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que permita
    /// exponer la descripción de generación de Crud para un constructor de UI.
    /// </summary>
    public interface IPropertyDescription
    {
        /// <summary>
        /// Obtiene la información por reflexión de la propiedad descrita.
        /// </summary>
        PropertyInfo Property { get; }

        /// <summary>
        /// Obtiene la información por reflexión del modelo descrito.
        /// </summary>
        Type Model { get; }

        /// <summary>
        /// Obtiene el valor predeterminado a establecer a esta instancia.
        /// </summary>
        object? DefaultValue { get; }

        /// <summary>
        /// Obtiene un valor que indica si los controles o widgets generados
        /// para esta propiedad serán de solo lectura.
        /// </summary>
        bool ReadOnly { get; }

        /// <summary>
        /// Obtiene una etiqueta amigable para los controles o widgets
        /// generados para la propiedad.
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Obtiene un valor que indica un grupo de pertenencia para la
        /// propiedad, o <see langword="null"/> si la propiedad no pertenece a
        /// ningún grupo.
        /// </summary>
        string? Group { get; }
        
        /// <summary>
        /// Obtiene un valor posicional para la propiedad, o
        /// <see langword="null"/> si no se ha establecido un orden especìfico
        /// para la misma.
        /// </summary>
        int? Position { get; }

        /// <summary>
        /// Obtiene un diccionario que contiene todos los valores configurados
        /// en la descripción de la propiedad.
        /// </summary>
        IDictionary<Guid, object?> Configurations { get; }
    }
}
