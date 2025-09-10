using BookMS.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Domain.Entities
{
    public class Books : BaseEntity
    {
        public string Title { get; set; } = default!;
        public string AuthorName { get; set; } = default!;
        public string? Isbn { get; set; }
        public int? PublishedYear { get; set; }
        public string? Category { get; set; }

        // Relationships
        public ICollection<BookAuthors> Authors { get; set; } = new List<BookAuthors>();
        public ICollection<BookCategories> Categories { get; set; } = new List<BookCategories>();
    }
}
