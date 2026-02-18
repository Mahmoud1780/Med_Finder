using FluentAssertions;
using MedicineFinder.Application.DTOs;
using MedicineFinder.Application.Exceptions;
using MedicineFinder.Application.Interfaces;
using MedicineFinder.Application.Services;
using MedicineFinder.Domain.Entities;
using MedicineFinder.Domain.Enums;
using MedicineFinder.Infrastructure.UnitOfWork;
using MedicineFinder.Tests.Helpers;
using Xunit;

namespace MedicineFinder.Tests.Services;

public class ReservationServiceTests
{
    private sealed class NoOpStockNotifier : IStockNotifier
    {
        public Task StockUpdatedAsync(Guid pharmacyId, Guid medicineId, int quantity) => Task.CompletedTask;
    }

    private sealed class NoOpReservationNotifier : IReservationNotifier
    {
        public Task ReservationUpdatedAsync(ReservationUpdatedDto update) => Task.CompletedTask;
    }

    [Fact]
    public async Task Reserve_fails_if_qty_exceeds_stock()
    {
        using var context = TestDbFactory.CreateDbContext();
        var mapper = TestDbFactory.CreateMapper();
        var unitOfWork = new UnitOfWork(context);
        var notifier = new NoOpStockNotifier();
        var reservationNotifier = new NoOpReservationNotifier();

        var pharmacy = new Pharmacy { Id = Guid.NewGuid(), Name = "Test", Latitude = 1, Longitude = 1 };
        var medicine = new Medicine { Id = Guid.NewGuid(), Name = "Med", Category = "Cat", ActiveIngredient = "Ing" };
        var stock = new Stock { Id = Guid.NewGuid(), PharmacyId = pharmacy.Id, MedicineId = medicine.Id, Quantity = 1 };

        context.Pharmacies.Add(pharmacy);
        context.Medicines.Add(medicine);
        context.Stocks.Add(stock);
        await context.SaveChangesAsync();

        var service = new ReservationService(unitOfWork, mapper, notifier, reservationNotifier);
        var request = new CreateReservationRequestDto
        {
            PharmacyId = pharmacy.Id,
            MedicineId = medicine.Id,
            Quantity = 5
        };

        Func<Task> act = async () => await service.CreateReservationAsync(Guid.NewGuid(), request);

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Approve_decrements_stock()
    {
        using var context = TestDbFactory.CreateDbContext();
        var mapper = TestDbFactory.CreateMapper();
        var unitOfWork = new UnitOfWork(context);
        var notifier = new NoOpStockNotifier();
        var reservationNotifier = new NoOpReservationNotifier();

        var pharmacy = new Pharmacy { Id = Guid.NewGuid(), Name = "Test", Latitude = 1, Longitude = 1 };
        var medicine = new Medicine { Id = Guid.NewGuid(), Name = "Med", Category = "Cat", ActiveIngredient = "Ing" };
        var stock = new Stock { Id = Guid.NewGuid(), PharmacyId = pharmacy.Id, MedicineId = medicine.Id, Quantity = 5 };
        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            PharmacyId = pharmacy.Id,
            MedicineId = medicine.Id,
            UserId = Guid.NewGuid(),
            Quantity = 2,
            Status = ReservationStatus.Pending
        };

        context.Pharmacies.Add(pharmacy);
        context.Medicines.Add(medicine);
        context.Stocks.Add(stock);
        context.Reservations.Add(reservation);
        await context.SaveChangesAsync();

        var service = new ReservationService(unitOfWork, mapper, notifier, reservationNotifier);

        await service.ApproveReservationAsync(reservation.Id);

        var updatedStock = context.Stocks.First();
        updatedStock.Quantity.Should().Be(3);
    }

    [Fact]
    public async Task Approve_fails_if_insufficient_stock_at_approval()
    {
        using var context = TestDbFactory.CreateDbContext();
        var mapper = TestDbFactory.CreateMapper();
        var unitOfWork = new UnitOfWork(context);
        var notifier = new NoOpStockNotifier();
        var reservationNotifier = new NoOpReservationNotifier();

        var pharmacy = new Pharmacy { Id = Guid.NewGuid(), Name = "Test", Latitude = 1, Longitude = 1 };
        var medicine = new Medicine { Id = Guid.NewGuid(), Name = "Med", Category = "Cat", ActiveIngredient = "Ing" };
        var stock = new Stock { Id = Guid.NewGuid(), PharmacyId = pharmacy.Id, MedicineId = medicine.Id, Quantity = 1 };
        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            PharmacyId = pharmacy.Id,
            MedicineId = medicine.Id,
            UserId = Guid.NewGuid(),
            Quantity = 2,
            Status = ReservationStatus.Pending
        };

        context.Pharmacies.Add(pharmacy);
        context.Medicines.Add(medicine);
        context.Stocks.Add(stock);
        context.Reservations.Add(reservation);
        await context.SaveChangesAsync();

        var service = new ReservationService(unitOfWork, mapper, notifier, reservationNotifier);

        Func<Task> act = async () => await service.ApproveReservationAsync(reservation.Id);

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Status_transition_validation_blocks_non_pending()
    {
        using var context = TestDbFactory.CreateDbContext();
        var mapper = TestDbFactory.CreateMapper();
        var unitOfWork = new UnitOfWork(context);
        var notifier = new NoOpStockNotifier();
        var reservationNotifier = new NoOpReservationNotifier();

        var pharmacy = new Pharmacy { Id = Guid.NewGuid(), Name = "Test", Latitude = 1, Longitude = 1 };
        var medicine = new Medicine { Id = Guid.NewGuid(), Name = "Med", Category = "Cat", ActiveIngredient = "Ing" };
        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            PharmacyId = pharmacy.Id,
            MedicineId = medicine.Id,
            UserId = Guid.NewGuid(),
            Quantity = 1,
            Status = ReservationStatus.Approved
        };

        context.Pharmacies.Add(pharmacy);
        context.Medicines.Add(medicine);
        context.Reservations.Add(reservation);
        await context.SaveChangesAsync();

        var service = new ReservationService(unitOfWork, mapper, notifier, reservationNotifier);

        Func<Task> act = async () => await service.ApproveReservationAsync(reservation.Id);

        await act.Should().ThrowAsync<BadRequestException>();
    }
}
