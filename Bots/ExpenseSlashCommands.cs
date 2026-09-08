using Discord.Interactions;
using ExpenseManager.Data.Models;
using ExpenseManager.Data.Requests.ExpenseRequests;
using ExpenseManager.Data.Responses;
using ExpenseManager.Data.Responses.ExpenseResponse;
using ExpenseManager.Services;
using ExpenseManager.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManager.Bots;

[Group("expense", "Expense management commands")]
public class ExpenseSlashCommands(IExpenseService expenseService, IServiceProvider services) : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IExpenseService _expenseService = expenseService;
    private readonly IServiceProvider _services = services;

    [SlashCommand("create", "Create a new expense record.")]
    public async Task CreateExpense(
        [Summary("price", "The expense amount.")] int price,
        [Summary("category", "Expense category.")] Category category,
        [Summary("other", "Optional additional details.")] string other = "")
    {
        await DeferAsync(ephemeral: true);
        var userDiscordId = Context.User.Id.ToString();
        var username = Context.User.Username;

        var request = new CreateExpenseRequest
        {
            Price = price,
            Category = category,
            PurchasedBy = username,
            Other = string.IsNullOrWhiteSpace(other) ? null : other,
        };


        var controller = ActivatorUtilities.CreateInstance<ExpenseManagerController>(_services);
        var guildId = Context.Guild?.Id.ToString();
        var actionResult = await controller.CreateExpense(request, userDiscordId, username, guildId);

        if (actionResult is ObjectResult obj && obj.Value is ServiceResponse<CreateExpenseResponse> svc)
        {
            var message = svc.IsSuccess ? $"Expense created successfully. Price: {svc.Data?.Price} of category {svc.Data?.Category.ToString()}" : svc.Message;
            await FollowupAsync(message, ephemeral: true);
            return;
        }

        await FollowupAsync("Unexpected response from API controller.", ephemeral: true);
    }

    [SlashCommand("list", "List expenses by page.")]
    public async Task ListExpenses(
        [Summary("page", "Page number.")] int page = 1,
        [Summary("page_size", "Items per page.")] int pageSize = 10)
    {
        var controller = ActivatorUtilities.CreateInstance<ExpenseManagerController>(_services);
        var actionResult = await controller.GetAllExpenses(new Data.Requests.PaginatedRequest { PageNumber = page, PageSize = pageSize }, Context.Guild?.Id.ToString());

        if (actionResult is ObjectResult obj && obj.Value is ServiceResponse<PagedResult<GetExpensesResponse>> svc)
        {
            if (svc.Data?.Items is not { Count: > 0 })
            {
                await RespondAsync("No expenses found for this page.", ephemeral: true);
                return;
            }

            var lines = svc.Data.Items.Select(item => $"• {item.Price} | {item.Category} | {item.Other ?? "-"} | {item.PurchasedBy}");
            var message = string.Join("\n", lines);
            await RespondAsync($"Expenses (page {svc.Data.PageNumber}/{svc.Data.TotalPages}):\n{message}", ephemeral: true);
            return;
        }

        await RespondAsync("Unexpected response from API controller.", ephemeral: true);
    }

    [SlashCommand("get", "Get one expense by id.")]
    public async Task GetExpenseById([Summary("id", "Expense id.")] string id)
    {
        if (!Guid.TryParse(id, out var expenseId))
        {
            await RespondAsync("Invalid expense id.", ephemeral: true);
            return;
        }

        var response = await _expenseService.GetExpenseById(expenseId, Context.Guild?.Id.ToString());
        if (!response.IsSuccess)
        {
            await RespondAsync(response.Message, ephemeral: true);
            return;
        }

        var item = response.Data!;
        await RespondAsync($"Expense `{item.Id}`:\nPrice: {item.Price}\nCategory: {item.Category}\nPurchased by: {item.PurchasedBy}\nOther: {item.Other ?? "-"}", ephemeral: true);
    }

    [SlashCommand("delete", "Delete an expense by id.")]
    public async Task DeleteExpense([Summary("id", "Expense id.")] string id)
    {
        if (!Guid.TryParse(id, out var expenseId))
        {
            await RespondAsync("Invalid expense id.", ephemeral: true);
            return;
        }

        var response = await _expenseService.DeleteExpense(expenseId, Context.Guild?.Id.ToString());
        await RespondAsync(response.Message, ephemeral: true);
    }


}
