using Domain.Entities;
using Infrastructure.Context;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Service;

public class WorkspaceService(DBContext _db, ILogger<WorkspaceService> _logger) : IWorkspaceService
{
    public async Task<IEnumerable<Workspace>> GetAllAsync()
    {
        try
        {
            return await _db.Workspaces
                .AsNoTracking()
                .OrderBy(w => w.Id)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка рабочих мест");
            throw;
        }
    }

    public async Task<Workspace?> GetByIdAsync(int id)
    {
        try
        {
            var workspace = await _db.Workspaces
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == id);

            if (workspace == null)
                _logger.LogWarning("Рабочее место с id {Id} не найдено", id);

            return workspace;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении рабочего места с id {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Workspace>> GetByRoomAsync(int roomId)
    {
        try
        {
            var roomExists = await _db.Rooms.AnyAsync(r => r.Id == roomId);
            if (!roomExists)
            {
                _logger.LogWarning("Комната с id {RoomId} не найдена", roomId);
                return new List<Workspace>();
            }

            return await _db.Workspaces
                .AsNoTracking()
                .Where(w => w.RoomId == roomId)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении рабочих мест для комнаты с id {RoomId}", roomId);
            throw;
        }
    }

    public async Task<int> CreateAsync(Workspace workspace)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(workspace.Name))
                throw new ArgumentException("Название рабочего места обязательно");

            if (string.IsNullOrWhiteSpace(workspace.Type))
                throw new ArgumentException("Тип рабочего места обязателен");

            if (workspace.RoomId < 1)
                throw new ArgumentException("Необходимо указать комнату");

            var roomExists = await _db.Rooms.AnyAsync(r => r.Id == workspace.RoomId);
            if (!roomExists)
            {
                _logger.LogWarning("Комната с id {RoomId} не найдена", workspace.RoomId);
                throw new ArgumentException($"Комната с id {workspace.RoomId} не найдена");
            }

            workspace.CreatedAt = DateTime.UtcNow;

            await _db.Workspaces.AddAsync(workspace);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Создано рабочее место: {Name}, id {Id}, комната {RoomId}",
                workspace.Name, workspace.Id, workspace.RoomId);

            return workspace.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании рабочего места {Name}", workspace.Name);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(int id, Workspace workspace)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(workspace.Name))
                throw new ArgumentException("Название рабочего места обязательно");

            if (string.IsNullOrWhiteSpace(workspace.Type))
                throw new ArgumentException("Тип рабочего места обязателен");

            var existing = await _db.Workspaces.FindAsync(id);
            if (existing == null)
            {
                _logger.LogWarning("Рабочее место с id {Id} не найдено для обновления", id);
                return false;
            }

            existing.RoomId = workspace.RoomId;
            existing.Name = workspace.Name;
            existing.Type = workspace.Type;

            await _db.SaveChangesAsync();

            _logger.LogInformation("Обновлено рабочее место с id {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении рабочего места с id {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var workspace = await _db.Workspaces.FindAsync(id);
            if (workspace == null)
            {
                _logger.LogWarning("Рабочее место с id {Id} не найдено для удаления", id);
                return false;
            }

            _db.Workspaces.Remove(workspace);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Удалено рабочее место с id {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении рабочего места с id {Id}", id);
            throw;
        }
    }
}