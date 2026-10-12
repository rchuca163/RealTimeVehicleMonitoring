# Monitoring API

The Monitoring API provides the data and real-time communication capabilities required by the vehicle monitoring dashboard.

It receives processed position notifications from the Telemetry API, exposes vehicle snapshots through HTTP endpoints, and broadcasts relevant changes to connected clients using SignalR.

## Responsibilities

- Expose vehicle monitoring data through HTTP endpoints.
- Provide vehicle snapshots that the dashboard can use to initialize its state.
- Receive processed position notifications from the Telemetry API.
- Broadcast position and alarm updates to connected clients.
- Organize SignalR connections into vehicle-specific groups.

## Technology Stack

- C#
- ASP.NET Core Web API
- .NET 8
- SignalR

## Architecture

The Monitoring API has a different responsibility from the Telemetry API.

The Telemetry API handles incoming GPS data and persistence. The Monitoring API focuses on making the latest monitoring information available to clients and distributing changes in real time.

This separation allows telemetry ingestion and client communication to evolve independently.

## Real-Time Communication with SignalR

Vehicle monitoring is a scenario where the user benefits from receiving updates as soon as new information becomes available.

Without server-to-client notifications, the dashboard could periodically query the API to check whether vehicle positions or alarms had changed. This polling approach is valid in some situations, but it generates repeated requests even when no new data is available.

SignalR provides a more suitable communication model for this use case:

- The server can push updates when new information is processed.
- The dashboard does not need to poll continuously for every position change.
- Vehicle-specific groups allow updates to be targeted to the relevant subscriptions.
- SignalR manages connections and can use WebSockets when available, with alternative transports when necessary.

### Event Flow

1. The dashboard requests a vehicle snapshot through HTTP.
2. The client establishes a SignalR connection.
3. The dashboard subscribes to the vehicle groups it needs to monitor.
4. The Telemetry API sends a processed-position notification to the Monitoring API.
5. The Monitoring API broadcasts the corresponding position and, when applicable, alarm updates.
6. The dashboard updates its state without requiring a page refresh.

HTTP remains useful for retrieving the initial snapshot, while SignalR handles ongoing updates. These technologies complement each other rather than replace one another.

## SignalR Hub

The hub is exposed at:

```text
/hubs/monitoring
```

The application uses vehicle-specific groups to distribute updates to clients monitoring particular vehicles.

The dashboard joins and leaves these groups as its filters change. It also re-establishes its subscriptions after a successful SignalR reconnection.

## Running Locally

### Prerequisites

- .NET 8 SDK
- The Telemetry API for end-to-end position processing

### Run the API

From the repository root:

```bash
dotnet run --project src/Monitoring/Monitoring.Api
```

Configure the Telemetry API to send processed-position notifications to the Monitoring API. Use the URL reported by the application when configuring the frontend connection.

## Design Considerations

The Monitoring API separates the delivery of monitoring updates from the ingestion and persistence of telemetry.

The current implementation is intentionally simple and suitable for demonstrating the architecture and real-time communication flow. More advanced delivery guarantees, distributed state management, and horizontal scaling would require additional design decisions beyond the scope of this demo.