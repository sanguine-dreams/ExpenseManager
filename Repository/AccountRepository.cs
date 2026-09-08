using ExpenseManager.Data.DB;
using ExpenseManager.Data.Models;
using ExpenseManager.Data.Responses;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManager.Repository;

public class AccountRepository(DBContext context) : IAccountRepository
{
    private readonly DBContext _context = context;

    public async Task<RepositoryResponse<Account>> Create(Account account)
    {
        try
        {
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
            return RepositoryResponse<Account>.Success(account, "Account created successfully.");
        }
        catch (Exception ex)
        {
            return RepositoryResponse<Account>.InternalError(
                $"Error creating account: {ex.Message}"
            );
        }
    }

    public async Task<RepositoryResponse<Account>> GetByDiscordId(string discordId, string guildId)
    {
        try
        {
            var account = await _context
                .Accounts.Where(a => a.DiscordId == discordId && a.GuildId == guildId)
                .FirstOrDefaultAsync();
            if (account == null)
            {
                return RepositoryResponse<Account>.NotFound("Account not found.");
            }
            return RepositoryResponse<Account>.Success(account, "Account retrieved successfully.");
        }
        catch (Exception ex)
        {
            return RepositoryResponse<Account>.InternalError(
                $"Error retrieving account: {ex.Message}"
            );
        }
    }

    public async Task<RepositoryResponse<Account>> EditAccount(Account account)
    {
        if (account.Income < 0) return RepositoryResponse<Account>.NotAllowed("Insufficient Funds.");
        try
        {
            _context.Update(account);
            await _context.SaveChangesAsync();
            return RepositoryResponse<Account>.Success(account, "Account updated successfully.");
        }
        catch (Exception ex)
        {
            return RepositoryResponse<Account>.InternalError(
                $"Error decreasing amount: {ex.Message}"
            );
        }
    }

    public async Task<RepositoryResponse<Account>> Delete(string discordId, string guildId)
    {
        try
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.DiscordId == discordId && a.GuildId == guildId);
            if (account == null)
            {
                return RepositoryResponse<Account>.NotFound("Account not found.");
            }

            _context.Remove(account);
            await _context.SaveChangesAsync();
            return RepositoryResponse<Account>.Success("Account deleted successfully.");
        }
        catch (Exception ex)
        {
            return RepositoryResponse<Account>.InternalError(
                $"Error deleting account: {ex.Message}"
            );
        }
    }

}
