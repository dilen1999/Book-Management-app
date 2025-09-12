using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Application.Auth.Commands.Revoke
{
    public record RevokeRefreshTokenCommand(string RefreshToken) : IRequest<Unit>;
}
