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
        /// Obtiene un diccionario que contiene todos los valores configurados en la descripción de la propiedad.
        /// </summary>
        public IDictionary<Guid, object?> Configurations { get; }
    }
}
