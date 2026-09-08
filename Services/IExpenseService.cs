using ExpenseManager.Data.Requests;
using ExpenseManager.Data.Requests.ExpenseRequests;
using ExpenseManager.Data.Responses;
using ExpenseManager.Data.Responses.ExpenseResponse;

namespace ExpenseManager.Services;

public interface IExpenseService
{
    Task<ServiceResponse<CreateExpenseResponse>> CreateExpense(CreateExpenseRequest request, string? discordId = null, string? guildId = null);
    Task<ServiceResponse<PagedResult<GetExpensesResponse>>> GetAllExpenses(PaginatedRequest request, string? guildId = null);
    Task<ServiceResponse<GetExpensesResponse>> GetExpenseById(Guid id, string? guildId = null);
    Task<ServiceResponse> DeleteExpense(Guid id, string? guildId = null);
}