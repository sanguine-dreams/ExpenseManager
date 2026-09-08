namespace ExpenseManager.Data.Models;

public class GuildSettings
{
    public Guid Id { get; set; } = new Guid();
    public ulong GuildId { get; set; } 
    public string Prefix { get; set; } = "!";
    public string LogChannelId { get; set; }
    public bool IsWelcomeEnabled { get; set; } = true;
    
}