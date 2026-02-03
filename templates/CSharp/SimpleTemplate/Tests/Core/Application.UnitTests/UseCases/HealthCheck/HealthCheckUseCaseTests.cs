using Application.UseCases.HealthCheck;

namespace Application.UnitTests.UseCases.HealthCheck;

public class HealthCheckUseCaseTests
{
    private readonly HealthCheckUseCase _healthCheckUseCase;

    public HealthCheckUseCaseTests()
    {
        _healthCheckUseCase = new HealthCheckUseCase();
    }

    [Fact]
    public async Task ShouldSucceed()
    {
        // Act
        var useCaseResult = await _healthCheckUseCase.Run(CancellationToken.None);

        // Assert
        Assert.True(useCaseResult.IsSuccess);
        Assert.Null(useCaseResult.Error);
    }
}