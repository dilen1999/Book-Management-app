using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Application.Auth.Dtos
{
    public record RegisterRequest(string Email, string Name, string Password, Guid RoleId);
}
