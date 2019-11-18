using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Extensions.Logging.Console;


#nullable enable

namespace TheXDS.Triton.TestModels
{
    internal class BlogContext : DbContext
    {
        private static readonly ILoggerFactory _m = LoggerFactory.Create(b => b.AddConsole());
        public virtual DbSet<Post> Posts { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Comment> Comments { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseInMemoryDatabase(GetType().FullName)
                .UseLoggerFactory(_m);                
        }
    }
}