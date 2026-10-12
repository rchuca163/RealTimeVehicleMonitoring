using Monitoring.Api.Data.Repositories;

namespace Monitoring.Api.State;

/// <summary>
/// Initializes the in-memory vehicle state when Monitoring starts.
/// </summary>
/// <remarks>
/// The initial state is reconstructed from the vehicles and their latest
/// persisted positions. After initialization, the store is updated through
/// position events received from Telemetry.
/// </remarks>
public class VehicleStateInitializer(MonitoringRepository repository, VehicleStateStore stateStore)
{
    /// <summary>
    /// Loads the persisted vehicle state into the in-memory store.
    /// </summary>
    /// <param name="cancellationToken">
    /// Token used to cancel the initialization operation.
    /// </param>
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var initialStates = await repository.GetInitialVehicleStatesAsync(cancellationToken);

        stateStore.Initialize(initialStates);
    }
}