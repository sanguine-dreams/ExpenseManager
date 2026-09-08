namespace ExpenseManager.Data.AccountResponses.Responses;

public class AccountResponse
{
    public Guid Id { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public string DiscordId { get; set; } = string.Empty;
    public Double Income { get; set; }
    public Double Savings { get; set; }
}