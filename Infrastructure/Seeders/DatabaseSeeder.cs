using MedicineFinder.Domain.Entities;
using MedicineFinder.Domain.Enums;
using MedicineFinder.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MedicineFinder.Infrastructure.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        await context.Database.MigrateAsync();

        var roles = new[] { UserRole.Admin.ToString(), UserRole.User.ToString() };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }

        await SeedUsersAsync(userManager);
        await SeedPharmaciesAsync(context);
        await SeedMedicinesAsync(context);
        await SeedStocksAsync(context);
    }

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        var adminEmail = "admin@mf.local";
        var userEmail = "user@mf.local";

        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Medicine Finder Admin",
                Role = UserRole.Admin,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(admin, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, UserRole.Admin.ToString());
            }
        }

        if (await userManager.FindByEmailAsync(userEmail) == null)
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = userEmail,
                Email = userEmail,
                FullName = "Medicine Finder User",
                Role = UserRole.User,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, "User123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, UserRole.User.ToString());
            }
        }
    }

    private static async Task SeedPharmaciesAsync(AppDbContext context)
    {
        if (await context.Pharmacies.AnyAsync())
        {
            return;
        }

        var pharmacies = new List<Pharmacy>
        {
            new() { Id = Guid.NewGuid(), Name = "City Care Pharmacy", Latitude = 30.0444, Longitude = 31.2357 },
            new() { Id = Guid.NewGuid(), Name = "Green Cross Pharmacy", Latitude = 29.9792, Longitude = 31.1342 },
            new() { Id = Guid.NewGuid(), Name = "HealthFirst Pharmacy", Latitude = 30.0131, Longitude = 31.2089 }
        };

        await context.Pharmacies.AddRangeAsync(pharmacies);
        await context.SaveChangesAsync();
    }

    private static async Task SeedMedicinesAsync(AppDbContext context)
    {
        if (await context.Medicines.AnyAsync())
        {
            return;
        }

        var medicines = new List<Medicine>
        {
            new() { Id = Guid.NewGuid(), Name = "PainRelief Plus", Category = "Pain Relief", ActiveIngredient = "Ibuprofen", TrendingScore = 95 },
            new() { Id = Guid.NewGuid(), Name = "Allergy Shield", Category = "Allergy", ActiveIngredient = "Loratadine", TrendingScore = 90 },
            new() { Id = Guid.NewGuid(), Name = "ColdAway", Category = "Cold & Flu", ActiveIngredient = "Paracetamol", TrendingScore = 88 },
            new() { Id = Guid.NewGuid(), Name = "HeartSafe", Category = "Cardio", ActiveIngredient = "Aspirin", TrendingScore = 80 },
            new() { Id = Guid.NewGuid(), Name = "GastroEase", Category = "Digestive", ActiveIngredient = "Omeprazole", TrendingScore = 78 },
            new() { Id = Guid.NewGuid(), Name = "SleepWell", Category = "Sleep", ActiveIngredient = "Diphenhydramine", TrendingScore = 70 },
            new() { Id = Guid.NewGuid(), Name = "AntiBac", Category = "Antibiotic", ActiveIngredient = "Amoxicillin", TrendingScore = 85 },
            new() { Id = Guid.NewGuid(), Name = "CalmMind", Category = "Anxiety", ActiveIngredient = "Sertraline", TrendingScore = 72 },
            new() { Id = Guid.NewGuid(), Name = "AsthmaCare", Category = "Respiratory", ActiveIngredient = "Salbutamol", TrendingScore = 76 },
            new() { Id = Guid.NewGuid(), Name = "Diabeto", Category = "Diabetes", ActiveIngredient = "Metformin", TrendingScore = 92 },
            new() { Id = Guid.NewGuid(), Name = "SkinClear", Category = "Dermatology", ActiveIngredient = "Benzoyl Peroxide", TrendingScore = 69 },
            new() { Id = Guid.NewGuid(), Name = "MuscleFlex", Category = "Pain Relief", ActiveIngredient = "Diclofenac", TrendingScore = 74 },
            new() { Id = Guid.NewGuid(), Name = "AllerFree", Category = "Allergy", ActiveIngredient = "Cetirizine", TrendingScore = 86 },
            new() { Id = Guid.NewGuid(), Name = "FluGuard", Category = "Cold & Flu", ActiveIngredient = "Phenylephrine", TrendingScore = 67 },
            new() { Id = Guid.NewGuid(), Name = "BloodFlow", Category = "Cardio", ActiveIngredient = "Clopidogrel", TrendingScore = 66 },
            new() { Id = Guid.NewGuid(), Name = "StomachCalm", Category = "Digestive", ActiveIngredient = "Famotidine", TrendingScore = 71 },
            new() { Id = Guid.NewGuid(), Name = "RestEasy", Category = "Sleep", ActiveIngredient = "Melatonin", TrendingScore = 83 },
            new() { Id = Guid.NewGuid(), Name = "InfectoStop", Category = "Antibiotic", ActiveIngredient = "Azithromycin", TrendingScore = 79 },
            new() { Id = Guid.NewGuid(), Name = "CalmFocus", Category = "Anxiety", ActiveIngredient = "Escitalopram", TrendingScore = 65 },
            new() { Id = Guid.NewGuid(), Name = "BreathEZ", Category = "Respiratory", ActiveIngredient = "Budesonide", TrendingScore = 73 }
        };

        foreach (var medicine in medicines)
        {
            medicine.Tags = new List<MedicineTag>
            {
                new() { Id = Guid.NewGuid(), MedicineId = medicine.Id, Tag = medicine.Category.Replace(" ", "").ToLower() },
                new() { Id = Guid.NewGuid(), MedicineId = medicine.Id, Tag = medicine.ActiveIngredient.Replace(" ", "").ToLower() }
            };
        }

        await context.Medicines.AddRangeAsync(medicines);
        await context.SaveChangesAsync();
    }

    private static async Task SeedStocksAsync(AppDbContext context)
    {
        if (await context.Stocks.AnyAsync())
        {
            return;
        }

        var pharmacies = await context.Pharmacies.ToListAsync();
        var medicines = await context.Medicines.ToListAsync();

        var stocks = new List<Stock>();
        for (var i = 0; i < pharmacies.Count; i++)
        {
            for (var j = 0; j < medicines.Count; j++)
            {
                var quantity = (i + j) % 12;
                stocks.Add(new Stock
                {
                    Id = Guid.NewGuid(),
                    PharmacyId = pharmacies[i].Id,
                    MedicineId = medicines[j].Id,
                    Quantity = quantity
                });
            }
        }

        await context.Stocks.AddRangeAsync(stocks);
        await context.SaveChangesAsync();
    }
}
