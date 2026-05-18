using Application.UseCases.HealthCheck.Abstractions;
using Application.UseCases.HealthCheck.Errors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using NSubstitute;
using Shared.ResultPattern;
using System.Net;
using WebAPI.Minimal.UseCases.HealthCheck;

namespace WebAPI.Minimal.UnitTests.UseCases.HealthCheck;

public class HealthCheckEndpointTests
{
    private readonly IHealthCheckUseCase _healthCheckUseCase;
    private readonly FakeLogger<HealthCheckEndpoint> _logger;
    private readonly CancellationToken _cancellationToken;

    public HealthCheckEndpointTests()
    {
        _healthCheckUseCase = Substitute.For<IHealthCheckUseCase>();
        _logger = new FakeLogger<HealthCheckEndpoint>();
        _cancellationToken = default;
    }

    [Fact]
    public async Task ShouldReturnOkWhenUseCaseSucceeds()
    {
        // Arrange
        _healthCheckUseCase
            .Run(_cancellationToken)
            .Returns(Result.Ok<HealthCheckError>());

        // Act
        var endpointResult = await HealthCheckEndpoint.Execute(_healthCheckUseCase, _logger, _cancellationToken);

        // Assert response
        var jsonResult = Assert.IsType<JsonHttpResult<HealthCheckEndpointResponse>>(endpointResult);
        Assert.Equal((int)HttpStatusCode.OK, jsonResult.StatusCode);
        Assert.NotNull(jsonResult.Value);
        Assert.Equal("Healthy!", jsonResult.Value.Message);

        // Assert logs
        var traceLog = _logger.Collector.GetSnapshot()[0];
        Assert.Equal(LogLevel.Trace, traceLog.Level);
        Assert.Equal("Healthy!", traceLog.Message);
    }

    [Fact]
    public async Task ShouldReturn503WhenServiceIsUnreachable()
    {
        // Arrange
        var error = new ServiceUnreachable("Database",
            Context: ErrorContext.FromException(new Exception("Connection refused"), source: "DbHealthCheck"));

        _healthCheckUseCase
            .Run(_cancellationToken)
            .Returns(Result<Unit, HealthCheckError>.Failure(error));

        // Act
        var endpointResult = await HealthCheckEndpoint.Execute(_healthCheckUseCase, _logger, _cancellationToken);

        // Assert response
        var jsonResult = Assert.IsType<JsonHttpResult<HealthCheckEndpointResponse>>(endpointResult);
        Assert.Equal((int)HttpStatusCode.ServiceUnavailable, jsonResult.StatusCode);
        Assert.NotNull(jsonResult.Value);
        Assert.Equal("Unhealthy: Database is unreachable.", jsonResult.Value.Message);

        // Assert logs
        var errorLog = _logger.Collector.GetSnapshot()[0];
        Assert.Equal(LogLevel.Error, errorLog.Level);
        Assert.Equal("Connection refused", errorLog.Message);
    }

    [Fact]
    public async Task ShouldReturn500WhenUnhandledExceptionEscaped()
    {
        // Arrange
        var exception = new InvalidOperationException("kaboom");
        var error = new UnhandledException(exception, Context: ErrorContext.FromException(exception));

        _healthCheckUseCase
            .Run(_cancellationToken)
            .Returns(Result<Unit, HealthCheckError>.Failure(error));

        // Act
        var endpointResult = await HealthCheckEndpoint.Execute(_healthCheckUseCase, _logger, _cancellationToken);

        // Assert response
        var jsonResult = Assert.IsType<JsonHttpResult<HealthCheckEndpointResponse>>(endpointResult);
        Assert.Equal((int)HttpStatusCode.InternalServerError, jsonResult.StatusCode);
        Assert.NotNull(jsonResult.Value);
        Assert.Equal("Unhealthy: unexpected failure.", jsonResult.Value.Message);

        // Assert logs — UnhandledException branch captures the exception itself
        var errorLog = _logger.Collector.GetSnapshot()[0];
        Assert.Equal(LogLevel.Error, errorLog.Level);
        Assert.Same(exception, errorLog.Exception);
        Assert.Equal("Unhandled exception in health check", errorLog.Message);
    }
}
