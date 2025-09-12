using BookMS.Application.Auth.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Application.Auth.Commands.Refresh
{
    public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponseDto>;
}
