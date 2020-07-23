using System.Collections.Generic;
using TheXDS.MCART.Types.Base;
using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Examples.BookstoreExample.Models
{
    public class Book : Model<string>, INameable
    {
        public string Name { get; set; }
        public ICollection<BookAuthor> Authors { get; set; } = new List<BookAuthor>();
        public int Edition { get; set; }
        public short ReleaseYear { get; set; }
        public byte Rating { get; set; }
        public Category? Category { get; set; }
        public int Existance { get; set; }
        public string? Tags { get; set; }
        public string ShortDescription { get; set; }
    }
}
