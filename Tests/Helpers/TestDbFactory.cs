using AutoMapper;
using MedicineFinder.Application.Mapping;
using MedicineFinder.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicineFinder.Tests.Helpers;

public static class TestDbFactory
{
    public static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    public static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        return config.CreateMapper();
    }
}
