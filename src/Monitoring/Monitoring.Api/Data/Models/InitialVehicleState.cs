namespace Monitoring.Api.Data.Models;

/// <summary>
/// Represents the persisted information available to reconstruct a vehicle's
/// initial monitoring state.
/// </summary>
/// <remarks>
/// Position information is optional because a vehicle may exist without having
/// received any position yet.
/// </remarks>
public record InitialVehicleState(
    int VehicleId,
    string Plate,
    int CompanyId,
    string CompanyName,
    long? PositionId,
    decimal? Latitude,
    decimal? Longitude,
    decimal? Speed);