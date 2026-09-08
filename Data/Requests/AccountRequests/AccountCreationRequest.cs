namespace ExpenseManager.Data.AccountRequests.Requests;

public class AccountCreationRequest
{
    public string Nickname { get; set; } = string.Empty;
    public string DiscordId { get; set; } = string.Empty;
    public Double Income { get; set; } = 0.0;
    public Double Savings { get; set; } = 0.0;
}