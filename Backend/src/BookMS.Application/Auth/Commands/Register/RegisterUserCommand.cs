using BookMS.Application.Auth.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Application.Auth.Commands.Register
{
    public record RegisterUserCommand(RegisterRequest Request) : IRequest<AuthResponseDto>;
}
