using BookMS.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Application.Auth.Commands.Revoke
{
    public class RevokeRefreshTokenHandler : IRequestHandler<RevokeRefreshTokenCommand, Unit>
    {
        private readonly IAppDbContext _db;
        public RevokeRefreshTokenHandler(IAppDbContext db) => _db = db;

        public async Task<Unit> Handle(RevokeRefreshTokenCommand cmd, CancellationToken ct)
        {
            var token = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == cmd.RefreshToken, ct);
            if (token is null) return Unit.Value;
            if (token.RevokedAt is null) token.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
