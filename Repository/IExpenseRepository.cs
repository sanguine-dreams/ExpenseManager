using System.Linq.Expressions;
using ExpenseManager.Data.Models;
using ExpenseManager.Data.Requests;
using ExpenseManager.Data.Responses;

namespace ExpenseManager.Repository;

public interface IExpenseRepository
{
    Task<RepositoryResponse<PagedResult<Expense>>> GetAllExpenses(PaginatedRequest request, string guildId);
    Task<RepositoryResponse<Expense>> CreateExpense(Expense entity);
    Task<RepositoryResponse> DeleteExpense(Guid id, string guildId);
    Task<RepositoryResponse<PagedResult<Expense>>> SearchExpenses(Expression<Func<Expense, bool>> query,
        string guildId,
        int pageNumber = 1,
        int pageSize = 10);
    Task<RepositoryResponse<Expense>> GetById(Guid id, string guildId);
}