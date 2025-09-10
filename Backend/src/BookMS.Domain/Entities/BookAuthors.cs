using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Domain.Entities
{
    public class BookAuthors
    {
        public Guid BookId { get; set; }
        public Books Books { get; set; } = default!;

        public Guid AuthorId { get; set; }
        public Authors Authors { get; set; } = default!;
    }
}
