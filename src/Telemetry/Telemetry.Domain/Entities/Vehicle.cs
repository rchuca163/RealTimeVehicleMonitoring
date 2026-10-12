namespace Telemetry.Domain.Entities
{
    public class Vehicle
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public string Plate { get; set; } = string.Empty;
    }
}
