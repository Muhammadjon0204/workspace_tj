using Dapper;
using Domain.Entities;
using Infrastructure.Context;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Service;

public class RoomService(DataContext _context, ILogger<RoomService> _logger) : IRoomService
{
    public async Task<IEnumerable<Room>> GetAllAsync()
    {
        try
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM rooms ORDER BY id";
            return await connection.QueryAsync<Room>(sql);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка комнат");
            throw;
        }
    }

    public async Task<Room> GetByIdAsync(int id)
    {
        try
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM rooms WHERE id = @Id";
            var room = await connection.QueryFirstOrDefaultAsync<Room>(sql, new { Id = id });

            if (room == null)
                _logger.LogWarning("Комната с id {Id} не найдена", id);

            return room!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении комнаты с id {Id}", id);
            throw;
        }
    }

    public async Task<int> CreateAsync(Room room)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(room.Name))
                throw new ArgumentException("Название комнаты обязательно");

            if (room.Capacity <= 0)
                throw new ArgumentException("Вместимость должна быть больше нуля");

            if (room.PricePerHour <= 0)
                throw new ArgumentException("Цена за час должна быть больше нуля");

            using var connection = _context.CreateConnection();
            var sql = """
                INSERT INTO rooms (name, capacity, price_per_hour, created_at)
                VALUES (@Name, @Capacity, @PricePerHour, @CreatedAt)
                RETURNING id
                """;

            room.CreatedAt = DateTime.UtcNow;
            var id = await connection.ExecuteScalarAsync<int>(sql, room);

            _logger.LogInformation("Создана комната: {Name}, id {Id}", room.Name, id);
            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании комнаты {Name}", room.Name);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(int id, Room room)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(room.Name))
                throw new ArgumentException("Название комнаты обязательно");

            if (room.Capacity <= 0)
                throw new ArgumentException("Вместимость должна быть больше нуля");

            using var connection = _context.CreateConnection();
            var sql = """
                UPDATE rooms
                SET name = @Name, capacity = @Capacity, price_per_hour = @PricePerHour
                WHERE id = @Id
                """;

            var rows = await connection.ExecuteAsync(sql, new
            {
                room.Name,
                room.Capacity,
                room.PricePerHour,
                Id = id
            });

            if (rows == 0)
            {
                _logger.LogWarning("Комната с id {Id} не найдена для обновления", id);
                return false;
            }

            _logger.LogInformation("Обновлена комната с id {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении комнаты с id {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            using var connection = _context.CreateConnection();
            var sql = "DELETE FROM rooms WHERE id = @Id";
            var rows = await connection.ExecuteAsync(sql, new { Id = id });

            if (rows == 0)
            {
                _logger.LogWarning("Комната с id {Id} не найдена для удаления", id);
                return false;
            }

            _logger.LogInformation("Удалена комната с id {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении комнаты с id {Id}", id);
            throw;
        }
    }
}