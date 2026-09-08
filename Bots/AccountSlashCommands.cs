using Discord.Interactions;
using ExpenseManager.Controllers;
using ExpenseManager.Data.AccountRequests.Requests;
using ExpenseManager.Data.AccountResponses.Responses;
using ExpenseManager.Data.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManager.Bots;

[Group("account", "Account management commands")]
public class AccountSlashCommands(IServiceProvider services) : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IServiceProvider _services = services;

    [SlashCommand("create", "Create a new account.")]
    public async Task CreateAccount(
        [Summary("nickname", "Display name for the account.")] string nickname,
        [Summary("income", "Initial income amount.")] double income = 0,
        [Summary("savings", "Initial savings amount.")] double savings = 0)
    {
        var discordId = Context.User.Id.ToString();

        var request = new AccountCreationRequest
        {
            Nickname = nickname,
            DiscordId = discordId,
            Income = income,
            Savings = savings,
        };

        var controller = ActivatorUtilities.CreateInstance<AccountManagerController>(_services);
        var guildId = Context.Guild?.Id.ToString();
        var actionResult = await controller.CreateAccount(request, discordId, Context.User.Username, guildId);

        if (actionResult is ObjectResult obj && obj.Value is ServiceResponse<AccountResponse> svc)
        {
            await RespondAsync(svc.IsSuccess ? $"Account created successfully: {svc.Data?.Nickname}" : svc.Message, ephemeral: true);
            return;
        }

        await RespondAsync("Unexpected response from API controller.", ephemeral: true);
    }

    [SlashCommand("rename", "Change your account nickname.")]
    public async Task RenameAccount([Summary("nickname", "New display name for the account.")] string newNickname)
    {
        var discordId = Context.User.Id.ToString();
        var controller = ActivatorUtilities.CreateInstance<AccountManagerController>(_services);
        var data = new Dictionary<string, string> { ["nickname"] = newNickname };
        var actionResult = await controller.Rename(data, discordId, Context.Guild?.Id.ToString());

        if (actionResult is ObjectResult obj && obj.Value is ServiceResponse<AccountResponse> svc)
        {
            await RespondAsync(svc.IsSuccess ? $"Account renamed to: {svc.Data?.Nickname}" : svc.Message, ephemeral: true);
            return;
        }

        await RespondAsync("Unexpected response from API controller.", ephemeral: true);
    }

    [SlashCommand("increase", "Increase an account balance.")]
    public async Task IncreaseAmount([Summary("amount", "Amount to add.")] double amount)
    {
        var discordId = Context.User.Id.ToString();
        var controller = ActivatorUtilities.CreateInstance<AccountManagerController>(_services);
        var actionResult = await controller.IncreaseAmount(discordId, amount, Context.Guild?.Id.ToString());

        if (actionResult is ObjectResult obj && obj.Value is ServiceResponse<AccountResponse> svc)
        {
            await RespondAsync(svc.IsSuccess ? $"Increased balance to {svc.Data?.Income}." : svc.Message, ephemeral: true);
            return;
        }

        await RespondAsync("Unexpected response from API controller.", ephemeral: true);
    }

    [SlashCommand("decrease", "Decrease an account balance.")]
    public async Task DecreaseAmount([Summary("amount", "Amount to subtract.")] double amount)
    {
        var discordId = Context.User.Id.ToString();
        var controller = ActivatorUtilities.CreateInstance<AccountManagerController>(_services);
        var actionResult = await controller.DecreaseAmount(discordId, amount, Context.Guild?.Id.ToString());

        if (actionResult is ObjectResult obj && obj.Value is ServiceResponse<AccountResponse> svc)
        {
            await RespondAsync(svc.IsSuccess ? $"Decreased balance to {svc.Data?.Income}." : svc.Message, ephemeral: true);
            return;
        }

        await RespondAsync("Unexpected response from API controller.", ephemeral: true);
    }

    [SlashCommand("rollover", "Move income into savings.")]
    public async Task Rollover()
    {
        var discordId = Context.User.Id.ToString();
        var controller = ActivatorUtilities.CreateInstance<AccountManagerController>(_services);
        var actionResult = await controller.Rollover(discordId, Context.Guild?.Id.ToString());

        if (actionResult is ObjectResult obj && obj.Value is ServiceResponse<AccountResponse> svc)
        {
            await RespondAsync(svc.IsSuccess ? $"Rolled over income to savings." : svc.Message, ephemeral: true);
            return;
        }

        await RespondAsync("Unexpected response from API controller.", ephemeral: true);
    }

    [SlashCommand("delete", "Delete an account.")]
    public async Task DeleteAccount()
    {
        var discordId = Context.User.Id.ToString();
        var controller = ActivatorUtilities.CreateInstance<AccountManagerController>(_services);
        var actionResult = await controller.DeleteAccount(discordId, Context.Guild?.Id.ToString());

        if (actionResult is ObjectResult obj && obj.Value is ServiceResponse svc)
        {
            await RespondAsync(svc.Message, ephemeral: true);
            return;
        }

        await RespondAsync("Unexpected response from API controller.", ephemeral: true);
    }
}
