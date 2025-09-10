using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Domain.Entities
{
    public class BookCategories
    {
        public Guid BookId { get; set; }
        public Books Books { get; set; } = default!;

        public Guid CategoryId { get; set; }
        public Categories Categories { get; set; } = default!;
    }
}
