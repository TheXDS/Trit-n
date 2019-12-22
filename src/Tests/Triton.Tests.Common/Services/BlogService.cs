#pragma warning disable CS1591

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TheXDS.Triton.Services;
using TheXDS.Triton.Models;

namespace TheXDS.Triton.Tests
{
    public class BlogService : Service<BlogContext>
    {
        public IEnumerable<IGrouping<User, Post>> GetAllUsersFirst3Posts()
        {
            var t = GetReadTransaction();
            try
            {
                return t.All<User>()
                    .Include(p => p.Posts)
                    .ThenInclude(p => p.Author)
                    .SelectMany(p => p.Posts.Take(3).OrderBy(q => q.CreationTime))
                    .ToList()
                    .GroupBy(p => p.Author);
            }
            finally
            {
                t.Dispose();
            }
        }
    }
}