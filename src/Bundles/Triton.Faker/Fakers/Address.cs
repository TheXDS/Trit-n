using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Triton.Fakers
{
    /// <summary>
    /// Objeto que describe una ubicación física completa.
    /// </summary>
    public record Address(string AddressLine, string? AddressLine2, string City, string Country, ushort Zip)
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            return $@"{string.Join(System.Environment.NewLine, new[] { AddressLine, AddressLine2, $"{City}, {Country} {Zip}" }.NotNull())}";
        }
    }
}
