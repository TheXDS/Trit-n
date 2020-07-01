using TheXDS.MCART.Types.Base;
using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Examples.BookstoreExample.Models
{
    public class Category : Model<int>, INameable
    {
        public string Name { get; set; }
    }
}
