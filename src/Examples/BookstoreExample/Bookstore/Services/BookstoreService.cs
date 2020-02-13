using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Examples.BookstoreExample.Models;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Examples.BookstoreExample.Services
{
    public class BookstoreService : Service<BookstoreContext>
    {
        public Task<List<Book>> GetBestBooksAsync()
        {
            return WithReadTransaction(t => t.All<Book>()
                .Include(p => p.Category)
                .OrderByDescending(p => p.Rating)
                .Take(10).ToListAsync());
        }

        public Task<IDictionary<string, int>> CrunchTagsAsync()
        {
            return WithReadTransaction(CrunchTagsAsync);
        }

        private async Task<IDictionary<string, int>> CrunchTagsAsync(ICrudReadTransaction t)
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
    }
}