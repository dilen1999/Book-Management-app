using BookMS.Application.Abstractions;
using BookMS.Application.Auth.Dtos;
using BookMS.Application.Common.Exceptions;
using BookMS.Application.Services;
using BookMS.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookMS.Application.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly IAppDbContext _db;
        private readonly ITokenService _tokens;
        private readonly IPasswordHasher<Users> _hasher;

        public LoginCommandHandler(
            IAppDbContext db,
            ITokenService tokens,
            IPasswordHasher<Users> hasher)
        {
            _db = db;
            _tokens = tokens;
            _hasher = hasher;
        }

        public async Task<AuthResponseDto> Handle(LoginCommand cmd, CancellationToken ct)
        {
            var r = cmd.Request;

            var user = await _db.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == r.Email, ct);

            if (user is null || string.IsNullOrWhiteSpace(user.PasswordHash))
                throw new NotFoundException("User", r.Email);

            var verify = _hasher.VerifyHashedPassword(user, user.PasswordHash!, r.Password);
            if (verify == PasswordVerificationResult.Failed)
                throw new ConflictException("Invalid email or password.");

            user.LastLoginAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);

            // --- refresh token: rotate ---
            var (newToken, newExpiry) = _tokens.CreateRefreshToken();
            await _db.RefreshTokens.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                Token = newToken,
                ExpiresAt = newExpiry,
                CreatedByIp = null,
                UserAgent = null
            }, ct);
            await _db.SaveChangesAsync(ct);

            // --- access token ---
            var access = _tokens.CreateAccessToken(user.Id, user.Email, user.Name, user.Roles.Name);

            return new AuthResponseDto(user.Id, user.Email, user.Name, user.Roles.Name, access, newToken);
        }
    }
}
