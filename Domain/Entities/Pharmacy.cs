namespace MedicineFinder.Domain.Entities;

public class Pharmacy
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
