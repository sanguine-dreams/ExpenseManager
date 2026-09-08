namespace ExpenseManager.Data.Models;

public class Expense
{
    public Guid Id { get; set; } = new Guid();
    public required int Price { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public required Category Category { get; set; }
    public required string GuildId { get; set; }
    public string? Other { get; set; }
    public required string PurchasedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
    public string? DeletedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public enum Category
{
    Transportation,
    Miscellaneous,
    Groceries,
    Electric,
    Rent,
    Internet,
    Phonebill,
    Cats,
    VetBills,
    Other
}