# Real-Time Vehicle Monitoring Dashboard

The Monitoring Dashboard is the Angular frontend of the Real-Time Vehicle Monitoring project.

It provides a map-based interface for visualizing vehicle locations, filtering vehicles, inspecting their details, and receiving position and alarm updates in real time.

## Features

- Interactive vehicle map powered by MapLibre GL JS.
- Real-time position updates through SignalR.
- Vehicle-specific alarm visualization.
- Filters by company and vehicle plate.
- Vehicle detail panel with position and speed information.
- Light and dark map themes.
- Automatic map framing based on the vehicles currently displayed.
- SignalR reconnection handling and subscription restoration.

## Technology Stack

- Angular
- TypeScript
- MapLibre GL JS
- SignalR JavaScript client
- CSS

## Application Structure

The frontend separates the dashboard's coordination logic from map rendering and communication services.

- **Dashboard:** Coordinates the initial data request, filters, selected vehicle, and real-time updates.
- **VehicleMap:** Encapsulates MapLibre map initialization, markers, camera behavior, map styles, and vehicle selection.
- **MonitoringApiService:** Retrieves the initial vehicle snapshot and applies the selected filters.
- **MonitoringSignalRService:** Manages the SignalR connection, event handlers, and vehicle group subscriptions.
- **Models:** Define the data structures used by the frontend.
- **Shared vehicle status:** Centralizes the labels and colors used to represent vehicle states.

## Data Flow

### Initial Load

1. The dashboard establishes the SignalR connection.
2. It requests the current vehicle snapshot from the Monitoring API.
3. The dashboard updates its vehicle state and subscribes to the corresponding vehicle groups.
4. The map renders markers for vehicles with valid GPS coordinates.

### Real-Time Updates

When the Monitoring API broadcasts a position update, the dashboard updates the corresponding vehicle in its state. The map receives the updated collection and moves the relevant marker.

Alarm updates are handled separately so that vehicle status can change without requiring a new snapshot request.

If the SignalR connection is interrupted, the client attempts to reconnect and restores its vehicle group subscriptions after reconnection.

## Why SignalR?

A vehicle monitoring dashboard needs to react to changes as they happen.

Using periodic HTTP polling would require repeated requests to discover new positions. SignalR allows the backend to push updates to connected clients, reducing the need for those repeated checks and providing a natural way to deliver position and alarm events.

The dashboard still uses HTTP to retrieve its initial snapshot. SignalR complements that request by keeping the interface up to date afterward.

## Map Behavior

The map uses MapLibre GL JS and OpenFreeMap styles.

- Vehicle markers are created, updated, and removed according to the current filtered collection.
- Marker colors reflect the vehicle's current alarm state.
- Existing markers animate toward their latest GPS coordinates.
- The camera adjusts to show the relevant vehicles.
- The user can switch between light and dark map styles.

## Running Locally

### Prerequisites

- Node.js and npm
- Angular CLI compatible with the project
- The Monitoring API running locally

### Install Dependencies

From the dashboard directory:

```bash
cd src/MonitoringDashboard
npm install
```

### Start the Development Server

```bash
npm start
```

Open the URL reported by the development server, typically:

```text
http://localhost:4200
```

Ensure the API base URL and SignalR hub URL match the local backend configuration. If the development environment uses HTTPS, ensure the browser trusts the development certificate.

## Scope

This frontend is a portfolio demonstration of an interactive monitoring interface. Some vehicle details, such as driver, destination, and fuel level, may currently use placeholder values rather than data supplied by the backend.