using Monitoring.Api.Data.Models;

namespace Monitoring.Api.State;

/// <summary>
/// Maintains the current in-memory state of the vehicles monitored by the application.
/// </summary>
/// <remarks>
/// The store is shared across requests and therefore synchronizes access to its
/// internal dictionary to ensure thread-safe reads and writes.
///
/// Vehicles without a persisted or received position are still represented by a
/// VehicleState with null position-related properties.
/// </remarks>
public class VehicleStateStore
{
    private readonly Dictionary<int, VehicleState> _vehicles = new();

    private readonly object _lock = new();

    /// <summary>
    /// Initializes the store with the persisted state of all known vehicles.
    /// </summary>
    /// <param name="initialStates">
    /// The persisted information used to reconstruct the initial vehicle states.
    /// Vehicles without a persisted position are initialized with null position data.
    /// </param>
    public void Initialize(IEnumerable<InitialVehicleState> initialStates)
    {
        lock (_lock)
        {
            foreach (var initialState in initialStates)
            {
                _vehicles[initialState.VehicleId] = new VehicleState(
                    VehicleId: initialState.VehicleId,
                    Plate: initialState.Plate,
                    CompanyId: initialState.CompanyId,
                    CompanyName: initialState.CompanyName,
                    PositionId: initialState.PositionId,
                    Latitude: initialState.Latitude,
                    Longitude: initialState.Longitude,
                    Speed: initialState.Speed,
                    Alarms: null);
            }
        }
    }

    /// <summary>
    /// Creates or replaces the current state of a vehicle.
    /// </summary>
    /// <param name="state">
    /// The latest state received for the vehicle.
    /// </param>
    public void Set(VehicleState state)
    {
        lock (_lock)
        {
            _vehicles[state.VehicleId] = state;
        }
    }

    /// <summary>
    /// Attempts to retrieve the current state of a vehicle.
    /// </summary>
    /// <param name="vehicleId">
    /// The identifier of the vehicle.
    /// </param>
    /// <param name="state">
    /// The current vehicle state, or null when the vehicle is not known by the store.
    /// </param>
    /// <returns>
    /// True when the vehicle is known by the store; otherwise, false.
    /// </returns>
    public bool TryGet(int vehicleId, out VehicleState? state)
    {
        lock (_lock)
        {
            return _vehicles.TryGetValue(vehicleId, out state);
        }
    }

    /// <summary>
    /// Returns a snapshot of the current state of all known vehicles.
    /// </summary>
    /// <returns>
    /// A read-only dictionary containing the current vehicle states.
    /// </returns>
    public IReadOnlyDictionary<int, VehicleState> GetAll()
    {
        lock (_lock)
        {
            return new Dictionary<int, VehicleState>(_vehicles);
        }
    }
}