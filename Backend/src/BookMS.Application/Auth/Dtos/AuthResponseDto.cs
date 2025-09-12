using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Application.Auth.Dtos
{
    public record AuthResponseDto(Guid UserId, string Email, string Name, string Role, string AccessToken, string RefreshToken);
}
