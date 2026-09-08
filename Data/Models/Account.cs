namespace ExpenseManager.Data.Models;

public class Account
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nickname { get; set; } = string.Empty;
    public string DiscordId { get; set; } = string.Empty;
    public string GuildId { get; set; } = string.Empty;
    public Double Income { get; set; } = 0.0;
    public Double Savings { get; set; } = 0.0;

}