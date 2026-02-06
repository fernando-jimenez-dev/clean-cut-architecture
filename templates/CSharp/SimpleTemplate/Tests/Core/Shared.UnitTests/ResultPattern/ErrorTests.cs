using Shared.ResultPattern;

namespace Shared.UnitTests.ResultPattern;

public class ErrorTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaults_WhenCausesNull()
    {
        var error = new Error(code: "test.code");

        Assert.Equal("test.code", error.Code);
        Assert.Equal(string.Empty, error.Message);
        Assert.NotNull(error.Causes);
        Assert.Empty(error.Causes);
        Assert.Null(error.Exception);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenCodeInvalid(string invalidCode)
    {
        Action act = () => new Error(code: invalidCode);

        var ex = Assert.Throws<ArgumentException>(act);
        Assert.Equal("code", ex.ParamName);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenMessageNull()
    {
        Action act = () => new Error(code: "test.code", message: null!);

        var ex = Assert.Throws<ArgumentNullException>(act);
        Assert.Equal("message", ex.ParamName);
    }

    [Fact]
    public void Constructor_ShouldCreateDefensiveCopyOfCauses()
    {
        var source = new List<Error> { new LeafError() };
        var error = new RootError(source);
        source.Clear();

        Assert.Single(error.Causes);
        Assert.IsType<LeafError>(error.Causes[0]);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenCausesContainsNull()
    {
        var causes = new List<Error> { new LeafError(), null! };

        Action act = () => new RootError(causes);

        var ex = Assert.Throws<ArgumentException>(act);
        Assert.Equal("causes", ex.ParamName);
    }

    [Fact]
    public void WithException_ShouldReturnNewInstance_WithExceptionSet()
    {
        var ex = new InvalidOperationException("boom");
        var original = new Error(code: "test.code", message: "test message");

        var updated = original.WithException(ex);

        Assert.NotSame(original, updated);
        Assert.Equal(original.Code, updated.Code);
        Assert.Equal(original.Message, updated.Message);
        Assert.Same(ex, updated.Exception);
    }

    [Fact]
    public void WithException_ShouldThrow_WhenExceptionNull()
    {
        var error = new Error(code: "test.code");

        Action act = () => error.WithException(null!);

        var ex = Assert.Throws<ArgumentNullException>(act);
        Assert.Equal("ex", ex.ParamName);
    }

    [Fact]
    public void HasExactAndTryGetExact_ShouldFindErrorsInGraph()
    {
        var leaf = new LeafError();
        var root = new RootError(causes: new List<Error> { leaf });

        Assert.True(root.HasExact<RootError>());
        Assert.True(root.HasExact<LeafError>());
        Assert.False(root.HasExact<OtherError>());

        Assert.True(root.TryGetExact<LeafError>(out var found));
        Assert.Same(leaf, found);

        Assert.False(root.TryGetExact<OtherError>(out var notFound));
        Assert.Null(notFound);
    }

    [Fact]
    public void HasExact_ShouldUseExactTypeMatchingOnly()
    {
        var root = new RootError(causes: new List<Error> { new LeafError() });

        Assert.False(root.HasExact<Error>());
        Assert.False(root.TryGetExact<Error>(out _));
    }

    [Fact]
    public void Clone_ShouldNotReuseCachedIndexFromSource()
    {
        var root = new RootError(causes: new List<Error> { new LeafError() });
        Assert.True(root.TryGetExact<RootError>(out var rootFound));
        Assert.Same(root, rootFound);

        var clone = root with { };
        Assert.True(clone.TryGetExact<RootError>(out var cloneFound));
        Assert.Same(clone, cloneFound);
        Assert.NotSame(rootFound, cloneFound);
    }

    [Fact]
    public void TryGetExact_ShouldReturnFirstEncountered_WhenMultipleSameTypeErrorsExist()
    {
        var first = new DuplicateTypeError("first");
        var second = new DuplicateTypeError("second");
        var root = new RootError(causes: new List<Error> { first, second });

        Assert.True(root.TryGetExact<DuplicateTypeError>(out var found));
        Assert.Same(first, found);
    }

    [Fact]
    public void Error_ShouldExposeOnlyExactLookupApi()
    {
        var methods = typeof(Error).GetMethods()
            .Select(m => m.Name)
            .ToHashSet(StringComparer.Ordinal);

        Assert.Contains(nameof(Error.HasExact), methods);
        Assert.Contains(nameof(Error.TryGetExact), methods);
        Assert.DoesNotContain("Has", methods);
        Assert.DoesNotContain("TryGet", methods);
    }

    [Fact]
    public void HasExactAndTryGetExact_ShouldBeThreadSafe()
    {
        var root = new RootError(causes: new List<Error> { new LeafError(), new OtherError() });
        var failures = new System.Collections.Concurrent.ConcurrentQueue<string>();

        Parallel.For(0, 1_000, index =>
        {
            if (!root.HasExact<LeafError>())
            {
                failures.Enqueue("HasExact<LeafError> returned false.");
            }

            if (!root.TryGetExact<LeafError>(out var found) || found is null)
            {
                failures.Enqueue("TryGetExact<LeafError> failed.");
            }

            if (root.TryGetExact<DuplicateTypeError>(out var _))
            {
                failures.Enqueue("TryGetExact<DuplicateTypeError> unexpectedly succeeded.");
            }
        });

        Assert.Empty(failures);
    }

    private sealed record RootError : Error
    {
        public RootError(IReadOnlyList<Error>? causes = null)
            : base(code: "root", message: "root", causes: causes)
        {
        }
    }

    private sealed record LeafError : Error
    {
        public LeafError(IReadOnlyList<Error>? causes = null)
            : base(code: "leaf", message: "leaf", causes: causes)
        {
        }
    }

    private sealed record OtherError : Error
    {
        public OtherError()
            : base(code: "other", message: "other")
        {
        }
    }

    private sealed record DuplicateTypeError : Error
    {
        public DuplicateTypeError(string codeSuffix)
            : base(code: $"duplicate.{codeSuffix}", message: codeSuffix)
        {
        }
    }
}
