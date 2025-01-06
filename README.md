<h1 align="center">Clean Cut Architecture</h1>
<h3 align="center">Slicing Use Cases with Intent</h3>

<p align="center">
   Let's build something clean, simple and effective—one slice at the time!
   <br/>
   <strong><i><u>0.1.2-alpha</u></i></strong>
</p>

Clean Cut Architecture (CCA) is an evolving approach that blends the principles of **Clean Architecture** and **Vertical Slice Architecture** to achieve **clarity**, **simplicity**, and a focus on **business intent**.

This repository serves as both a **knowledge base** (docs) and a collection of **practical templates and examples** for implementing CCA in your projects.

## Why Clean Cut

Over my years as a .NET engineer, I’ve been inspired by the structured approach of Clean Architecture and the simplicity of Vertical Slice Architecture. I often found myself blending the two approaches into something that worked better for my projects—something I now call **Clean Cut Architecture**.

<p align="center">
   <img src="/clean-cut-architecture-graphic.png" width="85%">
</p>

Clean Cut Architecture aims to:

- Make the **business purpose of your code immediately obvious** (Screaming Architecture).
- Focus on **use case-driven development**.
- Provide **cohesive slices** that bring together domain logic, infrastructure, and presentation.
- Balance **simplicity** and **scalability**, allowing systems to grow without unnecessary complexity.

## Architecture Workflow

Clean Cut Architecture aims to standarize the system's workflow. This workflow keeps the focus on the Use Case, with it being the entry point into the Application's business rules and orchestrating the necessary components to accomplish it's goal. Every use case will use different specially tailored components for the use case itself. These components can be of many types - including but not limited to Domain, Abstraction and Infrastructure.

<p align="center">
   <img src="/architecture-workflow.png" width="85%">
</p>

Here is how the flow is structured:

#### Consumers

External consumers, such as microservices, mobile applications, or frontend clients, initiate interactions with the system. These consumers communicate through HTTP endpoints, message brokers, or other defined interfaces.

#### Presentation Component

- Acts as the entry point for all incoming requests.
- Responsible for routing the requests to the appropriate Application Layer components.
- Handles communication protocols like HTTP and message queues, ensuring inputs and outputs are properly formatted.

#### Use Case ⭐

- Serves as the orchestrator for business logic execution.
- Represents a distinct **feature** or **action** that the application supports, modeled as a verb (e.g., "Process Order", "Finalize Payment", "Send Notification").
- Each use case encapsulates the application's behavior in a modular, testable, and reusable way, and operates independently, making it easy to test in isolation.
- It is responsible for managing all operations necessary to fullfil its purpose. It is **highly cohesive** and keeps all the related components together.

#### Domain

- Encapsulates core business rules and domain entities.
- Aims to support cases where Domain Logic is complex and heavy in behavior.
- Remains isolated from infrastructure or external dependencies to ensure pure, reusable logic.

#### Abstraction

- Separates the Use Case and Domain logics from infrastructure and shared components. This approach helps **invert responsabilities** and having our Use Cases not depend on Concrete Components.
- Provides interfaces for interacting with infrastructure components like databases, services, or external APIs.
- Defines contracts (e.g., repositories, gateways) that the Application Layer depends upon.

#### Infrastructure

- Implements the contracts defined in the abstraction layer by the that requires them.
- Responsible for direct interactions with databases, external systems, email connectors, or message brokers.
- Provides the actual functionality for storing data, sending messages, or interacting with external APIs.

#### Shared

- Contains cross-cutting concerns and reusable components such as error types, service bus abstractions, and shared DTOs.
- Ensures consistency and reduces duplication across the system.
- Components living here must be reusable by nature, and more than one use case requires access to it. Avoid temptation on putting everything here upfront - rather prefer to organize your components inside the Use Case **first** and then look for patterns that suggest a specific component is **indeed duplicated** among use cases.

## What is in this Repo?

### Documentation

The `/docs` folder contains:

- An **overview** of Clean Cut Architecture principles.
- A guide on how to structure your projects using CCA.
- Comparisons with Clean and Vertical Slice Architecture.

### Templates and Examples

The `/templates` folder provides a **starter project template** with basic CCA setup.
The `/examples` folder provides **real-world like examples** with different flavors of CCA implemented under different circumstances.

## Getting Started

### Step 1: Clone the Repository

```bash
git clone https://github.com/fernando-jimenez-dev/clean-cut-architecture.git
```

### Step 2: Explore the Docs

Head to the `/docs` folder to understand the core principles and how to get started with Clean Cut Architecture.

### Step 3: Try the Templates

Check out the `/templates` folder for project setups you can adapt to your own use cases.

## Roadmap

- ☑️ Set up repository and first ReadMe.
- ☐ Initial documentation on principles and concepts.
- ☑️ Add a starter project template.
- ☐ Provide more real-world examples.
- ☐ Create guides for integrating CCA into existing systems.
- ☐ Open the project for community feedback and contributions.

## Contributing

For now, this repository is a **work in progress**, but feel free to open issues for suggestions or ideas. Contributions will be welcomed in the future.

---

<h6 align="center">0.1.2-alpha</h6>
