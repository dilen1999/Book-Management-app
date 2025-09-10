using BookMS.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Domain.Entities
{
    public  class Roles : BaseEntity
    {
        public string Name { get; set; } = default!;
        public ICollection<Users> Users { get; set; } = new List<Users>();
    }
}
