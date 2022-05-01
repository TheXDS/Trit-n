#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.Services;

public class ServiceTests
{
    [Test]
    public void Service_discovers_ITransactionFactory_Test()
    {
        var s = new Service();
        Assert.IsNotNull(s.Factory);

        s = new(new TransactionConfiguration());
        Assert.IsNotNull(s.Factory);
    }
}