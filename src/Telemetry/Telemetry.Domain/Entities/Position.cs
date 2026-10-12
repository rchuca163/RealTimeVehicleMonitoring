namespace Telemetry.Domain.Entities
{
    public class Position
    {
        public long Id { get; set; }

        public int VehicleId { get; set; }

        public decimal Speed { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }
    }
}
