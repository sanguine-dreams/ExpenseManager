using ExpenseManager.Data.Models;
using ExpenseManager.Data.Requests;
using ExpenseManager.Data.Requests.ExpenseRequests;
using ExpenseManager.Data.Responses;
using ExpenseManager.Data.Responses.ExpenseResponse;
using ExpenseManager.Repository;
using MapsterMapper;

namespace ExpenseManager.Services;

public class ExpenseService(IExpenseRepository expenseRepository, IAccountRepository accountRepository, IMapper mapper) : IExpenseService
{
    private readonly IExpenseRepository _expenseRepository = expenseRepository;
    private readonly IAccountRepository _accountRepository = accountRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<ServiceResponse<CreateExpenseResponse>> CreateExpense(CreateExpenseRequest request, string? discordId = null, string? guildId = null)
    {
        if (!string.IsNullOrWhiteSpace(discordId))
        {
            var accountResult = await _accountRepository.GetByDiscordId(discordId, guildId ?? string.Empty);
            if (!accountResult.IsSuccess)
            {
                return ServiceResponse<CreateExpenseResponse>.BadRequest("No account found for your Discord user. Create one using the /account create command.");
            }

            accountResult.Data.Income -= request.Price;
            var editAcc = await _accountRepository.EditAccount(accountResult.Data);
            if (!editAcc.IsSuccess)
                return ServiceResponse<CreateExpenseResponse>.InternalError($"Failed updating account: {editAcc.Message}");
        }

        var expense = _mapper.Map<Expense>(request);
        if (!string.IsNullOrWhiteSpace(guildId)) expense.GuildId = guildId!;
        var result = await _expenseRepository.CreateExpense(expense);
        var response = _mapper.Map<CreateExpenseResponse>(result.Data);

        return ServiceResponse<CreateExpenseResponse>.Success(response);
    }

    public async Task<ServiceResponse> DeleteExpense(Guid id, string? guildId = null)
    {
        var result = await _expenseRepository.DeleteExpense(id, guildId ?? string.Empty);

        if (!result.IsSuccess) return ServiceResponse.InternalError(result.Message);

        return ServiceResponse.Success("Expense deleted successfully");

    }

    public async Task<ServiceResponse<PagedResult<GetExpensesResponse>>> GetAllExpenses(PaginatedRequest request, string? guildId = null)
    {
        var result = await _expenseRepository.GetAllExpenses(request, guildId ?? string.Empty);

        if (!result.IsSuccess) return ServiceResponse<PagedResult<GetExpensesResponse>>.InternalError(result.Message);
        var response = _mapper.Map<PagedResult<GetExpensesResponse>>(result.Data);

        return ServiceResponse<PagedResult<GetExpensesResponse>>.Success(response);
    }

    public async Task<ServiceResponse<GetExpensesResponse>> GetExpenseById(Guid id, string? guildId = null)
    {
        var result = await _expenseRepository.GetById(id, guildId ?? string.Empty);

        if (!result.IsSuccess) return ServiceResponse<GetExpensesResponse>.InternalError(result.Message);

        var response = _mapper.Map<GetExpensesResponse>(result.Data);

        return ServiceResponse<GetExpensesResponse>.Success(response);

    }

}