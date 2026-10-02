namespace ExpenseManager.Data.AccountResponses.Responses;

public class AccountResponse
{
    public Guid Id { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public string DiscordId { get; set; } = string.Empty;
    public string Income { get; set; } = "0";
    public string Savings { get; set; } = "0";
}