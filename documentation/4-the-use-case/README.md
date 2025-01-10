# **Chapter IV: The Use Case**

### Table of Contents

1. [What is a Use Case in CCA?](#1-what-is-a-use-case-in-cca)
2. [The Anatomy of a Use Case](#2-the-anatomy-of-a-use-case)
3. [Use Cases in Action](#3-use-cases-in-action)

At the heart of Clean Cut Architecture lies the **Use Case**. This is the central piece of your system, where business logic thrives and decisions are made. Think of a Use Case as the core that dictates **what your application does**—a self-contained unit of purpose, driving clarity and functionality.

## 1. **What is a Use Case in CCA?**

In CCA, a Use Case represents a distinct operation or workflow in your application.

- Each Use Case is **laser-focused** on one task.
- It knows nothing about the outside world, like APIs or databases.
- It communicates exclusively through well-defined inputs and outputs.

Whether it’s **finalizing an order** or **processing payment**, a Use Case encapsulates everything needed to perform its job—cleanly, predictably, and independently.

## 2. **The Anatomy of a Use Case**

Let’s zoom in on what a Use Case looks like internally.

<p align="center"><img src="images/use-case-details.png" alt="use-case-details" width="40%"/></p>
<p align="center"><i>Use Case Details: Visualizing the internal structure of a Use Case.</i></p>

A Use Case is like an inverted cone, where the layers flow from the top-level logic down to the technical details:

1. **Domain Layer:** Holds business rules, aggregates, and entities.
2. **Abstraction Layer:** Interfaces and contracts connecting the Domain to the Infrastructure.
3. **Infrastructure Layer:** The implementation details, like repositories or external services.

Each layer exists for a reason: to **keep your business logic pure and testable.**

## 3. **Use Cases in Action**

Use Cases are the lifeblood of the Application boundary. They:

- Take input from the Presentation boundary (e.g., an HTTP request).
- Perform business logic, often leveraging Shared utilities.
- Return output for the Presentation boundary to handle.

### **Example 1: Processing a Paid Order**

Not all workflows are request-response. Some are event-driven, like processing a paid order.

<p align="center"><img src="images/process-paid-order-use-case.png" alt="process-paid-order-use-case-graphic" width="75%"/></p>
<p align="center"><i>Process Paid Order Use Case: An event-driven example of a Use Case workflow.</i></p>

Here’s what happens:

- A microservice publishes a **PaidOrder event**, which is caught by a **bus event handler** (Presentation boundary).
- The event handler transforms the data and forwards it to the **Process Paid Order Use Case**.
- The Use Case operates similarly to the previous example but may use domain aggregates and interact with Shared utilities like ORM contexts or logging.

### **Example 2: Finalizing an Order**

Let’s break down another example, a use case that could very well be related to the previous one.

<p align="center"><img src="images/finalize-order-use-case.png" alt="finalize-order-use-case-graphic" width="75%"/></p>
<p align="center"><i>Finalize Order Use Case: Demonstrates an HTTP-driven Use Case workflow.</i></p>

Imagine a mobile app sends a request to finalize an order:

- The **Presentation boundary** transforms the request into an input DTO and forwards it to the **Finalize Order Use Case**.
- The Use Case performs its logic, possibly interacting with:
  - A **repository** for database operations.
  - A **service bus** to notify other systems.
- The output DTO flows back to the Presentation boundary, which sends a 201 Created response to the app.

### **Why Use Cases Matter**

Without Use Cases, your application risks turning into a tangle of scattered logic. With them, you gain:

- **Clarity:** Each Use Case serves a single, clear purpose.
- **Reusability:** Boundaries around the Use Case keep it adaptable.
- **Testability:** Use Cases can be tested in isolation, without worrying about dependencies like databases or APIs.

The Use Case is where Clean Cut Architecture shines. It’s not just a phylosophy; it’s a practice—one that makes your application easier to understand, maintain, and grow.

<br/>
<br/>

[Next Chapter: Questions and Answers](../5-questions-and-answers/README.md)

---

[Previous Chapter: Structure of Clean Cut Architecture](../3-structure-of-cca/README.md)

---
