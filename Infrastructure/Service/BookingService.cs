using Domain.DTOs;
using Domain.Entities;
using Infrastructure.Context;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Service;

public class BookingService(DBContext _db, ILogger<BookingService> _logger) : IBookingService
{
    public async Task<IEnumerable<Booking>> GetCompanyBookingsAsync(int companyId)
    {
        try
        {
            return await _db.Bookings
                .AsNoTracking()
                .Where(b => b.CompanyId == companyId)
                .OrderBy(b => b.BookingDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении бронирований компании с id {CompanyId}", companyId);
            throw;
        }
    }

    public async Task<int> CreateBookingAsync(CreateBookingRequest request)
    {
        try
        {
            if (request.EndTime <= request.StartTime)
                throw new ArgumentException("Время окончания должно быть позже времени начала");

            if (request.BookingDate < DateTime.Today)
                throw new ArgumentException("Дата бронирования не может быть в прошлом");

            // Получаем цену через Include — EF делает JOIN сам
            var pricePerHour = await _db.Workspaces
                .AsNoTracking()
                .Where(w => w.Id == request.WorkspaceId)
                .Select(w => w.Room!.PricePerHour)
                .FirstOrDefaultAsync();

            var hours = (decimal)(request.EndTime - request.StartTime).TotalHours;
            var totalPrice = pricePerHour * hours;

            var booking = new Booking
            {
                CompanyId = request.CompanyId,
                WorkspaceId = request.WorkspaceId,
                BookingDate = request.BookingDate,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                TotalPrice = totalPrice,
                Status = "pending",
                CreatedAt = DateTime.UtcNow
            };

            await _db.Bookings.AddAsync(booking);
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Создано бронирование id {Id}: компания {CompanyId}, workspace {WorkspaceId}, дата {Date}",
                booking.Id, request.CompanyId, request.WorkspaceId, request.BookingDate);

            return booking.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании бронирования для компании {CompanyId}", request.CompanyId);
            throw;
        }
    }

    public async Task<bool> UpdateBookingStatusAsync(int id, string status)
    {
        try
        {
            var allowedStatuses = new[] { "pending", "confirmed", "cancelled" };
            if (!allowedStatuses.Contains(status.ToLower()))
                throw new ArgumentException($"Недопустимый статус. Допустимые: {string.Join(", ", allowedStatuses)}");

            var booking = await _db.Bookings.FindAsync(id);
            if (booking == null)
            {
                _logger.LogWarning("Бронирование с id {Id} не найдено для обновления статуса", id);
                return false;
            }

            booking.Status = status;
            await _db.SaveChangesAsync();

            _logger.LogInformation("Статус бронирования id {Id} изменён на {Status}", id, status);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при изменении статуса бронирования id {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Booking>> GetBookingsByDateAsync(DateTime date)
    {
        try
        {
            return await _db.Bookings
                .AsNoTracking()
                .Where(b => b.BookingDate.Date == date.Date)
                .OrderBy(b => b.StartTime)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении бронирований за дату {Date}", date);
            throw;
        }
    }

    public async Task<BookingStatistics> GetStatisticsAsync()
    {
        try
        {
            var stats = await _db.Bookings
                .AsNoTracking()
                .GroupBy(_ => 1)
                .Select(g => new BookingStatistics
                {
                    TotalBookings = g.Count(),
                    TotalAmount = g.Sum(b => b.TotalPrice),
                    TotalCompanies = g.Select(b => b.CompanyId).Distinct().Count()
                })
                .FirstOrDefaultAsync();

            // Если бронирований нет вообще — GroupBy вернёт null
            stats ??= new BookingStatistics();

            _logger.LogInformation("Получена статистика бронирований");
            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении статистики бронирований");
            throw;
        }
    }
}