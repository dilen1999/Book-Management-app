using BookMS.Application.Abstractions;
using BookMS.Application.Auth.Dtos;
using BookMS.Application.Common.Exceptions;
using BookMS.Application.Services;
using BookMS.Domain.Entities;
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

            var role = await _db.Roles.FirstOrDefaultAsync(x => x.Id == r.RoleId, ct)
                       ?? throw new NotFoundException("Role", r.RoleId);

            var entity = new Users
            {
                Email = r.Email,
                Name = r.Name,
                Provider = Domain.Enums.AuthProvider.Microsoft,
                ProviderUserId = r.Email,
                RoleId = r.RoleId
            };
            entity.PasswordHash = _hasher.HashPassword(entity, r.Password);

            await _db.Users.AddAsync(entity, ct);
            await _db.SaveChangesAsync(ct); // ensure entity.Id exists for FK

            // --- refresh token: create + persist ---
            var (refreshToken, expires) = _tokens.CreateRefreshToken();
            await _db.RefreshTokens.AddAsync(new RefreshToken
            {
                UserId = entity.Id,
                Token = refreshToken,
                ExpiresAt = expires,
                CreatedByIp = null,
                UserAgent = null
            }, ct);
            await _db.SaveChangesAsync(ct);

            // --- access token ---
            var access = _tokens.CreateAccessToken(entity.Id, entity.Email, entity.Name, role.Name);

            return new AuthResponseDto(entity.Id, entity.Email, entity.Name, role.Name, access, refreshToken);
        }
    }
}
