using System;
using NUnit.Framework;
using TheXDS.Triton.Core.Models.Base;

namespace TritonTests.Models
{
    [TestFixture]
    public class IntegrationTests
    {
        private class User : ModelBase<Guid>, INameable
        {
            public string Username { get; set; }
            public string Name { get; set; }
        }

        private class UserViewModel : ViewModelBase<User, Guid>
        {
            
        }
        
    }
}