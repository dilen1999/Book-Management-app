// src/BookMS.Application/Auth/Commands/Refresh/RefreshTokenCommandHandler.cs
using BookMS.Application.Abstractions;
using BookMS.Application.Auth.Dtos;
using BookMS.Application.Common.Exceptions;
using BookMS.Application.Services;
using BookMS.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookMS.Application.Auth.Commands.Refresh
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
    {
        private readonly IAppDbContext _db;
        private readonly ITokenService _tokens;
        private readonly ILogger<RefreshTokenCommandHandler> _log;

        public RefreshTokenCommandHandler(IAppDbContext db, ITokenService tokens, ILogger<RefreshTokenCommandHandler> log)
        {
            _db = db; _tokens = tokens; _log = log;
        }

        public async Task<AuthResponseDto> Handle(RefreshTokenCommand cmd, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(cmd.RefreshToken))
                throw new ConflictException("Invalid or expired refresh token.");

            // 1) Find the refresh token in DB
            var existing = await _db.RefreshTokens
                .AsNoTracking() // read-only
                .FirstOrDefaultAsync(t => t.Token == cmd.RefreshToken, ct);

            if (existing is null)
            {
                _log.LogInformation("Refresh failed: token not found");
                throw new ConflictException("Invalid or expired refresh token.");
            }

            if (!existing.IsActive)
            {
                _log.LogInformation("Refresh failed: token inactive (revoked/expired)");
                throw new ConflictException("Invalid or expired refresh token.");
            }

            // 2) Load user + role
            var user = await _db.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == existing.UserId, ct);

            if (user is null)
            {
                _log.LogWarning("Refresh failed: user {UserId} missing", existing.UserId);
                throw new ConflictException("Invalid or expired refresh token.");
            }

            if (user.Roles is null)
            {
                _log.LogWarning("Refresh failed: user {UserId} has no role", existing.UserId);
                throw new ConflictException("Invalid or expired refresh token.");
            }

            // 3) Rotate token: revoke old and create new
            var (newToken, newExpiry) = _tokens.CreateRefreshToken();

            // track the old entity to update its revoked state
            var oldTracked = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == existing.Token, ct);
            if (oldTracked is not null)
            {
                oldTracked.RevokedAt = DateTime.UtcNow;
                oldTracked.ReplacedByToken = newToken;
            }

            await _db.RefreshTokens.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                Token = newToken,
                ExpiresAt = newExpiry
            }, ct);

            await _db.SaveChangesAsync(ct);

            // 4) Issue new access token
            var access = _tokens.CreateAccessToken(user.Id, user.Email, user.Name, user.Roles.Name);

            return new AuthResponseDto(user.Id, user.Email, user.Name, user.Roles.Name, access, newToken);
        }
    }
}
