using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Shared.ResultPattern;

/// <summary>
/// Represents a modeled failure with a typed error graph.
/// </summary>
/// <remarks>
/// Type lookups are exact by default.
/// For duplicate runtime types in the graph, the first node encountered in <see cref="Causes"/> order wins.
/// </remarks>
public record Error
{
    // Diagnostic-only context (never serialized)
    [JsonIgnore]
    [IgnoreDataMember]
    public Exception? Exception { get; init; }

    public string Code { get; }
    public string Message { get; }
    public IReadOnlyList<Error> Causes => _causes;

    [JsonIgnore]
    [IgnoreDataMember]
    private readonly FrozenErrorList _causes;

    // --------- lazy graph index (per-instance, clone-safe) ---------

    [JsonIgnore]
    [IgnoreDataMember]
    private Index? _index;

    public Error(string code, string message = "", IReadOnlyList<Error>? causes = null)
    {
        Code = string.IsNullOrWhiteSpace(code)
            ? throw new ArgumentException("Error code must be non-empty.", nameof(code))
            : code;
        Message = message ?? throw new ArgumentNullException(nameof(message));
        _causes = FrozenErrorList.Create(causes);
    }

    // Copy constructor used by record cloning (`with`) so caches are never shared across instances.
    protected Error(Error original)
    {
        ArgumentNullException.ThrowIfNull(original);

        Exception = original.Exception;
        Code = original.Code;
        Message = original.Message;
        _causes = original._causes;
        _index = null;
    }

    public Error WithException(Exception ex)
    {
        ArgumentNullException.ThrowIfNull(ex);
        return this with { Exception = ex };
    }

    public bool HasExact<TError>() where TError : Error
        => GetIndex().ByType.ContainsKey(typeof(TError));

    public bool TryGetExact<TError>([NotNullWhen(true)] out TError? error) where TError : Error
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
            var error = stack.Pop();
            if (!visited.Add(error))
            {
                continue;
            }

            byType.TryAdd(error.GetType(), error);
            error._causes.PushAllTo(stack);
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

    private sealed class FrozenErrorList : IReadOnlyList<Error>
    {
        private readonly Error[] _items;

        public static FrozenErrorList Empty { get; } = new([]);

        private FrozenErrorList(Error[] items)
        {
            _items = items;
        }

        public int Count => _items.Length;

        public Error this[int index] => _items[index];

        public static FrozenErrorList Create(IReadOnlyList<Error>? causes)
        {
            if (causes is null || causes.Count == 0)
            {
                return Empty;
            }

            if (causes is FrozenErrorList frozen)
            {
                return frozen;
            }

            var count = causes.Count;
            var items = new Error[count];
            for (var i = 0; i < count; i++)
            {
                items[i] = causes[i] ?? throw new ArgumentException(
                    "Causes cannot contain null entries.",
                    nameof(causes));
            }

            return new FrozenErrorList(items);
        }

        public void PushAllTo(Stack<Error> stack)
        {
            // Reverse push so pop-order matches original Causes order.
            for (var i = _items.Length - 1; i >= 0; i--)
            {
                stack.Push(_items[i]);
            }
        }

        public IEnumerator<Error> GetEnumerator()
            => ((IEnumerable<Error>)_items).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _items.GetEnumerator();
    }
}
