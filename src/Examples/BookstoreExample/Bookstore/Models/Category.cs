using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Examples.BookstoreExample.Models
{
    public class Category : Model<int>
    {
        public string Name { get; set; }
    }
}
