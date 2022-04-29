#pragma warning disable CS1591

namespace TheXDS.Triton.Tests.SecurityEssentials;
using TheXDS.Triton.Services;
using Services;

internal class TestUserService : Service, IUserService
{
    public TestUserService() : base(new TestTransFactory())
    {
    }
}