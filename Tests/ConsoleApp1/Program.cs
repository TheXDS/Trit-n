using System;
using System.Collections.Generic;
using System.Linq;
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
            var x = ViewModelBuilder<User, Guid>.New();
            Console.WriteLine("Hello World!");
        }

        public class User : ModelBase<Guid>
        {
            public string Name { get; set; } = "Test";
        }
    }
}
