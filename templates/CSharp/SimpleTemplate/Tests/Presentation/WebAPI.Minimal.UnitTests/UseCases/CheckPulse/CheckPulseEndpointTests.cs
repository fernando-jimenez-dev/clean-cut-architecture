using Application.UseCases.CheckPulse;
using Application.UseCases.CheckPulse.Abstractions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Net;
using WebAPI.Minimal.UseCases.CheckPulse;

namespace WebAPI.Minimal.UnitTests.UseCases.CheckPulse;

public class CheckPulseEndpointTests
{
    private readonly ICheckPulseUseCase checkPulseUseCase;
    private readonly ILogger<CheckPulseEndpoint> logger;
    private readonly CancellationToken cancellationToken;

    public CheckPulseEndpointTests()
    {
        checkPulseUseCase = Substitute.For<ICheckPulseUseCase>();
        logger = Substitute.For<ILogger<CheckPulseEndpoint>>();
        cancellationToken = default;
    }

    [Fact]
    public async Task ShouldReturnOkWhenUseCaseSucceeds()
    {
        checkPulseUseCase
            .Run(Arg.Any<string>(), cancellationToken)
            .Returns(new CheckPulseUseCaseOutput(IsSuccess: true));

        var endpointResult = await CheckPulseEndpoint.Execute(checkPulseUseCase, logger, cancellationToken);

        var jsonResult = Assert.IsType<JsonHttpResult<CheckPulseEndpointResponse>>(endpointResult);
        Assert.Equal((int)HttpStatusCode.OK, jsonResult.StatusCode);
        Assert.NotNull(jsonResult.Value);
        Assert.Equivalent("Pulse checked!", jsonResult.Value.Message);
    }

    [Fact]
    public async Task ShouldReturnInternalServerErrorWhenUseCaseFails()
    {
        var errorMessage = "Pulse checking failed.";
        checkPulseUseCase
            .Run(Arg.Any<string>(), cancellationToken)
            .Returns(new CheckPulseUseCaseOutput(IsSuccess: false, ErrorMessage: errorMessage));

        var endpointResult = await CheckPulseEndpoint.Execute(checkPulseUseCase, logger, cancellationToken);

        var jsonResult = Assert.IsType<JsonHttpResult<CheckPulseEndpointResponse>>(endpointResult);
        Assert.Equal((int)HttpStatusCode.InternalServerError, jsonResult.StatusCode);
        Assert.NotNull(jsonResult.Value);
        Assert.Equivalent(errorMessage, jsonResult.Value.Message);
    }

    [Fact]
    public async Task ShouldReturnInternalServerErrorWhenUseCaseThrows()
    {
        checkPulseUseCase
            .Run(Arg.Any<string>(), cancellationToken)
            .Throws(new ApplicationException("Something bad happened"));

        var endpointResult = await CheckPulseEndpoint.Execute(checkPulseUseCase, logger, cancellationToken);

        var jsonResult = Assert.IsType<JsonHttpResult<CheckPulseEndpointResponse>>(endpointResult);
        Assert.Equal((int)HttpStatusCode.InternalServerError, jsonResult.StatusCode);
        Assert.NotNull(jsonResult.Value);
        Assert.Equivalent("Unrecoverable error encountered.", jsonResult.Value.Message);
    }
}