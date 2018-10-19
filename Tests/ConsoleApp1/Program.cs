using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using TheXDS.Triton.Core.Models.Base;
using TheXDS.Triton.ViewModel;

namespace ConsoleApp1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var a = ViewModelBuilder<User, Guid>.New<UserViewModel>();

            
            Console.WriteLine("Hello World!");
            
        }


        public class User : ModelBase<Guid>
        {
            public string Name { get; set; } = "Test";

            public int Number { get; set; } = 2;
        }

        public class UserViewModel : ViewModelBase<User, Guid>
        {
            public bool IsRegistered => true;

            public int Number
            {
                get => Entity.Number;
                set
                {
                    Entity.Number = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
