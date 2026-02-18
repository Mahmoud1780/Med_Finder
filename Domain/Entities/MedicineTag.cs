namespace MedicineFinder.Domain.Entities;

public class MedicineTag
{
    public Guid Id { get; set; }
    public Guid MedicineId { get; set; }
    public string Tag { get; set; } = string.Empty;

    public Medicine? Medicine { get; set; }
}
