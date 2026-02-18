using AutoMapper;
using MedicineFinder.Application.DTOs;
using MedicineFinder.Application.Exceptions;
using MedicineFinder.Application.Interfaces;
using MedicineFinder.Domain.Entities;
using MedicineFinder.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MedicineFinder.Application.Services;

public class ReservationService : IReservationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IStockNotifier _notifier;
    private readonly IReservationNotifier _reservationNotifier;

    public ReservationService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IStockNotifier notifier,
        IReservationNotifier reservationNotifier)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _notifier = notifier;
        _reservationNotifier = reservationNotifier;
    }

    public async Task<ReservationDto> CreateReservationAsync(Guid userId, CreateReservationRequestDto request)
    {
        var stockRepo = _unitOfWork.Repository<Stock>();
        var stocks = await stockRepo.FindAsync(s => s.PharmacyId == request.PharmacyId && s.MedicineId == request.MedicineId);
        var stock = stocks.FirstOrDefault();

        if (stock == null || stock.Quantity < request.Quantity)
        {
            throw new ConflictException("Insufficient stock for reservation", "InsufficientStock");
        }

        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PharmacyId = request.PharmacyId,
            MedicineId = request.MedicineId,
            Quantity = request.Quantity,
            Status = ReservationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        var reservationRepo = _unitOfWork.Repository<Reservation>();
        await reservationRepo.AddAsync(reservation);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ReservationDto>(reservation);
    }

    public async Task<ReservationDto> ApproveReservationAsync(Guid reservationId)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var reservationRepo = _unitOfWork.Repository<Reservation>();
            var reservation = await reservationRepo.GetByIdAsync(reservationId);
            if (reservation == null)
            {
                throw new NotFoundException("Reservation not found");
            }

            if (reservation.Status != ReservationStatus.Pending)
            {
                throw new BadRequestException("Only pending reservations can be approved", "InvalidStatus");
            }

            var stockRepo = _unitOfWork.Repository<Stock>();
            var stocks = await stockRepo.FindAsync(s => s.PharmacyId == reservation.PharmacyId && s.MedicineId == reservation.MedicineId);
            var stock = stocks.FirstOrDefault();

            if (stock == null || stock.Quantity < reservation.Quantity)
            {
                throw new ConflictException("Insufficient stock at approval", "InsufficientStock");
            }

            stock.Quantity -= reservation.Quantity;
            stockRepo.Update(stock);

            reservation.Status = ReservationStatus.Approved;
            reservation.RejectionReason = null;
            reservationRepo.Update(reservation);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            await _notifier.StockUpdatedAsync(reservation.PharmacyId, reservation.MedicineId, stock.Quantity);
            await _reservationNotifier.ReservationUpdatedAsync(new ReservationUpdatedDto
            {
                ReservationId = reservation.Id,
                UserId = reservation.UserId,
                PharmacyId = reservation.PharmacyId,
                MedicineId = reservation.MedicineId,
                Quantity = reservation.Quantity,
                Status = reservation.Status.ToString(),
                RejectionReason = reservation.RejectionReason,
                UpdatedAt = DateTime.UtcNow
            });

            return _mapper.Map<ReservationDto>(reservation);
        }
        catch (DbUpdateConcurrencyException)
        {
            await _unitOfWork.RollbackAsync();
            throw new ConflictException("Reservation approval conflict", "ConcurrencyConflict");
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<ReservationDto> RejectReservationAsync(Guid reservationId, RejectReservationRequestDto request)
    {
        var reservationRepo = _unitOfWork.Repository<Reservation>();
        var reservation = await reservationRepo.GetByIdAsync(reservationId);
        if (reservation == null)
        {
            throw new NotFoundException("Reservation not found");
        }

        if (reservation.Status != ReservationStatus.Pending)
        {
            throw new BadRequestException("Only pending reservations can be rejected", "InvalidStatus");
        }

        reservation.Status = ReservationStatus.Rejected;
        reservation.RejectionReason = request.Reason;
        reservationRepo.Update(reservation);
        await _unitOfWork.SaveChangesAsync();
        await _reservationNotifier.ReservationUpdatedAsync(new ReservationUpdatedDto
        {
            ReservationId = reservation.Id,
            UserId = reservation.UserId,
            PharmacyId = reservation.PharmacyId,
            MedicineId = reservation.MedicineId,
            Quantity = reservation.Quantity,
            Status = reservation.Status.ToString(),
            RejectionReason = reservation.RejectionReason,
            UpdatedAt = DateTime.UtcNow
        });

        return _mapper.Map<ReservationDto>(reservation);
    }

    public async Task<IReadOnlyList<UserReservationItemDto>> GetUserReservationsAsync(Guid userId)
    {
        var reservationRepo = _unitOfWork.Repository<Reservation>();
        var reservations = await reservationRepo.FindAsync(r => r.UserId == userId);

        if (reservations.Count == 0)
        {
            return Array.Empty<UserReservationItemDto>();
        }

        var medicineRepo = _unitOfWork.Repository<Medicine>();
        var pharmacyRepo = _unitOfWork.Repository<Pharmacy>();

        var medicineIds = reservations.Select(r => r.MedicineId).Distinct().ToList();
        var pharmacyIds = reservations.Select(r => r.PharmacyId).Distinct().ToList();

        var medicines = await medicineRepo.FindAsync(m => medicineIds.Contains(m.Id));
        var pharmacies = await pharmacyRepo.FindAsync(p => pharmacyIds.Contains(p.Id));

        var medicineMap = medicines.ToDictionary(m => m.Id, m => m.Name);
        var pharmacyMap = pharmacies.ToDictionary(p => p.Id, p => p.Name);

        return reservations
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new UserReservationItemDto
            {
                Id = r.Id,
                MedicineId = r.MedicineId,
                MedicineName = medicineMap.TryGetValue(r.MedicineId, out var mname) ? mname : string.Empty,
                PharmacyId = r.PharmacyId,
                PharmacyName = pharmacyMap.TryGetValue(r.PharmacyId, out var pname) ? pname : string.Empty,
                Quantity = r.Quantity,
                Status = r.Status.ToString(),
                RejectionReason = r.RejectionReason,
                CreatedAt = r.CreatedAt
            }).ToList();
    }

    public async Task<IReadOnlyList<PendingReservationDto>> GetPendingReservationsAsync()
    {
        var reservationRepo = _unitOfWork.Repository<Reservation>();
        var reservations = await reservationRepo.FindAsync(r => r.Status == ReservationStatus.Pending);

        if (reservations.Count == 0)
        {
            return Array.Empty<PendingReservationDto>();
        }

        var medicineRepo = _unitOfWork.Repository<Medicine>();
        var pharmacyRepo = _unitOfWork.Repository<Pharmacy>();
        var userRepo = _unitOfWork.Repository<ApplicationUser>();

        var medicineIds = reservations.Select(r => r.MedicineId).Distinct().ToList();
        var pharmacyIds = reservations.Select(r => r.PharmacyId).Distinct().ToList();
        var userIds = reservations.Select(r => r.UserId).Distinct().ToList();

        var medicines = await medicineRepo.FindAsync(m => medicineIds.Contains(m.Id));
        var pharmacies = await pharmacyRepo.FindAsync(p => pharmacyIds.Contains(p.Id));
        var users = await userRepo.FindAsync(u => userIds.Contains(u.Id));

        var medicineMap = medicines.ToDictionary(m => m.Id, m => m.Name);
        var pharmacyMap = pharmacies.ToDictionary(p => p.Id, p => p.Name);
        var userMap = users.ToDictionary(u => u.Id, u => u);

        return reservations
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new PendingReservationDto
            {
                Id = r.Id,
                UserId = r.UserId,
                UserFullName = userMap.TryGetValue(r.UserId, out var user) ? user.FullName : string.Empty,
                UserEmail = userMap.TryGetValue(r.UserId, out var user2) ? (user2.Email ?? string.Empty) : string.Empty,
                MedicineId = r.MedicineId,
                MedicineName = medicineMap.TryGetValue(r.MedicineId, out var mname) ? mname : string.Empty,
                PharmacyId = r.PharmacyId,
                PharmacyName = pharmacyMap.TryGetValue(r.PharmacyId, out var pname) ? pname : string.Empty,
                Quantity = r.Quantity,
                CreatedAt = r.CreatedAt
            }).ToList();
    }
}
