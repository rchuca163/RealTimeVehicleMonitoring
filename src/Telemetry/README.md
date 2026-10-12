# Telemetry API

The Telemetry API is responsible for receiving, validating, and processing vehicle GPS data in the Real-Time Vehicle Monitoring project.

It represents the ingestion layer of the system, separating incoming telemetry processing from the API responsible for delivering real-time updates to the monitoring dashboard.

## Responsibilities

- Receive vehicle position data through HTTP endpoints.
- Validate incoming requests before processing them.
- Apply vehicle alarm rules, including panic, speeding, and stopped conditions.
- Persist vehicle positions using Entity Framework Core and SQL Server.
- Notify the Monitoring API when a position has been processed.

## Technology Stack

- C#
- ASP.NET Core Web API
- .NET 8
- Entity Framework Core
- SQL Server
- FluentValidation

## Architecture

The project follows a layered structure that separates API concerns, application use cases, domain concepts, and infrastructure implementations.

- **Telemetry.Api:** Exposes HTTP endpoints and handles the API boundary.
- **Telemetry.Application:** Coordinates position processing and application workflows.
- **Telemetry.Domain:** Contains the core entities and business concepts.
- **Telemetry.Infrastructure:** Implements persistence and external communication.

This separation helps keep business logic independent from HTTP handling and infrastructure details.

## Position Processing Flow

1. A client submits a vehicle position.
2. The API validates the incoming request.
3. The application processes the position and evaluates applicable alarm rules.
4. The position is persisted in SQL Server.
5. The processed position is communicated to the Monitoring API.

The position is persisted before the notification is sent. If the notification fails, the stored position is not rolled back. This keeps telemetry persistence independent from the availability of the monitoring notification flow.

## Running Locally

### Prerequisites

- .NET 8 SDK
- SQL Server
- A configured database connection string
- The Monitoring API running for end-to-end notifications

### Configuration

Configure the SQL Server connection string and the Monitoring API endpoint using the application's configuration files or environment-specific settings.

Ensure that the database exists and apply the required Entity Framework Core migrations.

### Run the API

From the repository root, run:

```bash
dotnet run --project src/Telemetry/Telemetry.Api
```

Use the URL reported by the application to access the API. Configure your HTTP client or API testing tool to submit position requests to the appropriate endpoint.

## Scope

This project is a portfolio demonstration focused on telemetry ingestion, validation, persistence, and service separation. It is not intended to represent a complete production fleet-management platform.