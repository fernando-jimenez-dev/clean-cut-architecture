namespace Application.UseCases.CheckPulse;

public record CheckPulseUseCaseOutput(bool IsSuccess, string? ErrorMessage = null) { }