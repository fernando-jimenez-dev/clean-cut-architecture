using Application.UseCases.HealthCheck.Abstractions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using NSubstitute;
using Shared.Errors;
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
        _healthCheckUseCase.Run(_cancellationToken).Returns(Result.Success());

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
    public async Task ShouldReturn500_AndLogError_WhenUseCaseReturnsUnexpectedError()
    {
        // Arrange
        var exception = new Exception("boom");
        var unexpectedError = new UnhandledExceptionError(exception);
        _healthCheckUseCase
            .Run(_cancellationToken)
            .Returns(Result.Failure(unexpectedError));

        // Act
        var endpointResult = await HealthCheckEndpoint.Execute(_healthCheckUseCase, _logger, _cancellationToken);

        // Assert response
        var jsonResult = Assert.IsType<JsonHttpResult<HealthCheckEndpointResponse>>(endpointResult);
        Assert.Equal((int)HttpStatusCode.InternalServerError, jsonResult.StatusCode);
        Assert.NotNull(jsonResult.Value);
        Assert.Equal("Unhealthy.", jsonResult.Value.Message);

        // Assert logs
        var errorLog = _logger.Collector.GetSnapshot()[0];
        Assert.Equal(LogLevel.Error, errorLog.Level);
        Assert.Equal(unexpectedError.Exception, errorLog.Exception);
        Assert.Equal($"HealthCheck failed with an unhandled exception. Code: {unexpectedError.Code}", errorLog.Message);
    }

    [Fact]
    public async Task ShouldReturn500_AndLogError_WhenUseCaseReturnsUnknownError()
    {
        // Arrange
        var unknownError = new UnknownError();
        _healthCheckUseCase
            .Run(_cancellationToken)
            .Returns(Result.Failure(unknownError));

        // Act
        var endpointResult = await HealthCheckEndpoint.Execute(_healthCheckUseCase, _logger, _cancellationToken);

        // Assert response
        var jsonResult = Assert.IsType<JsonHttpResult<HealthCheckEndpointResponse>>(endpointResult);
        Assert.Equal((int)HttpStatusCode.InternalServerError, jsonResult.StatusCode);
        Assert.NotNull(jsonResult.Value);
        Assert.Equal("Unhealthy.", jsonResult.Value.Message);

        // Assert logs
        var errorLog = _logger.Collector.GetSnapshot()[0];
        Assert.Equal(LogLevel.Error, errorLog.Level);
        Assert.Equal($"HealthCheck failed with an error not mapped by endpoint. Code: {unknownError.Code}. Message: {unknownError.Message}", errorLog.Message);
    }

    private sealed record UnknownError()
        : Error(code: "health.modeled_failure", message: "Modeled failure.");
}