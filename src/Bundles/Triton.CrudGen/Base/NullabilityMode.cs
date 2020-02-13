namespace TheXDS.Triton.CrudGen.Base
{
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
}
