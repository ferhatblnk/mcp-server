using McpServer.Application.Interfaces;
using McpServer.Application.Services;
using McpServer.Domain.Interfaces;
using McpServer.Infrastructure.Configuration;
using McpServer.Infrastructure.Http;
using McpServer.Infrastructure.Tools.Grafana;
using McpServer.Infrastructure.Tools.Jira;
using McpServer.Infrastructure.Tools.Slack;
using Microsoft.Extensions.Options;

namespace McpServer.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMcpOptions(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<JiraOptions>()
            .Bind(config.GetSection(JiraOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<SlackOptions>()
            .Bind(config.GetSection(SlackOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<McpServerOptions>()
            .Bind(config.GetSection(McpServerOptions.SectionName));

        services.AddOptions<ClaudeOptions>()
            .Bind(config.GetSection("Claude"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    public static IServiceCollection AddMcpApplication(this IServiceCollection services)
    {
        services.AddSingleton<ToolRegistryService>();
        services.AddSingleton<IToolRegistryService>(sp => sp.GetRequiredService<ToolRegistryService>());

        services.AddScoped<IToolExecutionService, ToolExecutionService>();
        services.AddScoped<IMcpProtocolService, McpProtocolService>();
        services.AddScoped<IGrafanaAlertService, GrafanaAlertService>();

        return services;
    }

    public static IServiceCollection AddMcpTools(this IServiceCollection services)
    {
        // Jira
        services.AddHttpClient<JiraHttpClient>((sp, client) =>
        {
            var opts = sp.GetRequiredService<IOptions<JiraOptions>>().Value;
            client.BaseAddress = new Uri(opts.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(15);
        });
        services.AddSingleton<IMcpTool, JiraCreateIssueTool>();

        // Slack
        services.AddHttpClient<SlackSendMessageTool>(client =>
            client.Timeout = TimeSpan.FromSeconds(10));
        services.AddSingleton<IMcpTool, SlackSendMessageTool>();

        // Claude
        services.AddHttpClient<IClaudeClient, ClaudeHttpClient>((sp, client) =>
        {
            var key = sp.GetRequiredService<IOptions<ClaudeOptions>>().Value.ApiKey;
            client.BaseAddress = new Uri("https://api.anthropic.com/");
            client.DefaultRequestHeaders.Add("x-api-key", key);
            client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddSingleton<IMcpTool, GrafanaAnalyzeTool>();

        return services;
    }
}
