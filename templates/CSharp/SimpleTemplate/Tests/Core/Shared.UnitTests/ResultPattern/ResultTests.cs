using Shared.ResultPattern;

namespace Shared.UnitTests.ResultPattern;

public class ResultTests
{
    private sealed record TestError(string Reason);

    // --- Factory: Success ---

    [Fact]
    public void Success_ShouldCreateSuccessfulResult_WithValue()
    {
        var result = Result<int, TestError>.Success(42);

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(42, result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Success_ShouldThrow_WhenValueIsNull()
    {
        var ex = Assert.Throws<ArgumentNullException>(
            () => Result<string, TestError>.Success(null!));

        Assert.Equal("value", ex.ParamName);
    }

    [Fact]
    public void Success_ShouldAcceptUnit()
    {
        var result = Result<Unit, TestError>.Success(default);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Error);
    }

    // --- Factory: Failure ---

    [Fact]
    public void Failure_ShouldCreateFailedResult_WithError()
    {
        var error = new TestError("oops");

        var result = Result<int, TestError>.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Same(error, result.Error);
        Assert.Equal(0, result.Value); // default(int)
    }

    [Fact]
    public void Failure_ShouldThrow_WhenErrorIsNull()
    {
        var ex = Assert.Throws<ArgumentNullException>(
            () => Result<int, TestError>.Failure(null!));

        Assert.Equal("error", ex.ParamName);
    }

    // --- Consumption: Succeeded ---

    [Fact]
    public void Succeeded_ShouldReturnTrue_OnSuccess()
    {
        var result = Result<int, TestError>.Success(7);

        Assert.True(result.Succeeded());
    }

    [Fact]
    public void Succeeded_ShouldReturnFalse_OnFailure()
    {
        var result = Result<int, TestError>.Failure(new TestError("x"));

        Assert.False(result.Succeeded());
    }

    [Fact]
    public void Succeeded_WithOut_ShouldOutputValue_OnSuccess()
    {
        var result = Result<string, TestError>.Success("hello");

        Assert.True(result.Succeeded(out var value));
        Assert.Equal("hello", value);
    }

    [Fact]
    public void Succeeded_WithOut_ShouldOutputDefault_OnFailure()
    {
        var result = Result<string, TestError>.Failure(new TestError("x"));

        Assert.False(result.Succeeded(out var value));
        Assert.Null(value);
    }

    // --- Consumption: Failed ---

    [Fact]
    public void Failed_ShouldReturnTrue_OnFailure()
    {
        var result = Result<int, TestError>.Failure(new TestError("x"));

        Assert.True(result.Failed());
    }

    [Fact]
    public void Failed_ShouldReturnFalse_OnSuccess()
    {
        var result = Result<int, TestError>.Success(1);

        Assert.False(result.Failed());
    }

    [Fact]
    public void Failed_WithOut_ShouldOutputError_OnFailure()
    {
        var error = new TestError("kaboom");
        var result = Result<int, TestError>.Failure(error);

        Assert.True(result.Failed(out var captured));
        Assert.Same(error, captured);
    }

    [Fact]
    public void Failed_WithOut_ShouldOutputNull_OnSuccess()
    {
        var result = Result<int, TestError>.Success(1);

        Assert.False(result.Failed(out var captured));
        Assert.Null(captured);
    }

    // --- Implicit conversions ---

    [Fact]
    public void ImplicitConversion_FromValue_ShouldWrapAsSuccess()
    {
        Result<int, TestError> result = 99;

        Assert.True(result.IsSuccess);
        Assert.Equal(99, result.Value);
    }

    [Fact]
    public void ImplicitConversion_FromError_ShouldWrapAsFailure()
    {
        var error = new TestError("nope");

        Result<int, TestError> result = error;

        Assert.True(result.IsFailure);
        Assert.Same(error, result.Error);
    }

    // --- Match (Func) ---

    [Fact]
    public void Match_Func_ShouldInvokeSuccessBranch_OnSuccess()
    {
        var result = Result<int, TestError>.Success(10);

        var output = result.Match(
            success: v => $"ok:{v}",
            failure: e => $"fail:{e.Reason}");

        Assert.Equal("ok:10", output);
    }

    [Fact]
    public void Match_Func_ShouldInvokeFailureBranch_OnFailure()
    {
        var result = Result<int, TestError>.Failure(new TestError("bad"));

        var output = result.Match(
            success: v => $"ok:{v}",
            failure: e => $"fail:{e.Reason}");

        Assert.Equal("fail:bad", output);
    }

    // --- Match (Action) ---

    [Fact]
    public void Match_Action_ShouldInvokeSuccessAction_OnSuccess()
    {
        var result = Result<int, TestError>.Success(5);
        var successCalled = false;
        var failureCalled = false;

        result.Match(
            success: _ => successCalled = true,
            failure: _ => failureCalled = true);

        Assert.True(successCalled);
        Assert.False(failureCalled);
    }

    [Fact]
    public void Match_Action_ShouldInvokeFailureAction_OnFailure()
    {
        var result = Result<int, TestError>.Failure(new TestError("x"));
        var successCalled = false;
        var failureCalled = false;

        result.Match(
            success: _ => successCalled = true,
            failure: _ => failureCalled = true);

        Assert.False(successCalled);
        Assert.True(failureCalled);
    }

    // --- Map ---

    [Fact]
    public void Map_ShouldTransformValue_OnSuccess()
    {
        var result = Result<int, TestError>.Success(3);

        var mapped = result.Map(v => v * 2);

        Assert.True(mapped.IsSuccess);
        Assert.Equal(6, mapped.Value);
    }

    [Fact]
    public void Map_ShouldPassErrorThrough_OnFailure()
    {
        var error = new TestError("untouched");
        var result = Result<int, TestError>.Failure(error);
        var transformCalled = false;

        var mapped = result.Map(v => { transformCalled = true; return v * 2; });

        Assert.True(mapped.IsFailure);
        Assert.Same(error, mapped.Error);
        Assert.False(transformCalled);
    }

    // --- Then ---

    [Fact]
    public void Then_ShouldChain_OnSuccess()
    {
        var result = Result<int, TestError>.Success(4);

        var chained = result.Then(v => Result<string, TestError>.Success($"val:{v}"));

        Assert.True(chained.IsSuccess);
        Assert.Equal("val:4", chained.Value);
    }

    [Fact]
    public void Then_ShouldShortCircuit_OnFailure()
    {
        var error = new TestError("stop");
        var result = Result<int, TestError>.Failure(error);
        var nextCalled = false;

        var chained = result.Then(v =>
        {
            nextCalled = true;
            return Result<string, TestError>.Success("never");
        });

        Assert.True(chained.IsFailure);
        Assert.Same(error, chained.Error);
        Assert.False(nextCalled);
    }

    [Fact]
    public void Then_ShouldPropagateFailure_FromNextStep()
    {
        var result = Result<int, TestError>.Success(1);
        var downstreamError = new TestError("downstream");

        var chained = result.Then<string>(v => downstreamError);

        Assert.True(chained.IsFailure);
        Assert.Same(downstreamError, chained.Error);
    }

    // --- Static factory: Result.Ok / Result.Fail ---

    [Fact]
    public void StaticOk_NoArgs_ShouldReturnDefaultUnit()
    {
        var unit = Result.Ok();

        Assert.Equal(default, unit);
    }

    [Fact]
    public void StaticOk_WithErrorType_ShouldReturnUnitSuccess()
    {
        var result = Result.Ok<TestError>();

        Assert.True(result.IsSuccess);
        Assert.IsType<Result<Unit, TestError>>(result);
    }

    [Fact]
    public void StaticOk_WithDataAndErrorType_ShouldReturnDataSuccess()
    {
        var result = Result.Ok<int, TestError>(123);

        Assert.True(result.IsSuccess);
        Assert.Equal(123, result.Value);
    }

    [Fact]
    public void StaticFail_WithErrorType_ShouldReturnUnitFailure()
    {
        var error = new TestError("boom");

        var result = Result.Fail<TestError>(error);

        Assert.True(result.IsFailure);
        Assert.Same(error, result.Error);
    }

    [Fact]
    public void StaticFail_WithDataAndErrorType_ShouldReturnTypedFailure()
    {
        var error = new TestError("boom");

        var result = Result.Fail<int, TestError>(error);

        Assert.True(result.IsFailure);
        Assert.Same(error, result.Error);
    }
}