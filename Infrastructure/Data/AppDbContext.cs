using MedicineFinder.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MedicineFinder.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Pharmacy> Pharmacies => Set<Pharmacy>();
    public DbSet<Medicine> Medicines => Set<Medicine>();
    public DbSet<MedicineTag> MedicineTags => Set<MedicineTag>();
    public DbSet<Stock> Stocks => Set<Stock>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(u => u.FullName).HasMaxLength(200).IsRequired();
            entity.Property(u => u.Email).HasMaxLength(200).IsRequired();
            entity.HasIndex(u => u.Email).IsUnique();
        });

        builder.Entity<Pharmacy>(entity =>
        {
            entity.Property(p => p.Name).HasMaxLength(200).IsRequired();
        });

        builder.Entity<Medicine>(entity =>
        {
            entity.Property(m => m.Name).HasMaxLength(200).IsRequired();
            entity.Property(m => m.Category).HasMaxLength(200).IsRequired();
            entity.Property(m => m.ActiveIngredient).HasMaxLength(200).IsRequired();
            entity.HasIndex(m => m.Name);
        });

        builder.Entity<MedicineTag>(entity =>
        {
            entity.Property(t => t.Tag).HasMaxLength(100).IsRequired();
            entity.HasOne(t => t.Medicine)
                .WithMany(m => m.Tags)
                .HasForeignKey(t => t.MedicineId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Stock>(entity =>
        {
            entity.HasIndex(s => new { s.PharmacyId, s.MedicineId }).IsUnique();
            entity.Property(s => s.RowVersion).IsRowVersion();
            entity.HasOne(s => s.Pharmacy)
                .WithMany(p => p.Stocks)
                .HasForeignKey(s => s.PharmacyId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(s => s.Medicine)
                .WithMany(m => m.Stocks)
                .HasForeignKey(s => s.MedicineId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Reservation>(entity =>
        {
            entity.Property(r => r.RejectionReason).HasMaxLength(500);
            entity.HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(r => r.Pharmacy)
                .WithMany(p => p.Reservations)
                .HasForeignKey(r => r.PharmacyId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(r => r.Medicine)
                .WithMany(m => m.Reservations)
                .HasForeignKey(r => r.MedicineId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
