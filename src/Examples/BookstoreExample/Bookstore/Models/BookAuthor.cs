using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Examples.BookstoreExample.Models
{
    public class BookAuthor : Model<int>
    {
        public Book BookId { get; set; }
        public Author AuthorId { get; set; }
    }
}
