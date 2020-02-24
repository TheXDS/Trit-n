using System;
using System.Collections.Generic;
using System.Reflection;
using TheXDS.MCART;
using TheXDS.MCART.ViewModel;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Models.Base;
using System.Linq;
using TheXDS.MCART.Types;
using St = TheXDS.Triton.CrudGen.Resources.Strings;
using TheXDS.MCART.Attributes;

namespace TheXDS.Triton.CrudGen.Base
{
    /// <summary>
    /// Contiene métodos de descripción comunes.
    /// </summary>
    public static class PropertyDescriptorExtensions
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

        /// <summary>
        /// Marca una propiedad de texto para funcionar como un campo grande de texto.
        /// </summary>
        /// <typeparam name="TModel">Tipo de modelo descrito.</typeparam>
        /// <typeparam name="TViewModel">Tipo de ViewModel editor.</typeparam>
        /// <param name="descriptor">
        /// Instancia de descriptor de propiedad sobre la cual aplicar la
        /// configuración.
        /// </param>
        /// <returns>
        /// La misma instancia que <paramref name="descriptor"/>, permitiendo
        /// el uso de sintaxis Fluent.
        /// </returns>
        public static IPropertyDescriptor<TModel, string, TViewModel> Big<TModel, TViewModel>
            (this IPropertyDescriptor<TModel, string, TViewModel> descriptor)
            where TModel : Model where TViewModel : ViewModel<TModel>
        {
            return TextKind(descriptor, Base.TextKind.Big);
        }

        /// <summary>
        /// Marca un campo de texto para funcionar como almacenamiento de ruta
        /// de archivo, aplicando la colección de filtros de archivo 
        /// especificados.
        /// </summary>
        /// <summary>
        /// Marca una propiedad de texto para funcionar como un campo grande de texto.
        /// </summary>
        /// <typeparam name="TModel">Tipo de modelo descrito.</typeparam>
        /// <typeparam name="TViewModel">Tipo de ViewModel editor.</typeparam>
        /// <param name="descriptor">
        /// Instancia de descriptor de propiedad sobre la cual aplicar la
        /// configuración.
        /// </param>
        /// <returns>
        /// La misma instancia que <paramref name="descriptor"/>, permitiendo
        /// el uso de sintaxis Fluent.
        /// </returns>
        /// <param name="extensions">
        /// Colección de filtros de extensión a utilizar.
        /// </param>
        public static IPropertyDescriptor<TModel, string, TViewModel> FilePath<TModel, TViewModel>
            (this IPropertyDescriptor<TModel, string, TViewModel> descriptor, (string Description, string[] Extensions)[] extensions)
            where TModel : Model where TViewModel : ViewModel<TModel>
        {
            return TextKind(descriptor, Base.TextKind.FilePath)
                .SetCustomConfigurationValue(GetGuid(), extensions);
        }

        /// <summary>
        /// Marca un campo de texto para funcionar como almacenamiento de ruta
        /// de archivo.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public static IPropertyDescriptor<TModel, string, TViewModel> FilePath<TModel, TViewModel>
            (this IPropertyDescriptor<TModel, string, TViewModel> descriptor)
            where TModel : Model where TViewModel : ViewModel<TModel>
        {
            return FilePath(descriptor, new[] { CommonFileFilters.AllFiles });
        }

        public static IPropertyDescriptor<TModel, string, TViewModel> PicturePath<TModel, TViewModel>
            (this IPropertyDescriptor<TModel, string, TViewModel> descriptor, (string Description, string[] Extensions)[] extensions)
            where TModel : Model where TViewModel : ViewModel<TModel>
        {
            return TextKind(descriptor, Base.TextKind.PicturePath)
                .SetCustomConfigurationValue(GetGuid(), extensions);
        }

        public static IPropertyDescriptor<TModel, string, TViewModel> PicturePath<TModel, TViewModel>
            (this IPropertyDescriptor<TModel, string, TViewModel> descriptor)
            where TModel : Model where TViewModel : ViewModel<TModel>
        {
            return FilePath(descriptor, new[] { CommonFileFilters.BitmapPictures });
        }

        public static IPropertyDescriptor<TModel, TProperty, TViewModel> Range<TModel, TProperty, TViewModel>
            (this IPropertyDescriptor<TModel, TProperty, TViewModel> descriptor, Range<TProperty> range)
            where TModel : Model
            where TProperty: IComparable<TProperty>
            where TViewModel : ViewModel<TModel>
        {
            return descriptor.SetCustomConfigurationValue(GetGuid(), range);
        }

        public static IPropertyDescriptor<TModel, TProperty, TViewModel> Range<TModel, TProperty, TViewModel>
            (this IPropertyDescriptor<TModel, TProperty, TViewModel> descriptor, TProperty min, TProperty max)
            where TModel : Model
            where TProperty : IComparable<TProperty>
            where TViewModel : ViewModel<TModel>
        {
            return Range(descriptor, new Range<TProperty>(min, max));
        }

        public static IPropertyDescriptor<TModel, string, TViewModel> MaxLength<TModel, TViewModel>
            (this IPropertyDescriptor<TModel, string, TViewModel> descriptor, int maxLength)
            where TModel : Model where TViewModel : ViewModel<TModel>
        {
            return descriptor.SetCustomConfigurationValue(GetGuid(), maxLength);
        }














        internal static IPropertyDescriptor<TModel, string, TViewModel> TextKind<TModel, TViewModel>
            (IPropertyDescriptor<TModel, string, TViewModel> descriptor, TextKind kind)
            where TModel : Model where TViewModel : ViewModel<TModel>
        {
            return descriptor.SetCustomConfigurationValue(GetGuid(), kind);
        }
    }

    /// <summary>
    /// Contiene una colección de filtros de archivos a utilizar.
    /// </summary>
    public static class CommonFileFilters
    {
        public static readonly (string, string[]) AllFiles = (St.AllFiles, new[] { "*" });
        public static readonly (string, string[]) TextFiles = (St.AllFiles, new[] { "*.txt", "*.log" });
        public static readonly (string, string[]) BitmapPictures = (St.AllFiles, new[] { "*.bmp", "*.png", "*.jpg", "*.jpeg", "*.jpe", "*.gif" });
    }
}
