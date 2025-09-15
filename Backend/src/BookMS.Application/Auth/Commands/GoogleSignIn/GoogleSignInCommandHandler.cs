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
    // Helper class for Google user info deserialization
    public class GoogleUserInfo
    {
        public string? id { get; set; }
        public string? email { get; set; }
        public string? name { get; set; }
        public string? picture { get; set; }
    }

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
                throw new ConflictException("Missing Google access token.");

            // 1) Get user info from Google using access token
            string email, name, googleUserId;
            try
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync($"https://www.googleapis.com/oauth2/v2/userinfo?access_token={cmd.Request.IdToken}");
                
                if (!response.IsSuccessStatusCode)
                {
                    throw new ConflictException("Invalid Google access token.");
                }
                
                var userInfoJson = await response.Content.ReadAsStringAsync();
                var userInfo = System.Text.Json.JsonSerializer.Deserialize<GoogleUserInfo>(userInfoJson);
                
                email = userInfo?.email ?? string.Empty;
                name = userInfo?.name ?? userInfo?.email ?? "Google User";
                googleUserId = userInfo?.id ?? string.Empty;
            }
            catch (Exception ex) when (!(ex is ConflictException))
            {
                _log.LogWarning(ex, "Failed to get user info from Google");
                throw new ConflictException("Invalid Google token.");
            }

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
