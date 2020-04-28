using System;
using TheXDS.MCART.ViewModel;
using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.CrudGen.Base
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que permita
    /// describir un modelo de datos por medio del establecimiento de
    /// propiedades de descripción.
    /// </summary>
    public interface IDescriptor
    {
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
        IDescriptor SetCustomConfigurationValue(Guid guid, object? value);
    }

    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que permita
    /// describir una propiedad dentro de un modelo o un ViewModel para generar
    /// páginas de operacioes Crud automáticamente.
    /// </summary>
    /// <typeparam name="TModel">Modelo de la entidad a describir.</typeparam>
    /// <typeparam name="TProperty">Tipo de propiedad seleccionada.</typeparam>
    /// <typeparam name="TViewModel">Tipo de ViewModel editor.</typeparam>
    public interface IPropertyDescriptor<in TModel, in TProperty, in TViewModel> : IDescriptor where TModel : Model where TViewModel : ViewModel<TModel>
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
        IPropertyDescriptor<TModel, TProperty, TViewModel> DefaultValue(TProperty value);

        /// <summary>
        /// Indica que un campo será de sólo lectura en el editor autogenerado.
        /// Se establece automáticamente para propiedades de solo lectura.
        /// </summary>
        /// <returns>
        /// La misma instancia sobre la cual se ha llamado al método.
        /// </returns>
        IPropertyDescriptor<TModel, TProperty, TViewModel> ReadOnly();

        /// <summary>
        /// Establece una etiqueta descriptiva corta del campo representado por
        /// la propiedad descrita.
        /// </summary>
        /// <returns>
        /// La misma instancia sobre la cual se ha llamado al método.
        /// </returns>
        IPropertyDescriptor<TModel, TProperty, TViewModel> Label(string label);

        /// <summary>
        /// Establece un grupo de pertenencia para la propiedad.
        /// </summary>
        /// <param name="groupName">Nombre descriptivo del grupo.</param>
        /// <returns>
        /// La misma instancia sobre la cual se ha llamado al método.
        /// </returns>
        IPropertyDescriptor<TModel, TProperty, TViewModel> Group(string groupName);

        /// <summary>
        /// Establece un valor posicional para mostrar la propiedad.
        /// </summary>
        /// <param name="position">Número de posición en la cual mostrar la porpiedad descrita.</param>
        /// <returns>
        /// La misma instancia sobre la cual se ha llamado al método.
        /// </returns>
        IPropertyDescriptor<TModel, TProperty, TViewModel> Position(int position);

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
        new IPropertyDescriptor<TModel, TProperty, TViewModel> SetCustomConfigurationValue(Guid guid, object? value);
    }
}
