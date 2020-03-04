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
using TheXDS.MCART.Types.Base;
using System.Collections;
using TheXDS.MCART.Exceptions;

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
        /// <typeparam name="TViewModel">Tipo de ViewModel editor.</typeparam>
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
        public static IPropertyDescriptor<TModel, string, TViewModel> Big<TModel, TViewModel>(this IPropertyDescriptor<TModel, string, TViewModel> descriptor) where TModel : Model where TViewModel : ViewModel<TModel>
        {
            return TextKind(descriptor, Base.TextKind.Big);
        }

        /// <summary>
        /// Marca un campo de texto para funcionar como almacenamiento de ruta
        /// de archivo, aplicando la colección de filtros de archivo 
        /// especificados.
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
        public static IPropertyDescriptor<TModel, string, TViewModel> FilePath<TModel, TViewModel>(this IPropertyDescriptor<TModel, string, TViewModel> descriptor, params FileFilter[] extensions) where TModel : Model where TViewModel : ViewModel<TModel>
        {
            return TextKind(descriptor, Base.TextKind.FilePath).SetCustomConfigurationValue(GetGuid(), extensions);
        }





        /// <summary>
        /// Marca un campo de texto para funcionar como almacenamiento de ruta
        /// de archivo.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public static IPropertyDescriptor<TModel, string, TViewModel> FilePath<TModel, TViewModel>(this IPropertyDescriptor<TModel, string, TViewModel> descriptor) where TModel : Model where TViewModel : ViewModel<TModel>
        {
            return FilePath(descriptor, FileFilter.AllFiles);
        }

        /// <summary>
        /// Marca un campo de texto para funcionar como almacenamiento de ruta
        /// de archivo para imágenes.
        /// </summary>
        /// <typeparam name="TModel">Tipo de modelo descrito.</typeparam>
        /// <typeparam name="TViewModel">Tipo de ViewModel editor.</typeparam>
        /// <param name="descriptor">
        /// Instancia de descriptor de propiedad sobre la cual aplicar la
        /// configuración.
        /// </param>
        /// <param name="extensions">
        /// Descriptores de extensión de archivo por medio de los cuales
        /// filtrar el tipo de archivo admitido por el cuadro de diálogo de
        /// carga.
        /// </param>
        /// <returns>
        /// La misma instancia que <paramref name="descriptor"/>, permitiendo
        /// el uso de sintaxis Fluent.
        /// </returns>
        public static IPropertyDescriptor<TModel, string, TViewModel> PicturePath<TModel, TViewModel>(this IPropertyDescriptor<TModel, string, TViewModel> descriptor, params FileFilter[] extensions) where TModel : Model where TViewModel : ViewModel<TModel>
        {
            return TextKind(descriptor, Base.TextKind.PicturePath).SetCustomConfigurationValue(GetGuid(), extensions);
        }

        /// <summary>
        /// Marca un campo de texto para funcionar como almacenamiento de ruta
        /// de archivo para imágenes.
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
        public static IPropertyDescriptor<TModel, string, TViewModel> PicturePath<TModel, TViewModel>(this IPropertyDescriptor<TModel, string, TViewModel> descriptor) where TModel : Model where TViewModel : ViewModel<TModel>
        {
            return PicturePath(descriptor, FileFilter.BitmapPictures );
        }

        /// <summary>
        /// Marca un campo de texto para funcionar como almacenamiento directo
        /// en Base64 del contenido de un archivo. Únicamente recomendable para
        /// archivos minúsculos (4 KiB o menos).
        /// </summary>
        /// <typeparam name="TModel">Tipo de modelo descrito.</typeparam>
        /// <typeparam name="TViewModel">Tipo de ViewModel editor.</typeparam>
        /// <param name="descriptor">
        /// Instancia de descriptor de propiedad sobre la cual aplicar la
        /// configuración.
        /// </param>
        /// <param name="extensions">
        /// Descriptores de extensión de archivo por medio de los cuales
        /// filtrar el tipo de archivo admitido por el cuadro de diálogo de
        /// carga.
        /// </param>
        /// <returns>
        /// La misma instancia que <paramref name="descriptor"/>, permitiendo
        /// el uso de sintaxis Fluent.
        /// </returns>
        public static IPropertyDescriptor<TModel, string, TViewModel> Base64<TModel, TViewModel>(this IPropertyDescriptor<TModel, string, TViewModel> descriptor, params FileFilter[] extensions) where TModel : Model where TViewModel : ViewModel<TModel>
        {
            return TextKind(descriptor, Base.TextKind.Base64).SetCustomConfigurationValue(GetGuid(), extensions);
        }

        /// <summary>
        /// Marca un campo de texto para funcionar como almacenamiento directo
        /// en Base64 del contenido de un archivo. Únicamente recomendable para
        /// archivos minúsculos (4 KiB o menos).
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
        public static IPropertyDescriptor<TModel, string, TViewModel> Base64<TModel, TViewModel>(this IPropertyDescriptor<TModel, string, TViewModel> descriptor) where TModel : Model where TViewModel : ViewModel<TModel>
        {
            return Base64(descriptor, FileFilter.AllFiles);
        }

        public static IPropertyDescriptor<TModel, string, TViewModel> Base64Picture<TModel, TViewModel>(this IPropertyDescriptor<TModel, string, TViewModel> descriptor, FileFilter[] extensions) where TModel : Model where TViewModel : ViewModel<TModel>
        {
            return TextKind(descriptor, Base.TextKind.Base64Picture).SetCustomConfigurationValue(GetGuid(), extensions);
        }

        public static IPropertyDescriptor<TModel, string, TViewModel> Base64Picture<TModel, TViewModel>(this IPropertyDescriptor<TModel, string, TViewModel> descriptor) where TModel : Model where TViewModel : ViewModel<TModel>
        {
            return Base64(descriptor, FileFilter.BitmapPictures);
        }

        /// <summary>
        /// Marca una porpiedad para aceptar únicamente valores dentro de un
        /// rango especificado.
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
        /// <param name="range">
        /// Rango de valores válidos para esta propiedad.
        /// </param>
        /// <returns>
        /// La misma instancia que <paramref name="descriptor"/>, permitiendo
        /// el uso de sintaxis Fluent.
        /// </returns>
        public static IPropertyDescriptor<TModel, TProperty, TViewModel> Range<TModel, TProperty, TViewModel>(this IPropertyDescriptor<TModel, TProperty, TViewModel> descriptor, Range<TProperty> range)
            where TModel : Model
            where TProperty: IComparable<TProperty>
            where TViewModel : ViewModel<TModel>
        {
            return descriptor.SetCustomConfigurationValue(GetGuid(), range);
        }

        /// <summary>
        /// Marca una porpiedad para aceptar únicamente valores dentro de un
        /// rango especificado.
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
        /// <param name="min">
        /// Valor mínimo aceptado por la propiedad, inclusive.
        /// </param>
        /// <param name="max">
        /// Valor máximo aceptado por la propiedad, inclusive.
        /// </param>
        /// <returns>
        /// La misma instancia que <paramref name="descriptor"/>, permitiendo
        /// el uso de sintaxis Fluent.
        /// </returns>
        public static IPropertyDescriptor<TModel, TProperty, TViewModel> Range<TModel, TProperty, TViewModel>(this IPropertyDescriptor<TModel, TProperty, TViewModel> descriptor, TProperty min, TProperty max)
            where TModel : Model
            where TProperty : IComparable<TProperty>
            where TViewModel : ViewModel<TModel>
        {
            return Range(descriptor, new Range<TProperty>(min, max));
        }

        /// <summary>
        /// Marca un campo de texto con un valor que describe la longitud
        /// máxima admitida por el mismo.
        /// </summary>
        /// <typeparam name="TModel">Tipo de modelo descrito.</typeparam>
        /// <typeparam name="TViewModel">Tipo de ViewModel editor.</typeparam>
        /// <param name="descriptor">
        /// Instancia de descriptor de propiedad sobre la cual aplicar la
        /// configuración.
        /// </param>
        /// <param name="maxLength">
        /// Longitud máxima a admitir para el campo.
        /// </param>
        /// <returns>
        /// La misma instancia que <paramref name="descriptor"/>, permitiendo
        /// el uso de sintaxis Fluent.
        /// </returns>
        public static IPropertyDescriptor<TModel, string, TViewModel> MaxLength<TModel, TViewModel>(this IPropertyDescriptor<TModel, string, TViewModel> descriptor, int maxLength) where TModel : Model where TViewModel : ViewModel<TModel>
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
    public struct FileFilter : IDescriptible, IEnumerable<string>
    {
        /// <summary>
        /// Obtiene un filtro que permite seleccionar todos los archivos. Este
        /// campo es de solo lectura.
        /// </summary>
        public static readonly FileFilter AllFiles = new FileFilter(St.AllFiles, "*" );

        /// <summary>
        /// Obtiene un filtro que permite seleccionar archivos de texto
        /// comunes. Este campo es de solo lectura.
        /// </summary>
        public static readonly FileFilter PlainTextFiles = new FileFilter(St.TextFiles, "txt", "log");

        /// <summary>
        /// Obtiene un filtro que permite seleccionar archivos de imagen de
        /// mapa de bits comunes. Este campo es de solo lectura.
        /// </summary>
        public static readonly FileFilter BitmapPictures = new FileFilter(St.Bitmaps, "bmp", "png", "jpg", "jpeg", "jpe", "gif");

        /// <summary>
        /// Obtiene un filtro que permite seleccionar archivos de imagen de
        /// mapa de bits comunes. Este campo es de solo lectura.
        /// </summary>
        public static readonly FileFilter AudioFiles = new FileFilter(St.AudioFiles, "wav", "mp3", "ogg", "aac", "aiff", "flac");

        /// <summary>
        /// Obtiene un filtro que permite seleccionar documentos en formatos
        /// comunes. Este campo es de solo lectura.
        /// </summary>
        public static readonly FileFilter Documents = new FileFilter(St.Documents, "pdf", "docx", "doc", "odt", "rtf", "wpd");

        /// <summary>
        /// Obtiene un filtro que permite seleccionar todo tipo de documentos
        /// comunes de Microsoft Office. Este campo es de solo lectura.
        /// </summary>
        public static readonly FileFilter MsOfficeFiles = new FileFilter(St.MsOfficeFiles, "docx", "xlsx", "accdb", "pptx", "pubx");

        /// <summary>
        /// Obtiene un filtro que permite seleccionar archivos de base de datos
        /// comunes. Este campo es de solo lectura.
        /// </summary>
        public static readonly FileFilter DatabaseFiles = new FileFilter(St.DatabaseFiles, "accdb", "mdf");

        /// <summary>
        /// Obtiene un filtro que permite seleccionar archivos de video en
        /// formatos comunes. Este campo es de solo lectura.
        /// </summary>
        public static readonly FileFilter VideoFiles = new FileFilter(St.VideoFiles, "mpg", "mpeg", "mp4", "ogv", "3gp", "avi", "mkv", "flv");

        /// <summary>
        /// Obtiene una descripción amigable para este
        /// <see cref="FileFilter"/>.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Obtiene una colección de extensiones con la cual este filtro está 
        /// asociado.
        /// </summary>
        public string[] Extensions { get; set; }

        /// <summary>
        /// Inicializa una nueva instancia de la estructura 
        /// <see cref="FileFilter"/> especificando la descripción y extensiones
        /// a utilizar.
        /// </summary>
        /// <param name="description">
        /// Descripción amigable para este filtro.
        /// </param>
        /// <param name="extensions">
        /// Extensiones a asociar a este filtro.
        /// </param>
        /// <exception cref="EmptyCollectionException">
        /// Se produce si <paramref name="extensions"/> no contiene elementos.
        /// </exception>
        public FileFilter(string? description, params string[] extensions)
        {
            if (!extensions.Any()) throw new EmptyCollectionException(extensions);
            Description = description ?? string.Format(St.XFile, extensions[0].ChopStart("*."));
            Extensions = ChkExtension(extensions?.NotNull().OrNull()) ?? throw new ArgumentNullException(nameof(extensions));
        }

        /// <summary>
        /// Convierte implícitamente una tupla de <c>(<see cref="string"/>,
        /// <see cref="string"/>[])</c> en un <see cref="FileFilter"/>.
        /// </summary>
        /// <param name="tuple">Tupla a convertir.</param>
        public static implicit operator FileFilter((string, string[]) tuple) => new FileFilter(tuple.Item1, tuple.Item2);

        /// <summary>
        /// Convierte implícitamente un <see cref="FileFilter"/> en una tupla
        /// de <c>(<see cref="string"/>, <see cref="string"/>[])</c>.
        /// </summary>
        /// <param name="filter"><see cref="FileFilter"/> a convertir.</param>
        public static implicit operator (string, string[])(FileFilter filter) => (filter.Description, filter.Extensions);

        /// <summary>
        /// Obtiene un valor que indica si ambos objetos son considerados
        /// iguales.
        /// </summary>
        /// <param name="obj">Objeto contra el cual comparar.</param>
        /// <returns>
        /// <see langword="true"/> si este objeto es considerado igual al
        /// especificado, <see langword="false"/> en caso contrario.
        /// </returns>
        public override bool Equals(object? obj)
        {
            return obj switch
            {
                FileFilter f => this == f,
                ValueTuple<string, string[]> t => Description == t.Item1 && Extensions.ItemsEqual(t.Item2),
                _ => false
            };
        }

        /// <summary>
        /// Obtiene un valor que indica si ambos objetos son considerados
        /// iguales.
        /// </summary>
        /// <param name="left">Primer objeto a comparar.</param>
        /// <param name="right">Segundo objeto a comparar.</param>
        /// <returns>
        /// <see langword="true"/> si ambos objetos son considerados iguales,
        /// <see langword="false"/> en caso contrario.
        /// </returns>
        public static bool operator ==(FileFilter left, FileFilter right)
        {
            return left.Extensions.ItemsEqual(right.Extensions);
        }

        /// <summary>
        /// Obtiene un valor que indica si ambos objetos son considerados
        /// distintos.
        /// </summary>
        /// <param name="left">Primer objeto a comparar.</param>
        /// <param name="right">Segundo objeto a comparar.</param>
        /// <returns>
        /// <see langword="true"/> si ambos objetos son considerados distintos,
        /// <see langword="false"/> en caso contrario.
        /// </returns>
        public static bool operator !=(FileFilter left, FileFilter right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Obtiene el código Hash de esta instancia.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Description, Extensions);
        }

        /// <summary>
        /// Obtiene un enumerador que permite iterar sobre las extensiones de
        /// archivo de este filtro.
        /// </summary>
        /// <returns>
        /// Un enumerador que permite iterar sobre las extensiones de archivo
        /// de este filtro.
        /// </returns>
        public IEnumerator<string> GetEnumerator()
        {
            return ((IEnumerable<string>)Extensions).GetEnumerator();
        }

        /// <summary>
        /// Obtiene un enumerador que permite iterar sobre las extensiones de
        /// archivo de este filtro.
        /// </summary>
        /// <returns>
        /// Un enumerador que permite iterar sobre las extensiones de archivo
        /// de este filtro.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<string>)Extensions).GetEnumerator();
        }

        private static string[]? ChkExtension(IEnumerable<string>? extCollection)
        {
            return extCollection?.Select(p => p.StartsWith("*.") ? p : $"*.{p}").ToArray();
        }
    }
}
