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
- Operations return Result or Result<T>, never exceptions.
- Errors are first-class domain objects, not strings.
- Errors can aggregate multiple causes.
- Exceptions are treated as diagnostic context, not control flow.
- Error handling is explicit and testable.

This allows failures to be:
- intentional
- observable
- handled consistently at boundaries

### Boundary-Only Responsibility
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
Output: Result.Success() or Result.Failure(...).

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
