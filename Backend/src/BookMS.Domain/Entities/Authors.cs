using BookMS.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Domain.Entities
{
    public class Authors : BaseEntity
    {
        public string FullName { get; set; } = default!;
        public string? Bio { get; set; }

        public ICollection<BookAuthors> Books { get; set; } = new List<BookAuthors>();
    }
}
