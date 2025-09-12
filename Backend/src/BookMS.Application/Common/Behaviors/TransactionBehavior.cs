using BookMS.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Application.Common.Behaviors
{
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IAppDbContext _db;

        public TransactionBehavior(IAppDbContext db) => _db = db;

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct)
        {
            // If the provider doesn’t support transactions, just continue
            var dbCtx = _db as DbContext;
            if (dbCtx == null)
                return await next();

            // Only wrap commands: convention — names ending with "Command"
            var isCommand = typeof(TRequest).Name.EndsWith("Command", StringComparison.Ordinal);
            if (!isCommand)
                return await next();

            // If there is already a transaction, just continue
            if (dbCtx.Database.CurrentTransaction is not null)
                return await next();

            await using var tx = await dbCtx.Database.BeginTransactionAsync(ct);
            try
            {
                var response = await next();
                await tx.CommitAsync(ct);
                return response;
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }
    }
}
