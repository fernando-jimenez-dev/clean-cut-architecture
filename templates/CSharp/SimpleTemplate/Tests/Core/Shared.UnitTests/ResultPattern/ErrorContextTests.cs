using Shared.ResultPattern;

namespace Shared.UnitTests.ResultPattern;

public class ErrorContextTests
{
    // --- Constructor ---

    [Fact]
    public void Constructor_ShouldSetMessage()
    {
        var context = new ErrorContext("something went wrong");

        Assert.Equal("something went wrong", context.Message);
        Assert.Null(context.Source);
        Assert.Null(context.Inner);
        Assert.Null(context.Exception);
        Assert.Null(context.Metadata);
    }

    [Fact]
    public void Constructor_ShouldAllowInitOnlyProperties()
    {
        var inner = new ErrorContext("inner");
        var exception = new InvalidOperationException("boom");
        var metadata = new Dictionary<string, object> { ["traceId"] = "abc-123" };

        var context = new ErrorContext("outer")
        {
            Source = "PaymentGateway",
            Inner = inner,
            Exception = exception,
            Metadata = metadata
        };

        Assert.Equal("outer", context.Message);
        Assert.Equal("PaymentGateway", context.Source);
        Assert.Same(inner, context.Inner);
        Assert.Same(exception, context.Exception);
        Assert.Same(metadata, context.Metadata);
    }

    // --- FromException ---

    [Fact]
    public void FromException_ShouldUseExceptionMessage_AsMessage()
    {
        var exception = new InvalidOperationException("db connection refused");

        var context = ErrorContext.FromException(exception);

        Assert.Equal("db connection refused", context.Message);
        Assert.Same(exception, context.Exception);
        Assert.Null(context.Source);
    }

    [Fact]
    public void FromException_ShouldSetSource_WhenProvided()
    {
        var exception = new Exception("oops");

        var context = ErrorContext.FromException(exception, source: "UserRepository");

        Assert.Equal("UserRepository", context.Source);
        Assert.Same(exception, context.Exception);
    }

    [Fact]
    public void FromException_ShouldNotSetInnerOrMetadata()
    {
        var context = ErrorContext.FromException(new Exception("x"));

        Assert.Null(context.Inner);
        Assert.Null(context.Metadata);
    }

    // --- Wrap ---

    [Fact]
    public void Wrap_ShouldBuildCausalChain()
    {
        var root = new ErrorContext("dns failed");
        var middle = ErrorContext.Wrap("connection timed out", root);
        var top = ErrorContext.Wrap("database write failed", middle, source: "UserRepository");

        Assert.Equal("database write failed", top.Message);
        Assert.Equal("UserRepository", top.Source);
        Assert.Same(middle, top.Inner);

        Assert.Equal("connection timed out", top.Inner!.Message);
        Assert.Same(root, top.Inner.Inner);

        Assert.Equal("dns failed", top.Inner.Inner!.Message);
        Assert.Null(top.Inner.Inner.Inner);
    }

    [Fact]
    public void Wrap_ShouldNotSetExceptionOrMetadata()
    {
        var inner = new ErrorContext("inner");

        var wrapped = ErrorContext.Wrap("outer", inner);

        Assert.Null(wrapped.Exception);
        Assert.Null(wrapped.Metadata);
    }

    // --- Record semantics ---

    [Fact]
    public void Records_ShouldBeEqual_WhenSameContent()
    {
        var a = new ErrorContext("same") { Source = "X" };
        var b = new ErrorContext("same") { Source = "X" };

        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Records_ShouldNotBeEqual_WhenMessageDiffers()
    {
        var a = new ErrorContext("a");
        var b = new ErrorContext("b");

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Records_WithExpression_ShouldCloneWithOverride()
    {
        var original = new ErrorContext("original") { Source = "A" };

        var modified = original with { Source = "B" };

        Assert.Equal("original", modified.Message);
        Assert.Equal("B", modified.Source);
        Assert.Equal("A", original.Source);
    }
}