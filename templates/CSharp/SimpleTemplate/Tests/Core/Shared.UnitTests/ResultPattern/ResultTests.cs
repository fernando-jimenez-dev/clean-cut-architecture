using Shared.ResultPattern;

namespace Shared.UnitTests.ResultPattern;

public class ResultTests
{
    [Fact]
    public void Result_Success_ShouldHaveSuccessState()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.True(result.Succeeded());
        Assert.False(result.Failed());

        Assert.False(result.Failed(out var error));
        Assert.Null(error);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Result_Failure_ShouldHaveFailureState()
    {
        var error = new Error(code: "test.error", message: "failed");

        var result = Result.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.False(result.Succeeded());
        Assert.True(result.Failed());

        Assert.True(result.Failed(out var found));
        Assert.Same(error, found);
        Assert.Same(error, result.Error);
    }

    [Fact]
    public void Result_Failure_ShouldThrow_WhenErrorNull()
    {
        Action act = static () => Result.Failure(null!);

        var ex = Assert.Throws<ArgumentNullException>(act);

        Assert.Equal("error", ex.ParamName);
    }

    [Fact]
    public void Result_GenericSuccess_ShouldReturnValueAndHasValue()
    {
        var result = Result.Success(42);

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.True(result.Succeeded());
        Assert.Equal(42, result.Value);
        Assert.True(result.HasValue);
        Assert.Null(result.Error);

        Assert.True(result.Succeeded(out var value));
        Assert.Equal(42, value);
        Assert.True(result.TryGetValue(out var strictValue));
        Assert.Equal(42, strictValue);
        Assert.False(result.Failed());
    }

    [Fact]
    public void Result_GenericSuccess_WithNoValue_ShouldHaveNoValue()
    {
        var result = Result<int?>.Success();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Null(result.Value);
        Assert.False(result.HasValue);

        Assert.True(result.Succeeded(out var value));
        Assert.Null(value);
        Assert.False(result.TryGetValue(out var strictValue));
        Assert.Null(strictValue);
        Assert.False(result.Failed());
    }

    [Fact]
    public void Result_GenericFailure_ShouldHaveFailureState()
    {
        var error = new Error(code: "test.error", message: "failed");

        var result = Result.Failure<int>(error);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.False(result.HasValue);
        Assert.Same(error, result.Error);

        Assert.True(result.Failed(out var found));
        Assert.Same(error, found);
        Assert.False(result.Succeeded());
        Assert.False(result.TryGetValue(out var value));
        Assert.Equal(default, value);
    }

    [Fact]
    public void Result_GenericSuccess_ShouldThrow_WhenValueNull()
    {
        Action act = static () => Result<string>.Success(null!);

        var ex = Assert.Throws<ArgumentNullException>(act);

        Assert.Equal("value", ex.ParamName);
    }

    [Fact]
    public void Result_GenericFailure_ShouldThrow_WhenErrorNull()
    {
        Action act = static () => Result<string>.Failure(null!);

        var ex = Assert.Throws<ArgumentNullException>(act);

        Assert.Equal("error", ex.ParamName);
    }

    [Fact]
    public void Result_WithExpression_ShouldClone()
    {
        var original = Result.Success();

        var clone = original with { };

        Assert.NotSame(original, clone);
        Assert.True(clone.IsSuccess);
        Assert.Null(clone.Error);
    }
}
