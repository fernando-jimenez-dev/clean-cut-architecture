# **Chapter III - Structure of Clean Cut Architecture**

### Table of Contents

1. [The Three Boundaries](#1-the-three-boundaries)
2. [The Flow of Data and Dependencies](#2-the-flow-of-data-and-dependencies)

Imagine building a puzzle: each piece has a defined shape, purpose, and place, yet all must fit together seamlessly to complete the picture. That's how Clean Cut Architecture (CCA) works—but instead of cardboard pieces, we're assembling boundaries, components, and workflows.

At its core, the structure of CCA isn't just a set of rules; it's a blueprint for creating software systems that are simple, intuitive, maintainable, and adaptable. Let's break it down piece by piece.

## 1. **The Two Boundaries and their Shared Concerns**

In Chapter II - Clean Cut Architecture, we established that the architecture revolves around **two primary boundaries:** Application and Presentation, alongside **Shared Concerns**. The latter is not truly a boundary but rather a set of components that operate accross and support the boundaries. Let's break this concepts down further.

### **The Application Boundary**

The Application boundary is organized into Use Cases, each divided further into three types of components:

- **Domain:** Represents the business core—entities, aggregates, or domain rules.
- **Abstraction:** Defines contracts and interfaces for external dependencies.
- **Infrastructure:** Real-world implementations of those abstractions-APIs, databases, external systems, etc.

But here's the kicker: **not every Use Case needs all three.**

#### **Tailoring the Components to Fit the Use Case**

Let's say you're building five microservices:

- Four of them handle heavy business logic and benefit from Domain-Driven Design principles.
- The fifth is different-it generates PDF reports and emails them to the stakeholders.

While the first four need robust Domain components to handle their complexity, the fifth service doesn't. Its focus is on orchestrating Infrastructure (e.g., fetching data, formatting reports). For this service, a fully fleshed-out Domain layer might be unnecessary overhead. Instead, it might consist of 20 Use Cases, each generating a unique report, without ever touching complex Domain rules.

#### **The Flexibility of the Application Boundary**

The Application Boundary shines because it adapts to the purpose of your software:

- **Lightweight Use Cases:** Reporting systems, ETLs, and email generators often focus more on Infrastructure orchestration than on Domain logic.
- **Rich Use Cases:** ERP modules like accounting, HR, and payments thrive on well-defined Domain layers. Their Use Cases naturally encapsulate more business logic.

This flexibility ensures that each Use Case has exactly what it needs—no more, no less.

#### **Purpose, Not Complexity**

At its core, the Application Boundary represents what your software does. Its Use Cases are the practical answers to that question:

- Need to assign a user to a new hire? That's a Use Case.
- Need to deactivate a user? Another Use Case.
- Resetting a user's password? You guessed it—a Use Case.

Each Use Case defines a focused, encapsulated unit of functionality. Together, they shape the application's purpose. And when all your applications follow the same Clean Cut principles, you gain more than consistency—you gain clarity. Changing one system doesn't mean untangling a web of scattered logic. It means making meaningful, business-driven adjustments.

#### **Making the Code Speak for Itself**

Imagine you open a project and see a directory called `/use-cases`. Inside, you find `AssignUserToNewHire`, `DeactivateUser`, and `ResetUserPassword`. Just from these names, you already have a strong sense of the system's purpose. This isn't just a "user manager"; it's a tool supporting IT in managing employee accounts.

- Assigned a bug in `DeactivateUser`? You know exactly where to start.
- Reviewing a Pull Request? If the fix touches multiple Use Cases when only one is relevant, you'll notice immediately.

This structure lets developers infer functionality and focus better, enhancing productivity and reducing frustration.

#### **A Preview of What's to Come**

We'll dive deeper into Use Cases in the next chapter, but here's the takeaway: organizing software around Use Cases doesn't just clarify what it does. It also makes how it does it easier to follow.

Within a Use Case, you'll see components working together, each with its own responsibility, each driving the Use Case forward. It's a step-by-step journey toward the purpose, with no detours into unrelated logic.

---

### **The Presentation Boundary**

The Presentation Boundary is the translator of your system. It's the part responsible for interpreting the outside world's requests and passing them to the Application Boundary in a format it understands, then packaging the Application's response into something the outside world can use.

It might take the form of:

- **API Controllers** interpreting HTTP requests and crafting HTTP responses.
- **Message Handlers** consuming events from a service bus.
- **CLI Tools translating** typed commands into executable instructions.

Its role is to provide **flexibility and adaptability**, enabling the Application Boundary to remain focused on business logic, unburdened by the specifics of input and output mechanisms.

#### **Flexibility for Diverse Inputs**

Let's imagine the **natural evolution** of a software system:

1. **The Starting Point – REST API**

   Your team builds a new system, and you start with a simple REST API. It's easy to implement, and clients can trigger use cases by sending HTTP requests. Everything works perfectly.

2. **The Next Step – SOAP Support**

   A new requirement emerges: your clients want SOAP instead of REST because their legacy systems can only communicate through SOAP. While this isn't ideal for you, it's critical for them.

   So, you extend your Presentation Boundary by adding a SOAP API. But you don't rewrite your Application—why would you? The existing use cases remain the same, and you simply translate the SOAP input into the same format your Application already understands.

   Now, your system has two faces: a REST API for modern clients and a SOAP API for legacy clients, both seamlessly working with the same Application logic.

3. **Adapting to Events – Message Queues**

   Fast forward a few months. A different team in your organization has a broadcasting system that publishes **real-time notifications** to a service bus. To stay in sync, your system needs to listen to these notifications and trigger corresponding use cases.

   This isn't something your REST or SOAP APIs can handle—they weren't designed for this kind of communication. But you don't panic. Instead, you implement a new **Message Handler** within your Presentation Boundary, subscribing to the service bus and translating the incoming messages into the same format your Application already uses.

At each stage, your Presentation Boundary evolves, adapting to new input mechanisms. This flexibility allows your Application to stay clean and focused while your system continues to meet growing demands.

#### **Adapting to Modern Needs**

Imagine your system doesn't just need to support new **input mechanisms**, but also new **communication patterns**. It's no longer just about APIs or service buses. Consider this scenario:

- Your system needs to **connect with legacy monoliths** via file exchange because they can't support direct API calls.
- It also needs to support **mobile apps** that require lightweight, low-latency communication.
- On top of that, you're tasked with integrating a **real-time dashboard** to display live metrics using WebSockets.

Without a well-defined Presentation Boundary, the Application would become a tangled mess of protocols, adapters, and logic. But with the Presentation Boundary in place, you can introduce each mechanism incrementally:

- Add a **file processor** to handle file-based inputs.
- Implement a **WebSocket server** to stream live updates.
- Integrate lightweight REST endpoints tailored for mobile apps.

By keeping these responsibilities in the Presentation Boundary, your Application remains decoupled from the **how** of communication, allowing it to focus solely on the **what**—its core business logic.

#### **Beyond Input and Output**

The Presentation Boundary also takes on several **non-functional responsibilities** essential for system operation.

In **C#**, for example, the Presentation layer is often where you:

- Configure **Dependency Injection** for service lifetimes.
- Translate configuration files (`appsettings.json`) into structured objects.
- Manage **CORS rules**, **port configuration**, and even **DevOps artifacts** like `Dockerfile` definitions.

These are critical, but they don't belong in the Application. Why should the Application know how many Docker volumes exist or whether a firewall is blocking a specific port? These responsibilities live squarely in the Presentation layer, leaving the Application focused on its true purpose: delivering business value.

#### **Why the Separation Matters**

By keeping the Presentation and Application Boundaries distinct, you:

- Avoid mixing responsibilities, which helps **maintain clarity and focus**.
- Enable the system to **adapt** to new input mechanisms or protocols without disrupting core logic.
- Reduce the cost and risk of adding features or integrations.

The result? A system that's easier to extend, maintain, and debug. Presentation is the face of your system, and keeping it flexible ensures your Application can thrive in an ever-changing landscape.

### **Shared Concerns**

**Shared Concerns** aren't a boundary in themselves; instead, they are **reusable components** that span across or within boundaries. Their purpose is to provide **common functionality** while respecting the independence of the Application and Presentation boundaries. Mismanaging them can lead to tangled dependencies, so it's crucial to handle them with care.

Let's explore their role in two contexts: **Boundary-Level Sharing** and **Cross-Boundary Sharing**.

#### **Boundary-Level Sharing**

Each boundary—Application and Presentation—may have its own `/shared` directory containing components that support functionality within that boundary.

- **In the Application Boundary:**
  Shared components here often revolve around **business logic** and abstractions. Examples include:
  - **ORM Contexts (e.g., `DbContext` in .NET):** These enable multiple Use Cases to access the same database without duplicating connection logic. For example, a Reporting use case and an Order Management Use Case may both rely on a shared ORM Context.
  - **Abstractions (e.g., `IUseCase`):** A shared interface like `IUseCase` can define a contract for all Use Cases. This abstraction allows you to introduce **decorators** or **middleware** for cross-cutting concerns like logging, input validation, or performance monitoring. These decorators would live in `/shared`.
  - **Input/Output Models:** Standardizing input and output DTOs across multiple Use Cases can simplify data flow and testing while maintaining clarity.
- **In the Presentation Boundary:**
  Shared concerns here depend heavily on the type of Presentation project. Examples include:
  - **UI Projects:** Shared CSS styles, layout components, or generic web components (e.g., a reusable date picker or modal dialog) can enhance consistency across pages.
  - **API Projects:** Common middleware (e.g., for authentication, logging, or exception handling) and shared DTOs (e.g., a standard response format for all endpoints) are common examples.
  - **Messaging Projects:** Shared configurations for service bus connections or retry policies ensure consistency across messaging handlers.

#### **Cross-Boundary Sharing**

Some shared concerns are critical for **communication between Application and Presentation**. These components need to be carefully managed to maintain clear boundaries while enabling effective interaction.

- **Error Types:**
  Each Use Case may define specific errors relevant to its logic. The Presentation boundary needs access to these errors for:
  - Returning consistent API error responses.
  - Logging errors for operational insights.
  - Displaying meaningful error messages in a UI.
  - Shared exception types, for example, help ensure that all parts of the system interpret and handle errors in the same way.
- **Application Abstractions:**
  The Application boundary might define contracts for external dependencies, such as configuration providers or system clocks. For example:
  - A Use Case might depend on a configuration value (e.g., a tax rate or API key) through an abstraction.
  - The Presentation boundary fulfills this contract by pulling actual values from configuration files like `appsettings.json`.

These shared components allow for smooth collaboration between boundaries while ensuring that each retains its focus and independence.

#### **Purposeful Accessibility**

Shared components often rely on accessibility modifiers (**public, private, protected, internal**) to control their visibility and usage. Mismanaging accessibility can lead to tangled dependencies. For example:

- **Publicly Accessible Components:** Use these only for objects that truly need cross-boundary sharing, such as error types or abstractions explicitly designed for reuse.
- **Internally Scoped Components:** Keep components private to a specific Use Case or internal to a boundary whenever possible.

The goal is to strike a balance: shared concerns should be reusable and flexible without sacrificing the clarity and independence of boundaries.

#### **The Golden Rule of Shared Concerns**

Shared concerns are here to **support boundaries**—not dictate them. They're powerful tools for improving efficiency and consistency but must be used judiciously.

When done right, shared concerns:

- Prevent duplication of code and effort.
- Maintain clear separation of responsibilities.
- Enhance system flexibility and scalability.

But when overused or poorly managed, they can lead to dependencies that are hard to untangle, reducing clarity and increasing maintenance overhead.

#### **Evolving Into Shared Components**

One of the most effective ways to identify what belongs in the shared zone is by observing **natural duplication patterns** in your code.

For instance, a pattern I've noticed is this: before deciding to move a component into a shared location, I've often copied and pasted that component a couple of times. This seemingly mundane act is actually very revealing:

- **Heavy Modifications Post-Copy:**
  If you find yourself copying the component and then heavily modifying it to fit a specific context, it's a sign that the component **isn't truly generic**. In these cases, it may not belong in a shared zone at all. Instead, it might be better to tailor it for each specific Use Case or boundary.
- **Minimal or No Modifications Post-Copy:**
  If you copy the component, reuse it, and find little to no changes are necessary, that's a strong indicator of shared potential. You're starting to see **duplicated code**, which signals an opportunity to abstract the component and move it into the `/shared` directory.

This process is a natural evolution of shared components:

1. **Start locally** within a boundary or Use Case.
2. **Observe duplication patterns** as your system grows.
3. Refactor and relocate only when you've identified a **clear and consistent need for reuse**.

This iterative approach helps you avoid prematurely moving components into shared zones, which can lead to over-engineering or misplaced abstractions. By letting usage patterns guide your decisions, you ensure that shared components genuinely support your system's growth instead of creating unnecessary complexity.

#### **The Danger of Misplaced Similarity**

Be mindful, however, that just because two components look the same doesn't mean they are the same. It's crucial to evaluate not just their appearance but their purpose and context.

For instance, consider validation logic for two Use Cases: adding a new user and updating an existing user. The validation rules might look almost identical, but they serve distinct purposes. Placing them in the shared zone might seem efficient, but it undermines the conceptual boundaries between the Use Cases.

Key takeaway: Similarity in implementation does not equate to similarity in concept. Shared components should represent universal concerns, **not just coincidental overlaps in functionality**. Ensuring this distinction preserves the clarity and independence of each boundary.

## **2. The Flow of Data and Dependencies**

Clean Cut Architecture (CCA) enforces a one-way dependency rule: **data flows inward**, while **decisions flow outward**. This ensures that dependencies remain predictable and aligned with the core purpose of the system.

### **How It Flows**

1. **The Presentation Boundary Receives Requests**

   The Presentation boundary serves as the system's entry point, whether it's an HTTP API, a CLI, or a message queue subscriber. Its primary job is to receive raw input from external consumers, such as:

   - HTTP requests from a REST API.
   - Commands from a CLI tool.
   - Events from a service bus or message queue.

   These requests are often in formats specific to the communication mechanism (e.g., JSON for REST, XML for SOAP). The Presentation boundary doesn't act on these directly. Instead, it **validates, sanitizes, and transforms** them into a format understood by the Application boundary.

   **Example:**

   Imagine a REST API receives a POST request with JSON data to create a new user:

   ```json
   {
     "name": "John Doe",
     "email": "john.doe@example.com"
   }
   ```

   The Presentation boundary would validate this data (e.g., ensuring email is in a valid format) and translate it into a request object like CreateUserRequest for the Application boundary.

2. **Application Boundary Processes run the Use Case**

   The Application boundary is the core of the system. Once the sanitized and transformed input reaches here, the system leverages its **Domain logic, Abstractions, and Infrastructure** to fulfill the use case.

   - **Domain Logic:** Applies the core rules and business logic (e.g., ensuring the email isn't already in use).
   - **Abstractions:** Interacts with external systems (e.g., sending a verification email via a `IMailService` interface).
   - **Infrastructure:** Provides the actual implementations of these abstractions (e.g., an SMTP client to send the email).

   **Example:**

   For the `CreateUser` use case, the Application boundary might:

   - Check if the user already exists in the database (via an abstraction).
   - Persist the new user data to the database.
   - Enqueue a message in the service bus to notify other systems of the new user creation.

3. **Results Flow Back to the Presentation Boundary**

   Once the Application boundary completes the use case, it sends the results (or errors) back to the Presentation boundary. This result is often a plain object or DTO that represents the outcome.

   The Presentation boundary then formats the result for the external consumer:

   - Converts internal error codes into HTTP status codes for REST APIs (e.g., 201 Created or 409 Conflict).
   - Converts the output into JSON, XML, or other required formats.
   - Logs responses or sends them to external monitoring systems.

   **Example:**

   The Application boundary returns a `CreateUserResult` object, and the Presentation boundary translates it into a 201 HTTP response with a JSON body:

   ```json
   {
     "id": 123,
     "name": "John Doe"
   }
   ```

4. **Decisions Flow Outward**

   While data flows inward, **decisions**—derived from the Application boundary—flow outward. These decisions often manifest as actions like notifying external systems, updating a UI, or triggering workflows.

---

### **Putting It All Together**

Here's where the _Architecture Workflow_ graphic comes into play:

<p align="center"><img src="images/generic-architecture-workflow.png" alt="architecture-workflow" width="75%"/></p>

The diagram visualizes how data flows step-by-step, moving from Presentation to Application to Shared concerns and back. It reinforces the idea that each boundary has a specific role, and dependencies never flow backward.

### **Why This Structure Works**

The structure of CCA isn't arbitrary—it's focused on **purpose** to address real-world software development challenges:

- **Simplicity:**

  - Clear roles for each boundary make the system easier to reason about.
  - When something breaks (e.g., a failed API call or a broken use case), you know where to look.

- **Flexibility:**

  - Boundaries are loosely coupled. Changing one (e.g., swapping a REST API with GraphQL or changing the database technology) doesn't ripple through the system.
  - Adding new input/output mechanisms becomes simpler—just extend the Presentation boundary without impacting the Application boundary.

- **Maintainability:**
  - Smaller, well-defined components are easier to test, debug, and evolve.
  - Encapsulation ensures each piece can evolve independently, reducing unintended side effects.

### **Extending the Example: Real-World Systems**

Imagine a real-world system like an e-commerce platform:

1. A user submits an order via a web app (Presentation).
2. The Application boundary validates the order, calculates totals, and reserves stock (Domain logic).
3. Payment is processed via an external payment gateway (Abstraction and Infrastructure).
4. The Presentation boundary formats a confirmation page for the user and triggers an email notification.

Now extend this system over time. Initially, it may only handle web orders via REST. But as the business grows:

- A CLI tool is added for customer support agents.
- Event listeners are added to process refunds from a separate financial system.

Because CCA enforces one-way dependencies, these additions don't disrupt existing logic—they extend it, keeping the system **cohesive, scalable, and maintainable**.

## 3. **Cohesion accross Boundaries**

Balancing strong boundaries with seamless cohesion is one of the foundations of Clean Cut Architecture. We want the **independence** of boundaries, but also the **unity** of a well-orchestrated system.

So how do we accomplish this? By ensuring both the **Presentation** and **Application** boundaries **scream their intent in harmony**. This doesn't mean they're indistinguishable—it means they're clearly **collaborating**. They may reside in separate spaces, but their organization should make it obvious how they support the same goals.

### **Screaming Intent: Why and How**

One of the most effective ways to achieve this is by **organizing your codebase around Use Cases**. Each boundary should explicitly show how its components support specific Use Cases.

**Why is this important?**

1. **Clarity:** When you look at your system, you should instantly understand which components serve which purpose.
2. **Maintainability:** If you need to update or debug a specific Use Case, you know exactly where to look.
3. **Scalability:** Adding a new Use Case or feature becomes straightforward.

### **Directory Structure for Cohesion**

Let's start with a **before** scenario, which is commonly seen in many projects:

```plaintext
/user-manager-system
  /application
  |  /shared
  |  /use-cases
  |    /assign-new-user
  |    /deactivate-user
  |    /reset-password
  /presentation
  |  /controllers
  |     user.controller
  |  /middlewares
  |  /helpers
  |  /validators
  |  /dtos
```

At first glance, this structure seems reasonable. The Application boundary has a clear organization around Use Cases. However, the Presentation boundary is a bit of a **mystery**. You can see its various components (e.g., controllers, middlewares, etc.), but you have no idea how these relate to specific Use Cases without diving deep into the code.

Now let's look at an **improved structure**:

```plaintext
/user-manager-system
  /application
  |  /shared
  |  /use-cases
  |    /assign-new-user
  |    /deactivate-user
  |    /reset-password
  /presentation
  |  /shared
  |    /middlewares
  |    /dtos
  |    /helpers
  |  /use-cases
  |    /assign-new-user
  |      assign-new-user.controller
  |      assign-new-user.validator
  |    /deactivate-user
  |      deactivate-user.controller
  |    /reset-password
  |      reset-password.controller
```

This structure does something extraordinary: **it makes intent obvious.**

- The Application boundary screams its intent through its `/use-cases` directory.
- The Presentation boundary echoes this by organizing its components under corresponding `/use-cases`.

If you need to understand or modify the `Deactivate User` Use Case, you know exactly where to go:

```plaintext
- /application/use-cases/deactivate-user
- /presentation/use-cases/deactivate-user
```

**Every file and folder is aligned, showing the system's intent in a way that's instantly understandable.**

### **Practical Benefits**

This organization achieves the elusive balance between independence and cohesion:

1. **Clear Mapping of Features:**

   Each Use Case is easily traceable across boundaries. If a new developer joins the team and is tasked with fixing something in `Reset Password`, they don't have to hunt through the entire Presentation or Application codebase. They can go straight to the relevant directories.

2. **Parallel Development:**

   Teams can work on the same Use Case across boundaries without stepping on each other's toes. For example:

   - The backend team can focus on `/application/use-cases/deactivate-user`.
   - The frontend team can work on `/presentation/use-cases/deactivate-user`.

3. **Future-Proofing:**

   When you add a new boundary (e.g., a CLI interface or event listeners), you already have a pattern to follow. Just add a `/use-cases` directory in the new boundary, and the cohesion remains intact.

### **Beyond Screaming Intent: Harmonizing Components**

The directory structure is just the starting point. True cohesion also involves aligning the **contracts** and **formats** between boundaries:

1. **Shared DTOs and Contracts:**

   Use shared data transfer objects (DTOs) or contract definitions to ensure consistency between boundaries. For example, if the Application boundary returns an error code, the Presentation boundary should understand how to format it for the external consumer.

2. **Aligned Error Handling:**

   Define a standard error-handling mechanism that both boundaries adhere to. For instance:

   - Application boundary returns a standardized error object.
   - Presentation boundary maps this to an HTTP status code or UI error message.

3. **Consistent Logging:**

   Ensure that logging is harmonized across boundaries to provide a unified picture of what's happening in the system.

### **A Final Thought: Cohesion Is Collaboration**

Cohesion across boundaries doesn't mean blurring the lines between them. Instead, it means designing those lines with care. Each boundary remains independent, but they work together seamlessly to achieve the system's goals.

With a clear structure that screams intent, you can focus less on **navigating complexity** and more on **building features, solving problems, and delivering value**.

CCA's structure isn't just a limitation—it's a guide. A guide that empowers you to create systems that are **easy to understand, maintain, and extend**, no matter how complex the requirements become.

---

The structure of Clean Cut Architecture isn't a limitation—it's a guide. It gives you the freedom to focus on building features, solving problems, and writing clean, efficient code. With a clear structure in place, you can stop worrying about the “how” and start focusing on the “what.”

<br/>

[Next Chapter: The Use Case](../4-the-use-case/README.md)

---

[Previous Chapter: Clean Cut Architecture](../2-clean-cut-architecture/README.md)
