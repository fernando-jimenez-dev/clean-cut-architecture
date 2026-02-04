using WebAPI.Minimal.UseCases.HealthCheck;

namespace WebAPI.Minimal.StartUp;

public static class EndpointsRegistryExtensions
{
    public static void RegisterWebApiEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.AddHealthCheckEndpoint();
    }

    private static IEndpointRouteBuilder AddHealthCheckEndpoint(this IEndpointRouteBuilder routes)
    {
        var groupName = "/healthcheck";
        var group = routes.MapGroup(groupName);

        group
            .MapGet("/", HealthCheckEndpoint.Execute)
            .WithName("HealthCheck")
            .WithOpenApi();

        return routes;
    }
}