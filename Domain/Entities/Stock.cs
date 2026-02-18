using System.ComponentModel.DataAnnotations;

namespace MedicineFinder.Domain.Entities;

public class Stock
{
    public Guid Id { get; set; }
    public Guid PharmacyId { get; set; }
    public Guid MedicineId { get; set; }
    public int Quantity { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public Pharmacy? Pharmacy { get; set; }
    public Medicine? Medicine { get; set; }
}
