using System;
using TheXDS.MCART.Attributes;

namespace TheXDS.Triton.Annotations
{
    /// <inheritdoc />
    /// <summary>
    ///     Atributo que anota un valor que describe la categoría a la que un
    ///     método pertenece, permitiendo o denegando la ejecución del mismo.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MethodCategoryAttribute : Attribute, IValueAttribute<MethodCategory>
    {
        /// <inheritdoc />
        /// <summary>
        ///     Marca un método con el atributo de categoría de seguridad
        ///     especificado.
        /// </summary>
        /// <param name="value">
        ///     Categoría de seguridad a la que el método pertenece.
        /// </param>
        public MethodCategoryAttribute(MethodCategory value)
        {
            Value = value;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Obtiene el valor de este atributo.
        /// </summary>
        public MethodCategory Value { get; }
    }
}