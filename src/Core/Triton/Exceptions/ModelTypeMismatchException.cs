using TheXDS.Triton.Resources.Strings;

namespace TheXDS.Triton.Exceptions
{
    /// <summary>
    /// Exception that is thrown when the model of two entities does not match
    /// in functions that require it.
    /// </summary>
    public class ModelTypeMismatchException() : Exception(Common.ModelMismatch)
    {
    }
}
