# GeoGuard API 🛡️

A high-performance, stateless .NET Core Web API for managing country-based IP blocking. Built with thread-safe in-memory collections and external geolocation integration.

## Problem Definition
The objective of this assignment was to create a robust API capable of managing a blocked countries list and validating incoming caller IP addresses against this list. The strict constraint was to bypass traditional databases entirely, relying exclusively on **thread-safe in-memory storage** while orchestrating requests to third-party geolocation APIs. Furthermore, the system needed to audit all access attempts and support both permanent and temporal blocks gracefully.

## Architecture & Design
To demonstrate senior-level .NET engineering principles, this solution avoids "Primitive Obsession" and "Spaghetti Code" by strictly adhering to **Clean Architecture** and **Domain-Driven Design (DDD)** concepts:

*   **GeoGuard.Domain:** The pure heart of the application. It contains highly encapsulated Entities (`BlockedCountry`, `BlockedAttemptLog`) with private setters and static factory methods. We utilized **Value Objects** (like `CountryCode` and native `System.Net.IPAddress`) to mathematically guarantee valid state within our domain rules (e.g., enforcing strict 2-character ISO Alpha-2 codes) before data ever reaches a service.
*   **GeoGuard.Infrastructure:** Strictly handles I/O, external HTTP connections, and data persistence. Handled the database constraint elegantly by implementing `IBlockedCountryRepository` using a `ConcurrentDictionary`, and `IBlockedAttemptRepository` using a `ConcurrentQueue`. This guarantees thread-safety across API requests without the overhead of manual locking.
*   **GeoGuard.Api:** The presentation layer. Controllers are kept intentionally "dumb", acting purely as traffic cops. All domain instantiation and HTTP response abstractions are delegated to Application Services.

##  Key Solutions & Engineering Patterns
*   **Result<T> Pattern:** Implemented across repositories and services to entirely eliminate costly exception-throwing for control flow, standardizing success/failure states seamlessly into HTTP responses.
*   **Early-Exit Orchestration:** The `BlockVerificationService` handles complex conditional logic (verifying IP, checking country block, validating temporal expiration, and safely logging the attempt) utilizing strict early-exit patterns for flawless, readable control flow.
*   **Unified Expiration Strategy:** By design, the `ExpirationTime` on blocks is a `DateTime?`. A `null` value conceptually acts as a "Permanent" block, while a populated value acts as a "Temporal" block, allowing an elegant unified storage mechanism without duplicating entities.
*   **Resilient HTTP Client:** Utilizes `IHttpClientFactory` instead of manual `HttpClient` instantiation to prevent socket exhaustion and effectively manage TCP connections during high-load API geolocation lookups.

##  Tech Stack & Libraries
*   **.NET 8 Web API**
*   **Dependency Injection & IHttpClientFactory** (`Microsoft.Extensions.Http`)
*   **System.Collections.Concurrent** (For Thread-Safe Database emulation)
*   **External Data Provider:** [IPGeolocation.io](https://ipgeolocation.io/)

---
*Technical assessment for .NET Developer position.*
