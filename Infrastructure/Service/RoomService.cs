using Domain.Entities;
using Infrastructure.Context;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Service;

public class RoomService(DBContext _db, ILogger<RoomService> _logger) : IRoomService
{
    public async Task<IEnumerable<Room>> GetAllAsync()
    {
        try
        {
            return await _db.Rooms
                .AsNoTracking()
                .OrderBy(r => r.Id)
                .ToListAsync();
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
            var room = await _db.Rooms
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

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

            room.CreatedAt = DateTime.UtcNow;

            await _db.Rooms.AddAsync(room);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Создана комната: {Name}, id {Id}", room.Name, room.Id);
            return room.Id;
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

            var existing = await _db.Rooms.FindAsync(id);
            if (existing == null)
            {
                _logger.LogWarning("Комната с id {Id} не найдена для обновления", id);
                return false;
            }

            existing.Name = room.Name;
            existing.Capacity = room.Capacity;
            existing.PricePerHour = room.PricePerHour;

            await _db.SaveChangesAsync();

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
            var room = await _db.Rooms.FindAsync(id);
            if (room == null)
            {
                _logger.LogWarning("Комната с id {Id} не найдена для удаления", id);
                return false;
            }

            _db.Rooms.Remove(room);
            await _db.SaveChangesAsync();

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