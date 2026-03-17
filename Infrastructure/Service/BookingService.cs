using Dapper;
using Domain.DTOs;
using Domain.Entities;
using Infrastructure.Context;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Service;

public class BookingService(DataContext _context, ILogger<BookingService> _logger) : IBookingService
{
    public async Task<IEnumerable<Booking>> GetCompanyBookingsAsync(int companyId)
    {
        try
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT Id , company_id as CompanyId , workspace_id as WorkspaceId , booking_date as BookingDate , start_time as StartTime , end_time as EndTime , total_price as TotalPrice , status , created_at as CreatedAt FROM bookings WHERE company_id = @CompanyId ORDER BY booking_date";
            return await connection.QueryAsync<Booking>(sql, new { CompanyId = companyId });
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

            using var connection = _context.CreateConnection();

            var priceSql = """
                SELECT r.price_per_hour
                FROM workspaces w
                JOIN rooms r ON r.id = w.room_id
                WHERE w.id = @WorkspaceId
                """;

            var pricePerHour = await connection.ExecuteScalarAsync<decimal>(priceSql, new
            {
                request.WorkspaceId
            });

            var hours = (decimal)(request.EndTime - request.StartTime).TotalHours;
            var totalPrice = pricePerHour * hours;

            var sql = """
                INSERT INTO bookings (company_id, workspace_id, booking_date, start_time, end_time, total_price, status, created_at)
                VALUES (@CompanyId, @WorkspaceId, @BookingDate, @StartTime, @EndTime, @TotalPrice, 'pending', @CreatedAt)
                RETURNING id
                """;

            var id = await connection.ExecuteScalarAsync<int>(sql, new
            {
                request.CompanyId,
                request.WorkspaceId,
                request.BookingDate,
                request.StartTime,
                request.EndTime,
                TotalPrice = totalPrice,
                CreatedAt = DateTime.UtcNow
            });

            _logger.LogInformation(
                "Создано бронирование id {Id}: компания {CompanyId}, workspace {WorkspaceId}, дата {Date}",
                id, request.CompanyId, request.WorkspaceId, request.BookingDate);

            return id;
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

            using var connection = _context.CreateConnection();
            var sql = "UPDATE bookings SET status = @Status WHERE id = @Id";
            var rows = await connection.ExecuteAsync(sql, new { Status = status, Id = id });

            if (rows == 0)
            {
                _logger.LogWarning("Бронирование с id {Id} не найдено для обновления статуса", id);
                return false;
            }

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
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM bookings WHERE booking_date = @Date ORDER BY start_time";
            return await connection.QueryAsync<Booking>(sql, new { Date = date.Date });
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
            using var connection = _context.CreateConnection();
            var sql = """
                SELECT 
                    COUNT(*) AS TotalBookings,
                    COALESCE(SUM(total_price), 0) AS TotalAmount,
                    COUNT(DISTINCT company_id) AS TotalCompanies
                FROM bookings
                """;

            var stats = await connection.QueryFirstAsync<BookingStatistics>(sql);

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