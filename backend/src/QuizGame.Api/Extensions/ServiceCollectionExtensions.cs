using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using QuizGame.Api.BackgroundServices;
using QuizGame.Api.Hubs;
using QuizGame.Api.Notifications;
using QuizGame.Application.Abstractions.Notifications;
using QuizGame.Infrastructure.Persistence;
using QuizGame.Infrastructure.Resilience;

namespace QuizGame.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.ConfigureHttpJsonOptions(options =>
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        services.AddOpenApi();

        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        string frontendOrigin = configuration["Cors:FrontendOrigin"] ?? "http://localhost:4200";
        services.AddCors(options => options.AddPolicy(CorsPolicies.Frontend, policy => policy
            .WithOrigins(frontendOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()));

        services.AddSignalR(options =>
        {
            options.KeepAliveInterval = TimeSpan.FromSeconds(15);
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
            options.MaximumReceiveMessageSize = 32 * 1024;
        }).AddJsonProtocol(options =>
            options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddFixedWindowLimiter(RateLimiterPolicies.Api, limiter =>
            {
                limiter.PermitLimit = 100;
                limiter.Window = TimeSpan.FromMinutes(1);
                limiter.QueueLimit = 0;
                limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });
        });

        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"])
            .AddDbContextCheck<QuizGameDbContext>("database", tags: ["ready"])
            .AddCheck<OutboxCircuitHealthCheck>("outbox-circuit", tags: ["ready"]);

        services.AddScoped<IGameNotifier, SignalRGameNotifier>();
        services.AddHostedService<GameTimeoutService>();

        return services;
    }
}
