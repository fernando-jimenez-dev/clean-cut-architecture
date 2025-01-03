using Application.UseCases.CheckPulse;
using Application.UseCases.CheckPulse.Infrastructure;
using FluentResults;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Application.UnitTests.UseCases.CheckPulse;

public class CheckPulseUseCaseTests
{
    private readonly ILogger<CheckPulseUseCase> logger;
    private readonly ICheckPulseRepository checkPulseRepository;
    private readonly CheckPulseUseCase checkPulseUseCase;

    public CheckPulseUseCaseTests()
    {
        logger = Substitute.For<ILogger<CheckPulseUseCase>>();
        checkPulseRepository = Substitute.For<ICheckPulseRepository>();
        checkPulseUseCase = new CheckPulseUseCase(logger, checkPulseRepository);
    }

    [Fact]
    public async Task ShouldSucceedWhenVitalsAreGood()
    {
        string[] goodVitals = ["All", "Good!"];
        checkPulseRepository
            .RetrieveVitalReadings()
            .Returns(Result.Ok(goodVitals));

        var input = "Test message";
        var useCaseResult = await checkPulseUseCase.Run(input);

        Assert.True(useCaseResult.IsSuccess);
        await checkPulseRepository.Received(1).SaveNewVitalCheck();
    }

    [Fact]
    public async Task ShouldFailWhenVitalsWereNotRetrieved()
    {
        checkPulseRepository
            .RetrieveVitalReadings()
            .Returns(Result.Fail("Could not get vitals for now."));

        var input = "Test message";
        var useCaseResult = await checkPulseUseCase.Run(input);

        Assert.True(useCaseResult.IsFailed);
        await checkPulseRepository.DidNotReceive().SaveNewVitalCheck();
    }

    [Fact]
    public async Task ShouldFailWhenVitalsAreEmpty()
    {
        var emptyVitals = Array.Empty<string>();
        checkPulseRepository
            .RetrieveVitalReadings()
            .Returns(Result.Ok(emptyVitals));

        var input = "Test message";
        var useCaseResult = await checkPulseUseCase.Run(input);

        Assert.True(useCaseResult.IsFailed);
        await checkPulseRepository.DidNotReceive().SaveNewVitalCheck();
    }
}