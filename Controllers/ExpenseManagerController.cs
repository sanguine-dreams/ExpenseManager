using ExpenseManager.Data.Requests;
using ExpenseManager.Data.Requests.ExpenseRequests;
using ExpenseManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManager.Controllers;

[ApiController, Route("api/[controller]")]

public class ExpenseManagerController(IExpenseService expenseService) : ControllerBase
{
    private readonly IExpenseService _expenseService = expenseService;

    [HttpPost("[action]")]
    public async Task<IActionResult> CreateExpense(CreateExpenseRequest request, [FromHeader(Name = "X-Discord-Id")] string? discordId = null, [FromHeader(Name = "X-Discord-Username")] string? discordUsername = null, [FromHeader(Name = "X-Guild-Id")] string? guildId = null)
    {
        var response = await _expenseService.CreateExpense(request, discordId, guildId);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetAllExpenses([FromQuery] PaginatedRequest request, [FromHeader(Name = "X-Guild-Id")] string? guildId = null)
    {
        var response = await _expenseService.GetAllExpenses(request, guildId);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("[action]/{id}")]
    public async Task<IActionResult> GetExpenseById(Guid id, [FromHeader(Name = "X-Guild-Id")] string? guildId = null)
    {
        var response = await _expenseService.GetExpenseById(id, guildId);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("[action]/{id}")]
    public async Task<IActionResult> DeleteExpense(Guid id, [FromHeader(Name = "X-Guild-Id")] string? guildId = null)
    {
        var response = await _expenseService.DeleteExpense(id, guildId);
        return StatusCode(response.StatusCode, response);
    }
}