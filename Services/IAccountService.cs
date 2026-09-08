using ExpenseManager.Data.AccountRequests.Requests;
using ExpenseManager.Data.AccountResponses.Responses;
using ExpenseManager.Data.Responses;

namespace ExpenseManager.Services;

public interface IAccountService
{
    Task<ServiceResponse<AccountResponse>> Create(AccountCreationRequest request, string? guildId = null);
    Task<ServiceResponse<AccountResponse>> GetOrCreateAccount(string discordId, string discordDisplayName, string? guildId = null);
    Task<ServiceResponse<AccountResponse>> DecreaseAmount(string id, Double amount, string? guildId = null);
    Task<ServiceResponse> Delete(string id, string? guildId = null);
    Task<ServiceResponse<AccountResponse>> IncreaseAmount(string id, Double amount, string? guildId = null);
    Task<ServiceResponse<AccountResponse>> Rollover(string id, string? guildId = null);
    Task<ServiceResponse<AccountResponse>> Rename(string discordId, string newNickname, string? guildId = null);
}