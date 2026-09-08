using ExpenseManager.Data.Models;

namespace ExpenseManager.Data.Responses.ExpenseResponse;

public class CreateExpenseResponse {
    public Guid Id { get; set; } 
    public required int Price { get; set; } 
    public DateTime CreatedAt { get; set; }
    public required Category Category { get; set; }
    public string? Other { get; set; }
    public required string PurchasedBy { get; set; }
  
}