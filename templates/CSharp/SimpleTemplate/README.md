# Clean Cut Architecture - Simple .NET Template

This repository provides a minimal, opinionated .NET template built around Clean Cut Architecture (CCA).

It is designed to help you start new projects with:
- clear boundaries
- explicit intent
- predictable failure handling
- zero architectural guesswork

The template is intentionally small. It gives you just enough structure to build real systems without forcing a framework mindset.

## What This Template Gives You

### Clear Architectural Intent
The folder structure is designed to scream intent.
You should be able to understand what the system does by reading the solution layout alone.

Use cases, boundaries, and infrastructure are separated explicitly, not implicitly.

### Use-Case-Driven Design
Business logic lives in use cases, not controllers, not services, not helpers.

Use cases:
- receive input
- execute application logic
- return a Result
- never throw

### Explicit Result and Error Model
The template includes a Result / Error model with the following guarantees:
- Operations return `Result<TValue, TError>`, never exceptions.
- Errors are first-class types, not strings or codes.
- The error's type *is* its identity. Consumers pattern-match on the type.
- Diagnostic detail (exceptions, source, causal chains, metadata) lives in an optional `ErrorContext`, separate from the error's identity.
- Identity drives control flow. Context drives observability. They travel together but never blur.
- Error handling is explicit, exhaustive, and testable.

When multiple errors with the same runtime type exist in a causes graph,
lookup returns the first encountered error in causes order.

This allows failures to be:
- intentional
- observable
- handled consistently at boundaries

## The Result API

The Result types live in `Source/Core/Shared/ResultPattern/`. This section is the reference for what they are and how to use them.

### `Result<TValue, TError>`
The container that every Use Case (and every operation that can fail) returns.

- `TValue` — what comes back on success. Use `Unit` (see below) when there's nothing meaningful to return.
- `TError` — the error type on failure. The Result type only constrains it to `class`; the Use Case contract adds `IContextualError` on top.

It carries either `Value` (on success) or `Error` (on failure), never both. `IsSuccess` / `IsFailure` and the `Succeeded` / `Failed` methods inspect which state it's in.

### `Unit`
A zero-size struct that represents "no value." Used as the success type for operations that don't produce data:

```csharp
public async Task<Result<Unit, DeleteUserError>> Run(Guid userId, CancellationToken ct) { ... }
```

There is exactly one `Unit` value: `default(Unit)`. You rarely need to write it explicitly — the factories below handle it.

### Creating a Result

**Implicit conversion** is the preferred shape inside Use Case bodies. Return a value directly for success, an error directly for failure:

```csharp
public async Task<Result<CreatedUser, CreateUserError>> Run(CreateUserInput input, CancellationToken ct)
{
    if (await _users.EmailExists(input.Email, ct))
        return new EmailAlreadyExists(input.Email);   // TError → Result (failure)

    var user = await _users.Create(input, ct);
    return new CreatedUser(user.Id, user.Email);      // TValue → Result (success)
}
```

No `Result.Success(...)` / `Result.Failure(...)` wrapping. The compiler does it.

**Explicit factory** for cases where the implicit conversion can't infer the types — primarily test setup and mock returns. The static `Result` class has:

| Call | What it returns |
|---|---|
| `Result.Ok()` | A `Unit` value. In an async method returning `Task<Result<Unit, TError>>`, it converts automatically. |
| `Result.Ok<TError>()` | A `Result<Unit, TError>` directly. Use when the implicit chain can't fire (non-async methods, mock setup). |
| `Result.Ok<TData, TError>(data)` | A `Result<TData, TError>` success. |
| `Result.Fail<TError>(error)` | A `Result<Unit, TError>` failure. |
| `Result.Fail<TData, TError>(error)` | A `Result<TData, TError>` failure. |

Typical async Use Case success:
```csharp
public async Task<Result<Unit, HealthCheckError>> Run(CancellationToken ct = default)
{
    await _probe.PingAsync(ct);
    return Result.Ok();
}
```

Typical test mock setup:
```csharp
_useCase.Run(default).Returns(Result.Ok<HealthCheckError>());
_useCase.Run(default).Returns(Result<Unit, HealthCheckError>.Failure(new ServiceUnreachable("Database")));
```

### Consuming a Result

Two styles, depending on what you're doing.

**`Succeeded` / `Failed` with `out`** — the common shape in endpoints:

```csharp
var result = await _useCase.Run(ct);

if (result.Succeeded(out var user))
    return Ok(new UserResponse(user.Id, user.Email));

if (result.Failed(out var error))
    return error switch
    {
        EmailAlreadyExists e => Conflict(e.Email),
        PasswordTooWeak p    => BadRequest(p.Reason),
        _                    => StatusCode(500)
    };
```

**Functional combinators** for chaining operations that produce Results:

| Method | Behavior |
|---|---|
| `Match(success, failure)` | Applies one of two functions depending on outcome. Forces exhaustive handling and returns a unified type. |
| `Map(transform)` | Transforms the success value. Passes failures through unchanged. |
| `Then(next)` | Chains an operation that itself returns a Result. Short-circuits on failure. |

Example chaining:
```csharp
return await _users.FindById(userId, ct)
    .Then(user => _permissions.Check(user, ct))
    .Map(user => new UserDto(user.Id, user.Email));
```

### `IContextualError`
The contract every Use Case error must satisfy. It declares one property:

```csharp
public interface IContextualError
{
    ErrorContext? Context { get; }
}
```

`IUseCase<TError>` constrains `TError : class, IContextualError`, so the compiler guarantees that any error returned from a Use Case can carry diagnostic context. Components *outside* Use Cases (repositories, HTTP clients, utilities) can use `Result<TValue, TError>` with any error class — the interface only binds at the Use Case boundary.

### `ErrorContext`
The "why did it fail?" diagnostic layer. Optional. Never used for control flow.

Properties:
- `Message` — human-readable description.
- `Source` — the component or layer that produced the error (e.g., `"UserRepository"`).
- `Inner` — an inner `ErrorContext` forming a causal chain.
- `Exception` — the original exception, if one was caught.
- `Metadata` — arbitrary structured key/value pairs (correlation IDs, timing, request IDs).

Two factory helpers cover the common cases:
- `ErrorContext.FromException(ex, source: "...")` — wrap a caught exception.
- `ErrorContext.Wrap(message, inner, source: "...")` — wrap an existing context with a new layer of meaning.

**Read by**: logging middleware, observability decorators, error filters. **Never by Presentation.** Presentation switches on the error type (Identity); it logs `error.Context?.Message` for diagnostics but never branches on it.

### Declaring a Use Case's Errors

Each Use Case owns its own sealed family of error types. The shape:

```csharp
// Errors/CreateUserError.cs — the family root
public abstract record CreateUserError(ErrorContext? Context = null) : IContextualError;

// Errors/EmailAlreadyExists.cs
public sealed record EmailAlreadyExists(string Email, ErrorContext? Context = null)
    : CreateUserError(Context);

// Errors/PasswordTooWeak.cs
public sealed record PasswordTooWeak(string Reason, ErrorContext? Context = null)
    : CreateUserError(Context);

// Errors/UnhandledException.cs
public sealed record UnhandledException(Exception Exception, ErrorContext? Context = null)
    : CreateUserError(Context);
```

The root is an `abstract record` so it can't be instantiated and so it satisfies `IContextualError` once for the whole family (records auto-generate the property from the primary constructor parameter). The variants are `sealed record`s carrying only the data Presentation needs to respond.

`HealthCheck` in this template is laid out exactly this way — see [Source/Core/Application/UseCases/HealthCheck/Errors/](Source/Core/Application/UseCases/HealthCheck/Errors).

### When to Attach an `ErrorContext`

Attach Context when there's diagnostic value worth capturing. Skip it when the error type and its fields already say everything.

- `EmailAlreadyExists("foo@bar.com")` — usually no Context. Self-explanatory.
- `ServiceUnreachable("Database", Context: ErrorContext.FromException(ex, source: "OrdersRepository"))` — Context is essential. The on-call engineer needs the exception, the source, and any inner causes.
- `PaymentRejected("3501", Context: ErrorContext.FromException(gatewayEx))` — Context carries the gateway's response details that Presentation doesn't need but operations do.

Rule of thumb: if a future debugger reading the log would need it, attach it. If they wouldn't, don't.

### The Use Case as Translator

A Use Case body is the one place where infrastructure exceptions become typed errors. Anywhere the Use Case calls out to fallible infrastructure, it catches what it knows how to classify and translates anything else into `UnhandledException`:

```csharp
try
{
    await _db.Insert(user, ct);
}
catch (UniqueViolationException ex)
{
    return new EmailAlreadyExists(input.Email, ErrorContext.FromException(ex, source: "UsersTable"));
}
catch (DbConnectionException ex)
{
    return new ServiceUnreachable("UsersDatabase", ErrorContext.FromException(ex));
}
catch (Exception ex)
{
    return new UnhandledException(ex, ErrorContext.FromException(ex));
}
```

Infrastructure exceptions never leak out of a Use Case as the error type. The Use Case decides what each failure *means* in its own vocabulary, and the original exception lives in `Context` for observability.


Endpoints (or other entry points) are responsible for:
- invoking use cases
- translating Result into protocol-specific responses (HTTP, messaging, etc.)
- logging failures appropriately

Use cases do not know about HTTP, logging, tracing, or transport concerns.

## Example Use Case: Health Check
The template includes a small example use case to demonstrate the full flow end-to-end.

### HealthCheckUseCase
Purpose: Verifies that the application is operational.
Input: None.
Output: `Result<Unit, HealthCheckError>` — success carries no payload, failure carries one of the variants in `HealthCheckError`.

This use case exists purely as a reference and can be safely removed.

### HealthCheck Endpoint
Exposes the use case via an HTTP endpoint.

Demonstrates:
- how to handle successful results
- how to log and translate failures
- how to distinguish between:
execution anomalies (exceptions wrapped in errors),
and errors that are simply not mapped by the endpoint yet.

## How to Use This Template

1. Explore the Structure
Start by browsing the solution folders.
Pay attention to where logic lives and where it does not.
If something feels "missing", that's intentional.

2. Replace the Example Use Case
Delete the HealthCheck use case and endpoint, or keep them as reference.
Create your own use cases following the same pattern:
input -> execution -> Result.

3. Model Failures Explicitly
Define error types that represent known failure scenarios in your domain.
If you catch an exception you did not plan for, wrap it in the designated exception-based error and return it as a failure result.
Seeing those errors in logs is a signal to extend your error model, not to ignore them.

4. Extend Infrastructure Deliberately
Add databases, messaging, external APIs, and other infrastructure concerns behind clear boundaries.
Infrastructure should adapt to your use cases - never the other way around.

## Using This Repository as a .NET Template (Optional)
You can install this repository as a reusable .NET template.

### Install the Template
From the repository root:
```bash
dotnet new install . --force
```

### Create a New Solution
Navigate to the directory where you want your new solution and run:
```bash
dotnet new cca -n <YourSolutionName>
```

Or, using the full name:
```bash
dotnet new clean-cut-architecture -n <YourSolutionName>
```
