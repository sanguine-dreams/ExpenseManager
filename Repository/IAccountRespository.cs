using ExpenseManager.Data.Models;
using ExpenseManager.Data.Responses;

namespace ExpenseManager.Repository;

public interface IAccountRepository
{
    Task<RepositoryResponse<Account>> Create(Account account);
    Task<RepositoryResponse<Account>> GetByDiscordId(string discordId, string guildId);
    Task<RepositoryResponse<Account>> EditAccount(Account account);
    Task<RepositoryResponse<Account>> Delete(string discordId, string guildId);

}