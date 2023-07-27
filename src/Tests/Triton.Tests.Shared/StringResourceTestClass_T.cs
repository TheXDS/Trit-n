namespace TheXDS.Triton.Tests;

public abstract class StringResourceTestClass<T> : StringResourceTestClass where T : notnull
{
    protected StringResourceTestClass() : base(typeof(T))
    {
    }
}
