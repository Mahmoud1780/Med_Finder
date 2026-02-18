using FluentAssertions;
using MedicineFinder.Application.Services;
using MedicineFinder.Domain.Entities;
using MedicineFinder.Infrastructure.UnitOfWork;
using MedicineFinder.Tests.Helpers;
using Xunit;

namespace MedicineFinder.Tests.Services;

public class MedicineServiceTests
{
    [Fact]
    public async Task Alternatives_return_related_or_trending_fallback()
    {
        using var context = TestDbFactory.CreateDbContext();
        var unitOfWork = new UnitOfWork(context);

        var baseMedicine = new Medicine
        {
            Id = Guid.NewGuid(),
            Name = "BaseMed",
            Category = "Pain Relief",
            ActiveIngredient = "Ibuprofen",
            TrendingScore = 10
        };

        var sameCategory = new Medicine
        {
            Id = Guid.NewGuid(),
            Name = "SameCategory",
            Category = "Pain Relief",
            ActiveIngredient = "Aspirin",
            TrendingScore = 50
        };

        var sameIngredient = new Medicine
        {
            Id = Guid.NewGuid(),
            Name = "SameIngredient",
            Category = "Other",
            ActiveIngredient = "Ibuprofen",
            TrendingScore = 40
        };

        var trending = new Medicine
        {
            Id = Guid.NewGuid(),
            Name = "Trending",
            Category = "Other",
            ActiveIngredient = "Other",
            TrendingScore = 99
        };

        context.Medicines.AddRange(baseMedicine, sameCategory, sameIngredient, trending);
        await context.SaveChangesAsync();

        var service = new MedicineService(unitOfWork);
        var alternatives = await service.GetAlternativesAsync(baseMedicine.Id);

        alternatives.Should().NotBeNull();
        alternatives.Should().HaveCount(3);
        alternatives.Should().Contain(a => a.MedicineId == sameCategory.Id || a.MedicineId == sameIngredient.Id);
    }
}
