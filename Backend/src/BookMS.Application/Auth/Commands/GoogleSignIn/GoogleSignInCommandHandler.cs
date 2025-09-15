using BookMS.Application.Abstractions;
using BookMS.Application.Auth.Dtos;
using BookMS.Application.Common.Exceptions;
using BookMS.Application.Services;
using BookMS.Domain.Entities;
using BookMS.Domain.Enums;
using Google.Apis.Auth;
using BookMS.Application.Common.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Application.Auth.Commands.GoogleSignIn
{
    public class GoogleSignInCommandHandler : IRequestHandler<GoogleSignInCommand, AuthResponseDto>
    {
        private readonly IAppDbContext _db;
        private readonly ITokenService _tokens;
        private readonly GoogleAuthOptions _opts;
        private readonly ILogger<GoogleSignInCommandHandler> _log;

        public GoogleSignInCommandHandler(
            IAppDbContext db,
            ITokenService tokens,
            IOptions<GoogleAuthOptions> opts,
            ILogger<GoogleSignInCommandHandler> log)
        {
            _db = db;
            _tokens = tokens;
            _opts = opts.Value;
            _log = log;
        }

        public async Task<AuthResponseDto> Handle(GoogleSignInCommand cmd, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(cmd.Request.IdToken))
                throw new ConflictException("Missing Google id_token.");

            // 1) Validate the Google ID token (signature + audience)
            GoogleJsonWebSignature.Payload payload;
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = _opts.ClientIds?.Length > 0 ? _opts.ClientIds : null
                };
                payload = await GoogleJsonWebSignature.ValidateAsync(cmd.Request.IdToken, settings);
            }
            catch (InvalidJwtException ex)
            {
                _log.LogWarning(ex, "Invalid Google ID token");
                throw new ConflictException("Invalid Google token.");
            }

            // payload fields we care about
            var googleUserId = payload.Subject;            // "sub"
            var email = payload.Email ?? string.Empty;
            var name = payload.Name ?? payload.Email ?? "Google User";

            if (string.IsNullOrWhiteSpace(email))
                throw new ConflictException("Google token does not contain an email.");

            // 2) Find existing user by ProviderUserId or Email
            var user = await _db.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Provider == AuthProvider.Google && u.ProviderUserId == googleUserId, ct);

            if (user is null)
            {
                // if email exists but with different provider, you can decide how to handle
                var emailOwner = await _db.Users.Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.Email == email, ct);

                if (emailOwner is not null && emailOwner.Provider != AuthProvider.Google)
                    throw new ConflictException("This email is already registered with another provider.");

                // choose a default role for new Google users (e.g., "User")
                var role = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "User", cancellationToken: ct)
                           ?? await _db.Roles.FirstOrDefaultAsync(ct)
                           ?? throw new NotFoundException("Default role", "User");

                user = new Users
                {
                    Email = email,
                    Name = name,
                    Provider = AuthProvider.Google,
                    ProviderUserId = googleUserId,
                    RoleId = role.Id,
                    LastLoginAt = DateTime.UtcNow
                };

                await _db.Users.AddAsync(user, ct);
                await _db.SaveChangesAsync(ct);

                // refresh token
                var (refreshTokenNew, refreshExpiry) = _tokens.CreateRefreshToken();
                await _db.RefreshTokens.AddAsync(new RefreshToken
                {
                    UserId = user.Id,
                    Token = refreshTokenNew,
                    ExpiresAt = refreshExpiry
                }, ct);
                await _db.SaveChangesAsync(ct);

                var accessNew = _tokens.CreateAccessToken(user.Id, user.Email, user.Name, role.Name);
                return new AuthResponseDto(user.Id, user.Email, user.Name, role.Name, accessNew, refreshTokenNew);
            }
            else
            {
                // existing Google user
                user.LastLoginAt = DateTime.UtcNow;
                await _db.SaveChangesAsync(ct);

                var (refreshToken, refreshExp) = _tokens.CreateRefreshToken();
                await _db.RefreshTokens.AddAsync(new RefreshToken
                {
                    UserId = user.Id,
                    Token = refreshToken,
                    ExpiresAt = refreshExp
                }, ct);
                await _db.SaveChangesAsync(ct);

                // ensure role is present
                if (user.Roles is null)
                {
                    user = await _db.Users.Include(u => u.Roles)
                             .FirstAsync(u => u.Id == user.Id, ct);
                }

                var access = _tokens.CreateAccessToken(user.Id, user.Email, user.Name, user.Roles.Name);
                return new AuthResponseDto(user.Id, user.Email, user.Name, user.Roles.Name, access, refreshToken);
            }
        }
    }
}
