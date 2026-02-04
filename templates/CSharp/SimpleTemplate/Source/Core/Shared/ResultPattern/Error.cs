using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Shared.ResultPattern;

public record Error
{
    // Diagnostic-only context (never serialized)
    [JsonIgnore]
    [IgnoreDataMember]
    public Exception? Exception { get; init; }

    public string Code { get; init; }
    public string Message { get; init; }
    public IReadOnlyList<Error> Causes { get; init; }

    public Error(string code, string message = "", IReadOnlyList<Error>? causes = null)
    {
        Code = code;
        Message = message;
        Causes = causes ?? [];
    }

    public Error WithException(Exception ex) => this with { Exception = ex };

    // --------- cheap graph search (lazy, cached, invisible to devs) ---------

    [JsonIgnore]
    [IgnoreDataMember]
    private Index? _index;

    public bool Has<TError>() where TError : Error
        => GetIndex().ByType.ContainsKey(typeof(TError));

    public bool TryGet<TError>(out TError? error) where TError : Error
    {
        if (GetIndex().ByType.TryGetValue(typeof(TError), out var found))
        {
            error = (TError)found;
            return true;
        }

        error = null;
        return false;
    }

    private Index GetIndex()
        => LazyInitializer.EnsureInitialized(ref _index, BuildIndex);

    private Index BuildIndex()
    {
        var byType = new Dictionary<Type, Error>();
        var visited = new HashSet<Error>(ReferenceEqualityComparer.Instance);
        var stack = new Stack<Error>();
        stack.Push(this);

        while (stack.Count > 0)
        {
            var e = stack.Pop();
            if (!visited.Add(e)) continue;

            byType.TryAdd(e.GetType(), e);

            foreach (var c in e.Causes)
                stack.Push(c);
        }

        return new Index(byType);
    }

    private sealed class Index(Dictionary<Type, Error> byType)
    {
        public Dictionary<Type, Error> ByType { get; } = byType;
    }

    private sealed class ReferenceEqualityComparer : IEqualityComparer<Error>
    {
        public static readonly ReferenceEqualityComparer Instance = new();
        public bool Equals(Error? x, Error? y) => ReferenceEquals(x, y);
        public int GetHashCode(Error obj) => RuntimeHelpers.GetHashCode(obj);
    }
}