using ExpenseManager.Data.Models;
using ExpenseManager.Data.Requests;
using ExpenseManager.Data.Responses;
using ExpenseManager.Repository;
using MapsterMapper;

namespace ExpenseManager.Services;

public class AccountService(IAccountRepository accountRepository, IMapper mapper) : IAccountService
{
    private readonly IAccountRepository _accountRepository = accountRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<ServiceResponse<AccountResponse>> GetOrCreateAccount(string discordId, string discordDisplayName, string? guildId = null)
    {
        var account = await _accountRepository.GetByDiscordId(discordId, guildId ?? string.Empty);

        if (account.IsSuccess)
            return ServiceResponse<AccountResponse>.Success(_mapper.Map<AccountResponse>(account.Data));

        var createRequest = new AccountCreationRequest
        {
            Nickname = discordDisplayName,
            DiscordId = discordId,
            Income = 0.0,
            Savings = 0.0,
        };

        return await Create(createRequest, guildId);
    }

    public async Task<ServiceResponse<AccountResponse>> Create(AccountCreationRequest request, string? guildId = null)
    {
        var account = _mapper.Map<Account>(request);
        if (!string.IsNullOrWhiteSpace(guildId)) account.GuildId = guildId!;
        var result = await _accountRepository.Create(account);
        if (!result.IsSuccess)
            return ServiceResponse<AccountResponse>.InternalError(result.Message);
        var response = _mapper.Map<AccountResponse>(result.Data);
        return ServiceResponse<AccountResponse>.Success(response);
    }

    public async Task<ServiceResponse<AccountResponse>> DecreaseAmount(string id, double amount, string? guildId = null)
    {
        var accountResult = await _accountRepository.GetByDiscordId(id, guildId ?? string.Empty);
        if (!accountResult.IsSuccess)
            return ServiceResponse<AccountResponse>.InternalError(accountResult.Message);

        accountResult.Data.Income -= amount;
        var result = await _accountRepository.EditAccount(accountResult.Data);
        if (!result.IsSuccess)
            return ServiceResponse<AccountResponse>.InternalError(result.Message);
        var response = _mapper.Map<AccountResponse>(result.Data);
        return ServiceResponse<AccountResponse>.Success(response);
    }

    public async Task<ServiceResponse> Delete(string id, string? guildId = null)
    {
        var result = await _accountRepository.Delete(id, guildId ?? string.Empty);

        if (!result.IsSuccess)
            return ServiceResponse.InternalError(result.Message);

        return ServiceResponse.Success(result.Message);
    }

    public async Task<ServiceResponse<AccountResponse>> IncreaseAmount(string id, double amount, string? guildId = null)
    {
        var accountResult = await _accountRepository.GetByDiscordId(id, guildId ?? string.Empty);
        if (!accountResult.IsSuccess)
            return ServiceResponse<AccountResponse>.InternalError(accountResult.Message);

        accountResult.Data.Income += amount;
        var result = await _accountRepository.EditAccount(accountResult.Data);
        if (!result.IsSuccess)
            return ServiceResponse<AccountResponse>.InternalError(result.Message);
        var response = _mapper.Map<AccountResponse>(result.Data);
        return ServiceResponse<AccountResponse>.Success(response);
    }

    public async Task<ServiceResponse<AccountResponse>> Rollover(string id, string? guildId = null)
    {
        var accountResult = await _accountRepository.GetByDiscordId(id, guildId ?? string.Empty);
        if (!accountResult.IsSuccess)
            return ServiceResponse<AccountResponse>.InternalError(accountResult.Message);

        var rolloverAmount = accountResult.Data.Income;
        accountResult.Data.Savings += rolloverAmount;
        accountResult.Data.Income = 0.0;
        var result = await _accountRepository.EditAccount(accountResult.Data);
        if (!result.IsSuccess)
            return ServiceResponse<AccountResponse>.InternalError(result.Message);
        var response = _mapper.Map<AccountResponse>(result.Data);
        return ServiceResponse<AccountResponse>.Success(response);
    }

    public async Task<ServiceResponse<AccountResponse>> Rename(string discordId, string newNickname, string? guildId = null)
    {
        var accountResult = await _accountRepository.GetByDiscordId(discordId, guildId ?? string.Empty);
        if (!accountResult.IsSuccess)
            return ServiceResponse<AccountResponse>.InternalError(accountResult.Message);

        accountResult.Data.Nickname = newNickname;
        var result = await _accountRepository.EditAccount(accountResult.Data);
        if (!result.IsSuccess)
            return ServiceResponse<AccountResponse>.InternalError(result.Message);

        var response = _mapper.Map<AccountResponse>(result.Data);
        return ServiceResponse<AccountResponse>.Success(response);
    }
}
