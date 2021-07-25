using System.Collections.Generic;
using System.Reflection;
using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Middleware
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que permita
    /// obtener información sobre los cambios sufridos por una entidad que ha
    /// sido modificada.
    /// </summary>
    public interface IOldValueProvider
    {
        /// <summary>
        /// Enumera las propiedades que ham cambiado en la entidad.
        /// </summary>
        /// <param name="entity">Entidad que ha sido modificada.</param>
        /// <returns>
        /// Una enumeración con las propiedades que han sufrido cambios, así
        /// como también el valor anterior de las mismas.
        /// </returns>
        IEnumerable<KeyValuePair<PropertyInfo, object?>>? GetOldValues(Model? entity);
    }
}