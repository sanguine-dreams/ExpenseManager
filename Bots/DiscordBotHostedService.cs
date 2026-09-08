using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Options;


namespace ExpenseManager.Bots;

public class DiscordBotHostedService : IHostedService, IDisposable
{
    private readonly IServiceProvider _services;
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactionService;
    private readonly ILogger<DiscordBotHostedService> _logger;
    private readonly DiscordOptions _options;
    private bool _disposed;

    public DiscordBotHostedService(
        IServiceProvider services,
        DiscordSocketClient client,
        InteractionService interactionService,
        IOptions<DiscordOptions> options,
        ILogger<DiscordBotHostedService> logger)
    {
        _services = services;
        _client = client;
        _interactionService = interactionService;
        _logger = logger;
        _options = options.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.Token))
            throw new InvalidOperationException("Discord bot token is not configured. Set Discord:Token in appsettings or environment variables.");

        _client.Log += LogAsync;
        _interactionService.Log += LogAsync;
        _client.Ready += ReadyAsync;
        _client.JoinedGuild += JoinedGuildAsync;
        _client.InteractionCreated += InteractionCreatedAsync;

        await _interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), _services);
        await _client.LoginAsync(TokenType.Bot, _options.Token);
        await _client.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.StopAsync();
        await _client.LogoutAsync();
    }

    private async Task ReadyAsync()
    {
        _logger.LogInformation("Discord client ready. Registering slash commands...");

        var guilds = _client.Guilds;
        if (guilds.Count > 0)
        {
            foreach (var guild in guilds)
            {
                try
                {
                    await _interactionService.RegisterCommandsToGuildAsync(guild.Id);
                    _logger.LogInformation("Registered slash commands to guild {GuildName} ({GuildId})", guild.Name, guild.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to register commands to guild {GuildId}", guild.Id);
                }
            }
        }
        else
        {
            await _interactionService.RegisterCommandsGloballyAsync();
            _logger.LogInformation("Bot is not in any guilds. Registered slash commands globally.");
        }
    }

    private async Task InteractionCreatedAsync(SocketInteraction interaction)
    {
        try
        {
            var context = new SocketInteractionContext(_client, interaction);
            var result = await _interactionService.ExecuteCommandAsync(context, _services);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Discord command failed: {Reason}", result.ErrorReason);
                if (!interaction.HasResponded)
                    await interaction.RespondAsync($"Error: {result.ErrorReason}", ephemeral: true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while processing Discord interaction.");
            if (interaction.Type == InteractionType.ApplicationCommand && !interaction.HasResponded)
                await interaction.RespondAsync("An unexpected error occurred while handling your command.", ephemeral: true);
        }
    }

    private Task LogAsync(LogMessage log)
    {
        _logger.Log(ConvertLogSeverity(log.Severity), log.Message, log.Exception);
        return Task.CompletedTask;
    }

    private async Task JoinedGuildAsync(SocketGuild guild)
    {
        try
        {
            await _interactionService.RegisterCommandsToGuildAsync(guild.Id);

            SocketTextChannel? channel = guild.SystemChannel ?? guild.TextChannels.FirstOrDefault();
            if (channel == null)
            {
                _logger.LogInformation("No suitable text channel found in guild {GuildId} to send welcome message.", guild.Id);
                return;
            }

            var message = "Thanks for adding ExpenseManager!\n\n" +
                          "To get started, create your account with `/account create <nickname>`.\n" +
                          "Available commands:\n" +
                          "• /account create — Create an account\n" +
                          "• /account rename — Change your account nickname\n" +
                          "• /account increase — Increase account balance\n" +
                          "• /expense create — Create an expense (requires an account)\n" +
                          "• /expense list — List expenses\n" +
                          "• /expense get — Get an expense by id\n" +
                          "• /expense delete — Delete an expense\n\n" +
                          "If you need help, use the slash commands and check the documentation.";

            await channel.SendMessageAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed handling JoinedGuild for guild {GuildId}", guild.Id);
        }
    }

    private static LogLevel ConvertLogSeverity(LogSeverity severity) => severity switch
    {
        LogSeverity.Critical => LogLevel.Critical,
        LogSeverity.Error => LogLevel.Error,
        LogSeverity.Warning => LogLevel.Warning,
        LogSeverity.Info => LogLevel.Information,
        _ => LogLevel.Debug,
    };

    public void Dispose()
    {
        if (_disposed)
            return;

        _client.Log -= LogAsync;
        _interactionService.Log -= LogAsync;
        _client.Ready -= ReadyAsync;
        _client.JoinedGuild -= JoinedGuildAsync;
        _client.InteractionCreated -= InteractionCreatedAsync;
        _disposed = true;
    }
}
