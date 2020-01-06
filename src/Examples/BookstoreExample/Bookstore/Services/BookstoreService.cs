using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Examples.BookstoreExample.Models;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Examples.BookstoreExample.Services
{
    public class BookstoreService : Service<BookstoreContext>
    {
        public Task<List<Book>> GetBestBooksAsync()
        {
            var t = GetReadTransaction();
            try
            {
                return t.All<Book>()
                    .Include(p => p.Category)
                    .OrderByDescending(p => p.Rating)
                    .Take(10).ToListAsync();
            }
            finally
            {
                if (!t.IsDisposed) t.Dispose();
            }
        }

        public async Task<IDictionary<string, int>> CrunchTagsAsync()
        {
            var t = GetReadTransaction();
            try
            {
                var tags = 
                    (await t.All<Book>().Select(p => p.Tags).ToListAsync())
                    .NotNull()
                    .SelectMany(p => p.Split(';'))
                    .ToList();

                t.Dispose();

                var d = new AutoDictionary<string, int>();
                foreach (var j in tags) d[j]++;
                return d;
            }
            finally
            {
                if (!t.IsDisposed) t.Dispose();
            }
        }
    }
}