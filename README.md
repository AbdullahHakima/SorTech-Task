# GeoGuard API

A high-performance, stateless .NET Core Web API for managing country-based IP blocking. Built with thread-safe in-memory collections and external geolocation integration.

## Problem Definition
The objective of this assignment was to create a robust API capable of managing a blocked countries list and validating incoming caller IP addresses against this list. The strict constraints were to bypass traditional databases entirely, relying exclusively on **thread-safe in-memory storage**, and to orchestrate requests to a third-party geolocation API to look up country codes. Furthermore, the system needed to audit all access attempts and support both permanent and temporal blocks gracefully.

## The Solution
To meet these requirements without over-engineering, the solution leverages **Clean Architecture** patterns mixed with pragmatic design choices. It uses `ConcurrentDictionary` and `ConcurrentQueue` for thread-safe in-memory data management, guaranteeing data integrity across simultaneous API requests without the overhead of manual locking mechanisms. The core logic handles early exits, caching strategies for domains, and cleanly separates concerns without introducing unnecessary layers.

## Architecture & Design
To demonstrate .NET engineering principles, this solution avoids "Primitive Obsession" and "Spaghetti Code" by strictly adhering to **Clean Architecture** and **Domain-Driven Design (DDD)** concepts:

*   **GeoGuard.Domain (Includes Application Layer):** The pure heart of the application. It contains highly encapsulated Entities (`BlockedCountry`, `BlockedAttemptLog`) with private setters and static factory methods. We utilized **Value Objects** (like `CountryCode` and native `System.Net.IPAddress`) to mathematically guarantee valid state within our domain rules (e.g., enforcing strict 2-character ISO Alpha-2 codes) before data ever reaches a service. **Note:** A dedicated Application layer (like MediatR or App Services) was deliberately omitted to keep the project lightweight. In this setup, the Domain layer encompasses application orchestration logic because the use cases are straightforward enough that splitting them would just create unnecessary abstraction and plumbing.
*   **GeoGuard.Infrastructure:** Strictly handles I/O, external HTTP connections, and data persistence. Handled the database constraint elegantly by implementing `IBlockedCountryRepository` using a `ConcurrentDictionary`, and `IBlockedAttemptRepository` using a `ConcurrentQueue`. This guarantees thread-safety across API requests without the overhead of manual locking.
*   **GeoGuard.Api:** The presentation layer. Controllers are kept intentionally "dumb", acting purely as traffic cops. All domain instantiation and HTTP response abstractions are delegated to Domain/Application Services.

## Architecture Trade-offs & Choices
*   **No AutoMapper or FluentValidation:** These libraries were deliberately excluded because the project is relatively simple. Introducing them would be overkill, adding unnecessary overhead and dependencies for simple mappings and validations that can easily be handled via constructors, value objects, and extension methods.
*   **Domain as Application Layer:** As mentioned, merging the Application orchestration logic directly into the Domain boundary reduced boilerplate plumbing, which is ideal for a focused microservice-style application.
*   **In-Memory Collections vs. Database:** Utilizing `ConcurrentDictionary` and `ConcurrentQueue` allows extreme read/write speeds, trading off persistence (data lost on restart) for strict compliance with the assignment constraints.
*   **Extensibility:** Although simplistic for this assignment, the bracket is open to add more details. The clean decoupling ensures that replacing the in-memory repositories with a real database (like PostgreSQL/Redis) or moving to a full CQRS pattern in the future requires minimal changes.

## Project Structure

```text
SorTech-Task/
├── GeoGuard.Api/                  # Presentation Layer (Outer)
│   ├── Controllers/               # HTTP Endpoints (Countries, IpLookup, Logs)
│   ├── Program.cs                 # Composition Root & DI Setup
|   ├── Extentions/                # ResultExtenstion for globalize response
│   └── appsettings.json           # Configurations & API Keys
├── GeoGuard.Infrastructure/       # I/O and External Integrations Layer
│   ├── Repositories/              # Thread-Safe In-Memory DB Implementations
│   |── Services/                  # External HTTP Clients (IpGeolocationService)
|   ├── Exceptions/                # Custom Infra. Exceptions
├── GeoGuard.Domain/               # Core Business Rules (Inner)
│   ├── Entities/                  # Encapsulated Models (BlockedCountry, BlockedAttemptLog)
│   ├── ValueObjects/              # Strongly Typed primitives (CountryCode)
│   ├── Interfaces/                # Contracts for Repositories & External Services
│   ├── Common/                    # Shared Patterns (Result<T>, PagedResult<T>)
│   ├── Exceptions/                # Custom Domain Exceptions
│   └── Services/                  # Application Use-Cases & Orchestration Logic
└── GeoGuard.Tests/                # Unit Tests & Verification
    ├── BlockVerificationServiceTests.cs  # Core process flow and IP lookups
    ├── CountryManagementServiceTests.cs  # Thread-safe dictionary limits and behavior
    └── DomainValueObjectsTests.cs        # Strongly-typed rules (e.g. ISO Country Codes)
```

##  Key Solutions & Engineering Patterns
*   **Result<T> Pattern:** Implemented across repositories and services to entirely eliminate costly exception-throwing for control flow, standardizing success/failure states seamlessly into HTTP responses.
*   **Early-Exit Orchestration:** The `BlockVerificationService` handles complex conditional logic (verifying IP, checking country block, validating temporal expiration, and safely logging the attempt) utilizing strict early-exit patterns for flawless, readable control flow.
*   **Unified Expiration Strategy:** By design, the `ExpirationTime` on blocks is a `DateTime?`. A `null` value conceptually acts as a "Permanent" block, while a populated value acts as a "Temporal" block, allowing an elegant unified storage mechanism without duplicating entities.
*   **Resilient HTTP Client:** Utilizes `IHttpClientFactory` instead of manual `HttpClient` instantiation to prevent socket exhaustion and effectively manage TCP connections during high-load API geolocation lookups.
*   **Rich Swagger API Documentation:** Endpoints are rigorously documented using integrated C# XML docstrings directly piped into `Swashbuckle`. This generates an interactive and highly descriptive Swagger UI out-of-the-box (at `/swagger`) that surfaces models, parameters, and precise return schemas.
##  Tech Stack & Libraries
**Core Framework:** .NET 8 Web API
**Libraries Used:**
*   `Microsoft.Extensions.Http` - Dependency Injection & IHttpClientFactory management.
*   `Microsoft.Extensions.DependencyInjection.Abstractions` / `Configuration.Abstractions` / `Hosting.Abstractions` - Clean service registration and abstractions.
*   `Swashbuckle.AspNetCore` - Swagger UI and OpenAPI documentation generation.
*   `System.Collections.Concurrent` - For Thread-Safe Database emulation.
**Testing Environment:**
*   `xunit` & `xunit.runner.visualstudio` - Primary test framework.
*   `Moq` - Mocking dependencies for isolated unit testing.
*   `FluentAssertions` - Fluent assertions for highly readable tests.
*   `coverlet.collector` - Code coverage statistics.

**External Data Provider:** [IPGeolocation.io](https://ipgeolocation.io/)

---
*Technical assessment for .NET Developer position.*
