using BookMS.Application.Abstractions;
using BookMS.Application.Auth.Dtos;
using BookMS.Application.Common.Exceptions;
using BookMS.Application.Services;
using BookMS.Domain.Entities;
using BookMS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookMS.Application.Auth.Commands.Register
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponseDto>
    {
        private readonly IAppDbContext _db;
        private readonly ITokenService _tokens;
        private readonly IPasswordHasher<Users> _hasher;

        public RegisterUserCommandHandler(
            IAppDbContext db,
            ITokenService tokens,
            IPasswordHasher<Users> hasher)
        {
            _db = db;
            _tokens = tokens;
            _hasher = hasher;
        }

        public async Task<AuthResponseDto> Handle(RegisterUserCommand cmd, CancellationToken ct)
        {
            var r = cmd.Request;

            if (await _db.Users.AnyAsync(u => u.Email == r.Email, ct))
                throw new ConflictException($"Email '{r.Email}' is already registered.");

            // find role by name
            var role = await _db.Roles.FirstOrDefaultAsync(x => x.Name == r.Role, ct);
            if (role is null)
                throw new NotFoundException("Role", r.Role);

            var entity = new Users
            {
                Email = r.Email,
                Name = r.Name,
                Provider = AuthProvider.Microsoft, // default if not SSO
                ProviderUserId = r.Email,
                RoleId = role.Id
            };
            entity.PasswordHash = _hasher.HashPassword(entity, r.Password);

            await _db.Users.AddAsync(entity, ct);
            await _db.SaveChangesAsync(ct);

            // refresh token
            var (refreshToken, refreshExpiry) = _tokens.CreateRefreshToken();
            await _db.RefreshTokens.AddAsync(new RefreshToken
            {
                UserId = entity.Id,
                Token = refreshToken,
                ExpiresAt = refreshExpiry
            }, ct);
            await _db.SaveChangesAsync(ct);

            var token = _tokens.CreateAccessToken(entity.Id, entity.Email, entity.Name, role.Name);
            return new AuthResponseDto(entity.Id, entity.Email, entity.Name, role.Name, token, refreshToken);
        }
    }
}
