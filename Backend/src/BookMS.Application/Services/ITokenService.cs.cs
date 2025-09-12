using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Application.Services
{
    public interface ITokenService
    {
        string CreateAccessToken(Guid userId, string email, string name, string role);
        (string token, DateTime expiresAt) CreateRefreshToken();
    }
}
