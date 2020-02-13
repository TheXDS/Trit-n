#pragma warning disable CS1591

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;


namespace TheXDS.Triton.Models
{
    public class BlogContext : DbContext
    {
        private static readonly ILoggerFactory _m = LoggerFactory.Create(b => b.AddConsole().AddDebug());
        public virtual DbSet<Post> Posts { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Comment> Comments { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseInMemoryDatabase(GetType().FullName)
                .UseLoggerFactory(_m);                
        }


        /// <summary>
        /// Inicializa la clase <see cref="BlogContext"/>.
        /// </summary>
        static BlogContext()
        {
            using var c = new BlogContext();
            User u1, u2, u3;

            c.Users.AddRange(new[]
            {
                u1 = new User()
                {
                    Id = "user1",
                    PublicName = "User #1",
                    Joined = new DateTime(2001, 1, 1)
                },
                u2 = new User()
                {
                    Id = "user2",
                    PublicName = "User #2",
                    Joined = new DateTime(2009, 3, 4)
                },
                u3 = new User()
                {
                    Id = "user3",
                    PublicName = "User #3",
                    Joined = new DateTime(2004, 9, 11)
                }
            });

            c.Posts.Add(new Post()
            {
                Title = "Test",
                CreationTime = new DateTime(2016, 12, 31),
                Published = true,
                Content = "This is a test.",
                Author = u1,
                Comments =
                {
                    new Comment()
                    {
                        Author = u2,
                        Timestamp = new DateTime(2017,1,1),
                        Content = "It works!"
                    },
                    new Comment()
                    {
                        Author = u3,
                        Timestamp = new DateTime(2017,1,2),
                        Content = "Yay! c:"
                    },
                    new Comment()
                    {
                        Author = u1,
                        Timestamp = new DateTime(2017,1,1),
                        Content = "Shuddap >:("
                    },
                    new Comment()
                    {
                        Author = u3,
                        Timestamp = new DateTime(2017,1,1),
                        Content = "ok :c"
                    },

                }
            });

            c.SaveChanges();

        }
    }
}