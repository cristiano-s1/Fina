using Fina.Api.Data;
using Fina.Core.Common;
using Fina.Core.Enums;
using Fina.Core.Handlers;
using Fina.Core.Models;
using Fina.Core.Requests.Transactions;
using Fina.Core.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Fina.Api.Handlers
{
    public class TransactionHandler(AppDbContext context) : ITransactionHandler
    {
        public async Task<Response<Transaction?>> CreateAsync(CreateTransactionRequest request)
        {
            if (request is { Type: Core.Enums.ETransactionType.Withdraw, Amount: > 0 })
            {
                request.Amount *= -1;
            }

            var transaction = new Transaction
            {
                UserId = request.UserId,
                CategoryId = request.CategoryId,
                CreatedAt = DateTime.UtcNow,
                Amount = request.Amount,
                PaidOrReceiveAt = request.PaidOrReceiveAt,
                Title = request.Title,
                Type = request.Type
            };

            try
            {
                await context.Transactions.AddAsync(transaction);
                await context.SaveChangesAsync();

                return new Response<Transaction?>(transaction, 201, "Transacao criada com sucesso");
            }
            catch
            {
                return new Response<Transaction?>(null, 500, "Nao foi possivel criar sua transacao");
            }
        }

        public async Task<Response<Transaction?>> UpdateAsync(UpdateTransactionRequest request)
        {
            if (request is { Type: Core.Enums.ETransactionType.Withdraw, Amount: > 0 })
            {
                request.Amount *= -1;
            }

            try
            {
                var transaction = await context.Transactions.FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

                if (transaction is null)
                    return new Response<Transaction?>(null, 404, "Transacao nao encontrada");

                transaction.UserId = request.UserId;
                transaction.CategoryId = request.CategoryId;
                transaction.CreatedAt = DateTime.UtcNow;
                transaction.Amount = request.Amount;
                transaction.PaidOrReceiveAt = request.PaidOrReceiveAt;
                transaction.Title = request.Title;
                transaction.Type = request.Type;

                context.Transactions.Update(transaction);
                await context.SaveChangesAsync();

                return new Response<Transaction?>(transaction, 201, "Transacao atualizada com sucesso");
            }
            catch
            {
                return new Response<Transaction?>(null, 500, "Nao foi possivel atualizar transacao");
            }
        }

        public async Task<Response<Transaction?>> DeleteAsync(DeleteTransactionRequest request)
        {
            try
            {
                var transaction = await context.Transactions.FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

                if (transaction is null)
                    return new Response<Transaction?>(null, 404, "Transacao nao encontrada");

                context.Transactions.Remove(transaction);
                await context.SaveChangesAsync();

                return new Response<Transaction?>(transaction, 201, "Transacao excluida com sucesso");
            }
            catch
            {
                return new Response<Transaction?>(null, 500, "Nao foi possivel excluir transacao");
            }
        }

        public async Task<Response<Transaction?>> GetByIdAsync(GetTransactionByIdRequest request)
        {
            try
            {
                var transaction = await context.Transactions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

                return transaction is null
                    ? new Response<Transaction?>(null, 404, "Transacao nao encontrada")
                    : new Response<Transaction?>(transaction);
            }
            catch
            {
                return new Response<Transaction?>(null, 500, "Nao foi possivel encontrar transacao");
            }
        }

        public async Task<PagedResponse<List<Transaction>?>> GetAllAsync(GetTransactionsByPeriodRequest request)
        {
            try
            {
                request.StartDate ??= DateTime.UtcNow.GetFirstDay();
                request.EndDate ??= DateTime.UtcNow.GetLastDay();
            }
            catch
            {
                return new PagedResponse<List<Transaction>?>(null, 500, "Nao foi possivel determinar a data de transacao");
            }

            try
            {
                var query = context
                    .Transactions
                    .AsNoTracking()
                    .Where(x => 
                        x.PaidOrReceiveAt > request.StartDate &&
                        x.PaidOrReceiveAt < request.EndDate &&
                        x.UserId == request.UserId)
                    .OrderBy(x => x.PaidOrReceiveAt);

                var transactions = await query
                    .Skip((request.PageNumber - 1) * request.PageSize) 
                    .Take(request.PageSize)
                    .ToListAsync();

                var count = await query.CountAsync();

                return new PagedResponse<List<Transaction>?>(transactions, count, request.PageNumber, request.PageSize);
            }
            catch
            {
                return new PagedResponse<List<Transaction>?>(null, 0, request.PageNumber, request.PageSize);
            }
        }
    }
}
