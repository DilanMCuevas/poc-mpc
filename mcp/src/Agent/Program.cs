using Agent.Features.Chat;
using Agent.Integrations.MCP;
using Agent.Integrations.Ollama;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<McpOptions>(
    builder.Configuration.GetSection(McpOptions.SectionName));

builder.Services.AddSingleton<IMcpService, McpService>();

builder.Services.Configure<OllamaOptions>(
    builder.Configuration.GetSection(OllamaOptions.SectionName));

builder.Services.AddHttpClient<IOllamaService, OllamaService>(
    (serviceProvider, httpClient) =>
    {
        var options = serviceProvider
            .GetRequiredService<
                Microsoft.Extensions.Options.IOptions<OllamaOptions>>()
            .Value;

        httpClient.BaseAddress = new Uri(options.BaseUrl);
    });

builder.Services.AddScoped<IChatService, ChatService>();

var app = builder.Build();

app.MapGet("/", () => "Agent funcionando");

app.MapControllers();

app.Run();