<h1 align="center">Clean Cut Architecture</h1>
<h3 align="center">Documentation</h3>
<p align="center">0.1.2-alpha</p>

---

> [!WARNING]
> This documentation is a **work in progress** and currently in its **alpha** stage (`0.1.2-alpha`). Expect incomplete sections, evolving content, and potential changes in structure. We are actively working on completing the core chapters and refining the examples. Feedback and suggestions are welcome as we refine the material.

Welcome to the **Documentation Hub** for Clean Cut Architecture! This directory is the go-to place for understanding the principles, practices, and guidelines that form the foundation of CCA.

# Chapters

1. [Introduction](#i-introduction) - The Birth of Clean Cut Architecture
2. [Clean Cut Architecture](2-clean-cut-architecture/README.md) - The Philosophical Foundation
3. [Structure of CCA](3-structure-of-cca/README.md) - The Under the Hood
4. [The Use Case](4-the-use-case/README.md) - The Building Blocks
5. [Questions & Answers](5-questions-and-answers/README.md) - Answering the Big Questions

# I. Introduction

We've all been there. You start a new project, full of energy. Clean code, thorough documentation, all the right patterns, maybe even a shiny new library you've been itching to try. You ship it to production, feeling proud, all is neat and organized. Then, a few months later, a bug report comes in for a different project—one you have touched not too long ago. You open the code… and it's like looking at someone else's work. It's a mess. You barely recognize your own code. "What does this even do?" you ask yourself. How could something I wrote recently now feel so alien?

Although it sounds extreme, I know it has happened to me before. I remember one time, wrestling with a particularly messy codebase, when I stumbled upon Clean Architecture by Robert C. Martin (Uncle Bob). It was validating to see that others were grappling with the same challenges and that there was a structured way of thinking about these problems. Clean Architecture showed me the importance of decoupling business logic from external systems and organizing code to "scream intent." This revelation marked the beginning of a journey—a quest to develop architectures that prioritize clarity, maintainability, and developer sanity.

As I explored other paradigms, Vertical Slice Architecture by Jimmy Bogard provided another key piece of the puzzle: organizing code by feature rather than by technical layers. This approach complemented Clean Architecture's principles and offered new ways to keep codebases cohesive and focused.

These experiences, along with countless hours spent refining messy projects and building new ones, led me to gradually develop Clean Cut Architecture (CCA). Clean Cut Architecture draws inspiration from Clean Architecture and Vertical Slice Architecture, inheriting their strongest principles: decoupling, testability, and feature-focused organization. But CCA is neither of them. It introduces its own set of practices to address practical developer concerns that often arise during real-world projects.

Clean Cut Architecture is built on the idea of "screaming intent"—making the code's purpose obvious at a glance—and high cohesion, keeping related code together. It discourages premature sharing of components, instead advocating for isolating logic within use cases until sharing becomes truly necessary. CCA also emphasizes developer ergonomics, encouraging folder structures and practices that feel intuitive, so it's clear where things should go and why—not just for you today, but for anyone (including future you) who has to work on it tomorrow.

Most importantly, CCA doesn't stop at theory. This documentation goes beyond abstract principles, offering concrete code examples and templates to bring the architecture to life. You'll find step-by-step demonstrations of how to implement its principles in real-world scenarios, making it easier to bridge the gap between understanding the concepts and applying them effectively in your projects.

Whether you're battling an unwieldy legacy system or starting a fresh project, CCA offers a practical framework designed to evolve with your needs. It's about combining the best of Clean Architecture and Vertical Slice Architecture while introducing fresh ideas to help developers deliver clean, maintainable, and scalable software.

[Next Chapter: Clean Cut Architecture](./2-clean-cut-architecture/README.md)
