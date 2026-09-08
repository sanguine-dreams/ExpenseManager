using System.Linq.Expressions;
using ExpenseManager.Data.DB;
using ExpenseManager.Data.Models;
using ExpenseManager.Data.Requests;
using ExpenseManager.Data.Responses;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManager.Repository;

public class ExpenseRepository(DBContext context) : IExpenseRepository
{
    protected readonly DBContext _context = context;

    public async Task<RepositoryResponse<Expense>> CreateExpense(Expense entity)
    {
        try
        {
            await _context.Expenses.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return RepositoryResponse<Expense>.InternalError(
                $"Trouble creating expense: {ex.Message}"
            );
        }
        return RepositoryResponse<Expense>.Success(entity);
    }

    public async Task<RepositoryResponse> DeleteExpense(Guid id, string guildId)
    {
        var entity = await GetById(id, guildId);
        entity.Data.IsDeleted = true;
        entity.Data.DeletedAt = DateTime.UtcNow;

        try
        {
            _context.Expenses.Update(entity.Data);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return RepositoryResponse.InternalError($"Problem deleting expense: {ex.Message}");
        }

        return RepositoryResponse.Success();
    }

    public async Task<RepositoryResponse<PagedResult<Expense>>> GetAllExpenses(
        PaginatedRequest request,
        string guildId
    )
    {
        var totalCount = await _context.Expenses.Where(e => e.GuildId == guildId).CountAsync();

        var items = await _context
            .Expenses.Where(e => !e.IsDeleted && e.GuildId == guildId)
            .OrderBy(d => d.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var pagedResult = new PagedResult<Expense>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
        };
        return RepositoryResponse<PagedResult<Expense>>.Success(pagedResult);
    }

    public async Task<RepositoryResponse<Expense>> GetById(Guid id, string guildId)
    {
        var entity = await _context
            .Expenses.Where(e => !e.IsDeleted && e.GuildId == guildId)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (entity == null)
            return RepositoryResponse<Expense>.NotFound("No such expense found");

        return RepositoryResponse<Expense>.Success(entity);
    }

    public async Task<RepositoryResponse<PagedResult<Expense>>> SearchExpenses(
        Expression<Func<Expense, bool>> query,
        string guildId,
        int pageNumber = 1,
        int pageSize = 10
    )
    {
        var totalCount = await _context.Expenses.Where(e => e.GuildId == guildId).CountAsync(query);

        var items = await _context
            .Expenses.AsNoTracking()
            .Where(e => !e.IsDeleted && e.GuildId == guildId)
            .Where(query)
            .OrderBy(d => d.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var pagedResult = new PagedResult<Expense>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
        };

        return RepositoryResponse<PagedResult<Expense>>.Success(pagedResult);
    }
}
