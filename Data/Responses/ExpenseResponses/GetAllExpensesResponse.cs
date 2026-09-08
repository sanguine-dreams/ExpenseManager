using ExpenseManager.Data.Models;

namespace ExpenseManager.Data.Responses;

public class GetExpensesResponse
{
    public Guid Id { get; set; }
    public required int Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string Category { get; set; }
    public string? Other { get; set; }
    public required string PurchasedBy { get; set; }

}