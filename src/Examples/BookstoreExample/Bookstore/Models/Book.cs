using Microsoft.EntityFrameworkCore;
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

    public class BookAuthor : Model<int>
    {
        public Book BookId { get; set; }
        public Author AuthorId { get; set; }
    }

    public class Author : Model<int>, INameable
    {
        public string Name { get; set; }
        public string? Bio { get; set; }
        public string? Picture { get; set; }
        public ICollection<BookAuthor> Books { get; set; } = new List<BookAuthor>();
    }

    public class Category : Model<int>, INameable
    {
        public string Name { get; set; }
    }

    public class BookstoreContext : DbContext
    {
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<BookAuthor> BookAuthors { get; set; }
        public virtual DbSet<Author> Authors { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("(LocalDB)\\MSSQLLocalDB");
        }
    }
}
