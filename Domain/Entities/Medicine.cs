namespace MedicineFinder.Domain.Entities;

public class Medicine
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ActiveIngredient { get; set; } = string.Empty;
    public int TrendingScore { get; set; }

    public ICollection<MedicineTag> Tags { get; set; } = new List<MedicineTag>();
    public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
