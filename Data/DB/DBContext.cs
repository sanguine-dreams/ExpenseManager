using ExpenseManager.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManager.Data.DB;

public class DBContext(DbContextOptions<DBContext> options) : DbContext(options)
{
    public DbSet<Expense> Expenses { get; set; } = null!;
    public DbSet<GuildSettings> GuildSettings { get; set; } = null!;

    public DbSet<Account> Accounts { get; set; } = null!;
}