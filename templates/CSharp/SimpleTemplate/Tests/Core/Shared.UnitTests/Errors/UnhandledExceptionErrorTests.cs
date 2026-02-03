using Shared.Errors;
using Shared.ResultPattern;

namespace Shared.UnitTests.Errors;

public class UnhandledExceptionErrorTests
{
    [Fact]
    public void Constructor_ShouldSetDefaults()
    {
        var ex = new InvalidOperationException("boom");

        var error = new UnhandledExceptionError(ex);

        Assert.Equal("app.unhandled-exception", error.Code);
        Assert.Equal("An unhandled exception occurred.", error.Message);
        Assert.Same(ex, error.Exception);
        Assert.Empty(error.Causes);
    }

    [Fact]
    public void Constructor_ShouldUseCustomMessageAndCauses()
    {
        var ex = new Exception("boom");
        var causes = new[] { new Error(code: "cause", message: "cause") };

        var error = new UnhandledExceptionError(ex, message: "custom", causes: causes);

        Assert.Equal("custom", error.Message);
        Assert.Same(causes, error.Causes);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenExceptionNull()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new UnhandledExceptionError(null!));

        Assert.Equal("exception", ex.ParamName);
    }
}
