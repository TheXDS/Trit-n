using TheXDS.Triton.Services;
using TheXDS.Triton.Tests.Services;

namespace TheXDS.Triton.Tests.SecurityEssentials;

internal class TestUserService : TritonService, IUserService
{
    public TestUserService() : base(new TestTransFactory())
    {
    }
}