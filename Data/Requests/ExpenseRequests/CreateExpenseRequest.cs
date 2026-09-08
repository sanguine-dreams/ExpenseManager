using ExpenseManager.Data.Models;

namespace ExpenseManager.Data.Requests.ExpenseRequests;

public class CreateExpenseRequest {
    public required int Price { get; set; } 
    public required Category Category { get; set; }
    public string? Other { get; set; }
    public required string PurchasedBy { get; set; }

}