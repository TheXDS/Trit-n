using System;
using System.Collections.Generic;
using System.Reflection;
using TheXDS.MCART;
using TheXDS.MCART.ViewModel;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.CrudGen.Base
{
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
        /// <typeparam name="TViewModel">Tipo de ViewModel editor.</typeparam>
        /// <param name="descriptor">
        /// Instancia de descriptor de propiedad sobre la cual aplicar la
        /// configuración.
        /// </param>
        /// <returns>
        /// La misma instancia que <paramref name="descriptor"/>, permitiendo
        /// el uso de sintaxis Fluent.
        /// </returns>
        public static IPropertyDescriptor<TModel, TProperty, TViewModel> Nullable<TModel, TProperty, TViewModel>(this IPropertyDescriptor<TModel, TProperty, TViewModel> descriptor) where TModel : Model where TProperty : class where TViewModel : ViewModel<TModel>
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
        /// <typeparam name="TViewModel">Tipo de ViewModel editor.</typeparam>
        /// <param name="descriptor">
        /// Instancia de descriptor de propiedad sobre la cual aplicar la
        /// configuración.
        /// </param>
        /// <returns>
        /// La misma instancia que <paramref name="descriptor"/>, permitiendo
        /// el uso de sintaxis Fluent.
        /// </returns>
        public static IPropertyDescriptor<TModel, TProperty, TViewModel> InferNullability<TModel, TProperty, TViewModel>(this IPropertyDescriptor<TModel, TProperty, TViewModel> descriptor) where TModel : Model where TProperty : class where TViewModel : ViewModel<TModel>
        {
            return NullMode(descriptor, NullabilityMode.Infer);
        }

        /// <summary>
        /// Marca una propiedad de forma explícita con el modo de nulabilidad a
        /// utilizar para generar el control de edición en la página de Crud.
        /// </summary>
        /// <typeparam name="TModel">Tipo de modelo descrito.</typeparam>
        /// <typeparam name="TProperty">
        /// Información de tipo devuelto por la propiedad descrita.
        /// </typeparam>
        /// <typeparam name="TViewModel">Tipo de ViewModel editor.</typeparam>
        /// <param name="descriptor">
        /// Instancia de descriptor de propiedad sobre la cual aplicar la
        /// configuración.
        /// </param>
        /// <param name="mode">
        /// Modo de nulabilidad a utilizar para generar el control de edición
        /// en la página de Crud.
        /// </param>
        /// <returns>
        /// La misma instancia que <paramref name="descriptor"/>, permitiendo
        /// el uso de sintaxis Fluent.
        /// </returns>
        public static IPropertyDescriptor<TModel, TProperty, TViewModel> NullMode<TModel, TProperty, TViewModel>(this IPropertyDescriptor<TModel, TProperty, TViewModel> descriptor, NullabilityMode mode) where TModel : Model where TProperty : class where TViewModel : ViewModel<TModel>
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
        public static IPropertyDescriptor<TModel, TProperty, TViewModel> NonNullable<TModel, TProperty, TViewModel>(this IPropertyDescriptor<TModel, TProperty, TViewModel> descriptor) where TModel : Model where TProperty : class where TViewModel : ViewModel<TModel>
        {
            return NullMode(descriptor, NullabilityMode.NonNullable);
        }
    }
}
