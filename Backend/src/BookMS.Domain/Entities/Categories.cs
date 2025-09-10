using BookMS.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Domain.Entities
{
    public class Categories : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }

        public ICollection<BookCategories> Books { get; set; } = new List<BookCategories>();
    }
}
