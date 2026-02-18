using System.Reflection;
using FluentAssertions;
using MedicineFinder.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Xunit;

namespace MedicineFinder.Tests.Controllers;

public class AdminStockControllerTests
{
    [Fact]
    public void Admin_stock_endpoint_requires_admin_role()
    {
        var attribute = typeof(AdminStockController)
            .GetCustomAttributes<AuthorizeAttribute>()
            .FirstOrDefault();

        attribute.Should().NotBeNull();
        attribute!.Roles.Should().Contain("Admin");
    }
}
