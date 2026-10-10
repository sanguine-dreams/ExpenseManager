using ExpenseManager.Data.Models;
using ExpenseManager.Data.Requests;
using ExpenseManager.Data.Requests.ExpenseRequests;
using ExpenseManager.Data.Responses;
using ExpenseManager.Data.Responses.ExpenseResponse;
using ExpenseManager.Repository;
using MapsterMapper;

namespace ExpenseManager.Services;

public class ExpenseService(IExpenseRepository expenseRepository, IAccountRepository accountRepository, IMapper mapper, IAccountService accountService) : IExpenseService
{
    private readonly IExpenseRepository _expenseRepository = expenseRepository;
    private readonly IAccountRepository _accountRepository = accountRepository;

    private readonly IAccountService _accountService = accountService;
    private readonly IMapper _mapper = mapper;

    public async Task<ServiceResponse<CreateExpenseResponse>> CreateExpense(CreateExpenseRequest request, string? discordId = null, string? guildId = null)
    {
        var account = default(Account);

        if (!string.IsNullOrWhiteSpace(discordId))
        {
            var accountResult = await _accountRepository.GetByDiscordId(discordId, guildId ?? string.Empty);
            if (!accountResult.IsSuccess)
            {
                return ServiceResponse<CreateExpenseResponse>.BadRequest("No account found for your Discord user. Create one using the /account create command.");
            }

            account = accountResult.Data;
            if (!string.IsNullOrWhiteSpace(account.Nickname))
            {
                request.PurchasedBy = account.Nickname;
            }

            account.Income -= request.Price;
            var editAcc = await _accountRepository.EditAccount(account);
            if (!editAcc.IsSuccess)
                return ServiceResponse<CreateExpenseResponse>.InternalError($"Failed updating account: {editAcc.Message}");
        }

        var expense = _mapper.Map<Expense>(request);
        if (account is not null)
        {
            expense.AccountId = account.Id;
        }
        if (!string.IsNullOrWhiteSpace(guildId)) expense.GuildId = guildId!;
        var result = await _expenseRepository.CreateExpense(expense);
        var response = _mapper.Map<CreateExpenseResponse>(result.Data);
        response.Price = result.Data.Price.ToString("N0");

        return ServiceResponse<CreateExpenseResponse>.Success(response);
    }

    public async Task<ServiceResponse> DeleteExpense(Guid id, string? guildId = null)
    {
        var existingExpense = await _expenseRepository.GetById(id, guildId);
        var result = await _expenseRepository.DeleteExpense(id, guildId ?? string.Empty);


        if (!result.IsSuccess) return ServiceResponse.InternalError(result.Message);

        return ServiceResponse.Success("Expense deleted successfully");

    }

    public async Task<ServiceResponse<PagedResult<GetExpensesResponse>>> GetAllExpenses(PaginatedRequest request, string? guildId = null)
    {
        var result = await _expenseRepository.GetAllExpenses(request, guildId ?? string.Empty);

        if (!result.IsSuccess) return ServiceResponse<PagedResult<GetExpensesResponse>>.InternalError(result.Message);

        var response = new PagedResult<GetExpensesResponse>
        {
            PageNumber = result.Data.PageNumber,
            PageSize = result.Data.PageSize,
            TotalCount = result.Data.TotalCount,
            Items = result.Data.Items.Select(item => new GetExpensesResponse
            {
                Id = item.Id,
                Price = item.Price.ToString("N0"),
                CreatedAt = item.CreatedAt,
                Category = item.Category.ToString(),
                Other = item.Other,
                PurchasedBy = item.PurchasedBy,
            }).ToList(),
        };

        return ServiceResponse<PagedResult<GetExpensesResponse>>.Success(response);
    }

    public async Task<ServiceResponse<GetExpensesResponse>> GetExpenseById(Guid id, string? guildId = null)
    {
        var result = await _expenseRepository.GetById(id, guildId ?? string.Empty);

        if (!result.IsSuccess) return ServiceResponse<GetExpensesResponse>.InternalError(result.Message);

        var response = _mapper.Map<GetExpensesResponse>(result.Data);
        response.Price = result.Data.Price.ToString("N0");

        return ServiceResponse<GetExpensesResponse>.Success(response);

    }

}