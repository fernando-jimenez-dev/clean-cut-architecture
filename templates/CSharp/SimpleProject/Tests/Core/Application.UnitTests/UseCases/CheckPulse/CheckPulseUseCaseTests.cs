using Application.UseCases.CheckPulse;
using Application.UseCases.CheckPulse.Infrastructure;
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
            .Returns(goodVitals);

        var input = "Test message";
        var useCaseOutput = await checkPulseUseCase.Run(input);

        Assert.True(useCaseOutput.IsSuccess);
        await checkPulseRepository.Received(1).SaveNewVitalCheck();
    }

    [Fact]
    public async Task ShouldFailWhenVitalsAreEmpty()
    {
        var emptyVitals = Array.Empty<string>();
        checkPulseRepository
            .RetrieveVitalReadings()
            .Returns(emptyVitals);

        var input = "Test message";
        var useCaseOutput = await checkPulseUseCase.Run(input);

        Assert.False(useCaseOutput.IsSuccess);
        await checkPulseRepository.DidNotReceive().SaveNewVitalCheck();
    }
}