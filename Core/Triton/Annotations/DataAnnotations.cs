using System;

namespace TheXDS.Triton.Annotations
{
    /// <summary>
    ///     Establece un nombre alternativo para una propiedad de la clase base
    ///     generada por <see cref="GenericScaffold"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public sealed class AltNameAttribute : Attribute
    {
        /// <inheritdoc />
        /// <summary>
        /// Establece un nombre alternativo para una propiedad generada por
        /// <see cref="GenericScaffold" />.
        /// </summary>
        /// <param name="element">Nombre del elemento.</param>
        /// <param name="newName">Nuevo nombre a aplicar.</param>
        public AltNameAttribute(string element, string newName) : this(element, newName, null)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Establece un nombre alternativo para una propiedad generada por
        /// <see cref="GenericScaffold" />.
        /// </summary>
        /// <param name="element">Nombre del elemento.</param>
        /// <param name="newName">Nuevo nombre a aplicar.</param>
        /// <param name="mask">Máscara de texto opcional a utilizar para el elemento.</param>
        public AltNameAttribute(string element, string newName, string mask)
        {
            Element = element;
            NewName = newName;
            Mask = mask;
        }

        /// <summary>
        /// Nombre del elemento al cual reemplazar el nombre.
        /// </summary>
        public string Element { get; }
        /// <summary>
        /// Nuevo nombre del elemento.
        /// </summary>
        public string NewName { get; }
        /// <summary>
        /// Máscara de texto opcional a utilizar.
        /// </summary>
        /// <remarks>No todos los controles soportan máscara opcional.</remarks>
        public string Mask { get; }
    }
    
    
    
}
