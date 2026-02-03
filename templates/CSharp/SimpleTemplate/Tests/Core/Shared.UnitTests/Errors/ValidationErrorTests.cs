using FluentValidation.Results;
using Shared.Errors;
using Shared.ResultPattern;

namespace Shared.UnitTests.Errors;

public class ValidationErrorTests
{
    [Fact]
    public void Constructor_WithIssues_ShouldSetCodeMessageAndIssues()
    {
        var issues = new[] { "a", "b" };

        var error = new ValidationError<int>(42, issues);

        Assert.Equal(ValidationError<int>.CodeValue, error.Code);
        Assert.Equal("Value of type Int32 failed validation.", error.Message);
        Assert.Equal(issues, error.Issues);
        Assert.Equal(42, error.Value);
        Assert.Null(error.Exception);
        Assert.Empty(error.Causes);
    }

    [Fact]
    public void Constructor_WithIssuesNull_ShouldUseEmptyIssues()
    {
        IEnumerable<string>? issues = null;
        var error = new ValidationError<string>("x", issues!);

        Assert.Empty(error.Issues);
    }

    [Fact]
    public void Constructor_WithSingleIssue_ShouldWrapIssue()
    {
        var error = new ValidationError<string>("x", "bad input");

        Assert.Single(error.Issues);
        Assert.Equal("bad input", error.Issues[0]);
    }

    [Fact]
    public void Constructor_WithValidationResult_ShouldExtractIssues()
    {
        var validationResult = new ValidationResult(
            new[]
            {
                new ValidationFailure("Name", "Name is required"),
                new ValidationFailure("Age", "Age must be positive"),
            });

        var error = new ValidationError<object>(new object(), validationResult);

        Assert.Equal(2, error.Issues.Count);
        Assert.Equal("Name is required", error.Issues[0]);
        Assert.Equal("Age must be positive", error.Issues[1]);
    }

    [Fact]
    public void Constructor_WithValidationResultNull_ShouldUseEmptyIssues()
    {
        ValidationResult? validationResult = null;
        var error = new ValidationError<object>(new object(), validationResult!);

        Assert.Empty(error.Issues);
    }

    [Fact]
    public void Constructor_WithMessageOverride_ShouldUseProvidedMessage()
    {
        var error = new ValidationError<int>(1, "bad", message: "custom");

        Assert.Equal("custom", error.Message);
    }

    [Fact]
    public void ValidationErrors_ForOverloads_ShouldCreateError()
    {
        var viaIssues = ValidationErrors.For(5, new[] { "x" });
        var viaIssue = ValidationErrors.For(5, "y");
        var viaResult = ValidationErrors.For(5, new ValidationResult());

        Assert.Equal(ValidationError<int>.CodeValue, viaIssues.Code);
        Assert.Equal(ValidationError<int>.CodeValue, viaIssue.Code);
        Assert.Equal(ValidationError<int>.CodeValue, viaResult.Code);

        Assert.Single(viaIssues.Issues);
        Assert.Equal("x", viaIssues.Issues[0]);

        Assert.Single(viaIssue.Issues);
        Assert.Equal("y", viaIssue.Issues[0]);

        Assert.Empty(viaResult.Issues);
    }

    [Fact]
    public void ValidationError_ShouldBeUsableViaBaseType()
    {
        ValidationError error = new ValidationError<string>("x", "bad");

        Assert.Equal(ValidationError<string>.CodeValue, error.Code);
        Assert.Single(error.Issues);
        Assert.Equal("bad", error.Issues[0]);
    }
}
