using QuizGame.Api.Endpoints;

namespace QuizGame.Api.Extensions;

public static class EndpointModuleExtensions
{
    public static IEndpointRouteBuilder MapEndpointModules(this IEndpointRouteBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        IEnumerable<IEndpointModule> modules = typeof(EndpointModuleExtensions).Assembly
            .GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false }
                && typeof(IEndpointModule).IsAssignableFrom(type))
            .Select(type => (IEndpointModule)Activator.CreateInstance(type)!);

        foreach (IEndpointModule module in modules)
        {
            module.MapEndpoints(app);
        }

        return app;
    }
}
