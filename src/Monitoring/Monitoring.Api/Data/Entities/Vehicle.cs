namespace Monitoring.Api.Data.Entities;

public class Vehicle
{
    public int Id { get; set; }

    public int CompanyId { get; set; }

    public Company Company { get; set; } = null!;

    public string Plate { get; set; } = string.Empty;
}