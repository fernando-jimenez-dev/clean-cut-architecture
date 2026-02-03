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
    public void HasAndTryGet_ShouldFindErrorsInGraph()
    {
        var leaf = new LeafError();
        var root = new RootError(causes: new List<Error> { leaf });

        Assert.True(root.Has<RootError>());
        Assert.True(root.Has<LeafError>());
        Assert.False(root.Has<OtherError>());

        Assert.True(root.TryGet<LeafError>(out var found));
        Assert.Same(leaf, found);

        Assert.False(root.TryGet<OtherError>(out var notFound));
        Assert.Null(notFound);
    }

    [Fact]
    public void HasAndTryGet_ShouldHandleCycles()
    {
        var causes = new List<Error>();
        var root = new RootError(causes);
        var leaf = new LeafError(causes);
        causes.Add(leaf);
        causes.Add(root);

        Assert.True(root.Has<LeafError>());
        Assert.True(root.TryGet<LeafError>(out var found));
        Assert.Same(leaf, found);
    }

    [Fact]
    public void NestedRecordMembers_ShouldBeExercised()
    {
        var causes = new List<Error>();
        var root = new RootError(causes);
        var leaf = new LeafError(causes);
        var other = new OtherError();

        Assert.Same(causes, root.causes);
        Assert.Same(causes, leaf.causes);

        var updatedCauses = new List<Error> { other };
        var rootClone = root with { causes = updatedCauses };
        var leafClone = leaf with { causes = updatedCauses };
        var otherClone = other with { };

        Assert.Same(updatedCauses, rootClone.causes);
        Assert.Same(updatedCauses, leafClone.causes);
        Assert.NotSame(other, otherClone);
        Assert.Equal(other.Code, otherClone.Code);
    }

    private sealed record RootError(IReadOnlyList<Error>? causes = null)
        : Error(code: "root", message: "root", causes: causes);

    private sealed record LeafError(IReadOnlyList<Error>? causes = null)
        : Error(code: "leaf", message: "leaf", causes: causes);

    private sealed record OtherError()
        : Error(code: "other", message: "other");
}
