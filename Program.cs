using Discord.Interactions;
using Discord.WebSocket;
using ExpenseManager.Bots;
using ExpenseManager.Services;
using ExpenseManager.Repository;
using ExpenseManager.Data.DB;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Discord;
using dotenv.net;

try
{
    DotEnv.Load();
}
catch { }

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMapster();

builder.Services.Configure<DiscordOptions>(builder.Configuration.GetSection("Discord"));
builder.Services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
{
    GatewayIntents = GatewayIntents.Guilds,
}));
builder.Services.AddSingleton(sp => new InteractionService(sp.GetRequiredService<DiscordSocketClient>()));
builder.Services.AddHostedService<DiscordBotHostedService>();

// HTTP client to call local API controllers from the bot (modular integration)
builder.Services.AddHttpClient("local", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("LocalApiBase", "http://localhost:5000"));
});

builder.Services.AddDbContext<DBContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DBContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
