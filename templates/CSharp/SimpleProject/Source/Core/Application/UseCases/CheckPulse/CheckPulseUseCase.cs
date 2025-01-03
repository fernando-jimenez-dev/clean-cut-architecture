using Application.UseCases.CheckPulse.Abstractions;
using Application.UseCases.CheckPulse.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.CheckPulse;

/// <summary>
/// A simple use case to verify the application's operational state.
/// Use this class as a reference to start implementing your very own use cases.
/// </summary>
/// <remarks>
/// This template serves as a baseline for implementing basic "pulse check"
/// within the system. It logs a confirmation message along with the
/// provided input and returns a successful result.
/// </remarks>
public class CheckPulseUseCase : ICheckPulseUseCase
{
    private readonly ILogger<CheckPulseUseCase> logger;
    private readonly ICheckPulseRepository checkPulseRepository;

    public CheckPulseUseCase(ILogger<CheckPulseUseCase> logger, ICheckPulseRepository checkPulseRepository)
    {
        this.logger = logger;
        this.checkPulseRepository = checkPulseRepository;
    }

    public async Task<CheckPulseUseCaseOutput> Run(string input, CancellationToken cancellationToken = default)
    {
        var vitalReadings = await checkPulseRepository.RetrieveVitalReadings(cancellationToken);
        if (vitalReadings.Length > 0)
        {
            logger.LogInformation("We are up and running! Here is your input: {}", input);
            await checkPulseRepository.SaveNewVitalCheck();
            return new CheckPulseUseCaseOutput(IsSuccess: true);
        }

        logger.LogError("We could not find any vitals.");
        return new CheckPulseUseCaseOutput(IsSuccess: false, ErrorMessage: "Vitals were empty!");
    }
}