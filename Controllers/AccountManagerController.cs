using ExpenseManager.Data.AccountRequests.Requests;
using ExpenseManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManager.Controllers;

[ApiController, Route("api/[controller]")]
public class AccountManagerController(IAccountService accountService) : ControllerBase
{
    private readonly IAccountService _accountService = accountService;

    [HttpPost("[action]")]
    public async Task<IActionResult> CreateAccount(AccountCreationRequest input, [FromHeader(Name = "X-Discord-Id")] string? discordId = null, [FromHeader(Name = "X-Discord-Username")] string? discordUsername = null, [FromHeader(Name = "X-Guild-Id")] string? guildId = null)
    {
        // If the bot provided the discord id via header, prefer that value
        if (!string.IsNullOrWhiteSpace(discordId)) input.DiscordId = discordId;
        if (!string.IsNullOrWhiteSpace(discordUsername) && string.IsNullOrWhiteSpace(input.Nickname)) input.Nickname = discordUsername;

        var response = await _accountService.Create(input, guildId);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Rename([FromBody] Dictionary<string, string> body, [FromHeader(Name = "X-Discord-Id")] string? discordId = null, [FromHeader(Name = "X-Guild-Id")] string? guildId = null)
    {
        body.TryGetValue("nickname", out var newNickname);
        if (string.IsNullOrWhiteSpace(newNickname) || string.IsNullOrWhiteSpace(discordId))
            return BadRequest("Missing nickname or discord id");

        var response = await _accountService.Rename(discordId, newNickname, guildId);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("[action]")]
    public async Task<IActionResult> IncreaseAmount(string id, Double amount, [FromHeader(Name = "X-Guild-Id")] string? guildId = null)
    {
        var response = await _accountService.IncreaseAmount(id, amount, guildId);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("[action]")]
    public async Task<IActionResult> DecreaseAmount(string id, Double amount, [FromHeader(Name = "X-Guild-Id")] string? guildId = null)
    {
        var response = await _accountService.DecreaseAmount(id, amount, guildId);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("[action]")]
    public async Task<IActionResult> Rollover(string id, [FromHeader(Name = "X-Guild-Id")] string? guildId = null)
    {
        var response = await _accountService.Rollover(id, guildId);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("[action]")]
    public async Task<IActionResult> DeleteAccount(string id, [FromHeader(Name = "X-Guild-Id")] string? guildId = null)
    {
        var response = await _accountService.Delete(id, guildId);
        return StatusCode(response.StatusCode, response);
    }
}
