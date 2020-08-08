using Microsoft.EntityFrameworkCore;

namespace TheXDS.Triton.Examples.BookstoreExample.Models
{
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
