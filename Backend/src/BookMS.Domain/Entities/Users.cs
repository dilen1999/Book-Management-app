using BookMS.Domain.Common;
using BookMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Domain.Entities
{
    public class Users : BaseEntity
    {
        public string Email { get; set; } = default!;
        public string Name { get; set; } = default!;
        public AuthProvider Provider { get; set; } = default!; 
        public string ProviderUserId { get; set; } = default!;
        public DateTime? LastLoginAt { get; set; }

        //Relationships
        public Guid RoleId { get; set; }
        public Roles Roles { get; set; } = default!;
    }
}
