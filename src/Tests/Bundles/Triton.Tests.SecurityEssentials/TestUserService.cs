#pragma warning disable CS1591

using TheXDS.Triton.InMemory.Services;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests
{
    internal class TestUserService : Service, IUserService
    {
        public TestUserService() : base(new TestTransFactory())
        {
        }
    }
}