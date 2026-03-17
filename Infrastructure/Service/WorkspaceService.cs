using Dapper;
using Domain.Entities;
using Infrastructure.Context;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Service;

public class WorkspaceService : IWorkspaceService
{
    private readonly DataContext _context;
    private readonly ILogger<WorkspaceService> _logger;

    public WorkspaceService(DataContext context, ILogger<WorkspaceService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Workspace>> GetAllAsync()
    {
        try
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM workspaces ORDER BY id";
            return await connection.QueryAsync<Workspace>(sql);
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
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM workspaces WHERE id = @Id";
            var workspace = await connection.QueryFirstOrDefaultAsync<Workspace>(sql, new { Id = id });

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
            using var connection = _context.CreateConnection();
            var sql = "SELECT Id , room_id as RoomId , name , type , created_at as CreatedAt FROM workspaces WHERE room_id = @RoomId";
            return await connection.QueryAsync<Workspace>(sql, new { RoomId = roomId });
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

            if (workspace.RoomId <= 0)
                throw new ArgumentException("Необходимо указать комнату");

            using var connection = _context.CreateConnection();
            var sql = """
                INSERT INTO workspaces (room_id, name, type, created_at)
                VALUES (@RoomId, @Name, @Type, @CreatedAt)
                RETURNING id
                """;

            workspace.CreatedAt = DateTime.UtcNow;
            var id = await connection.ExecuteScalarAsync<int>(sql, workspace);

            _logger.LogInformation("Создано рабочее место: {Name}, id {Id}, комната {RoomId}",
                workspace.Name, id, workspace.RoomId);

            return id;
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

            using var connection = _context.CreateConnection();
            var sql = """
                UPDATE workspaces
                SET room_id = @RoomId, name = @Name, type = @Type
                WHERE id = @Id
                """;

            var rows = await connection.ExecuteAsync(sql, new
            {
                workspace.RoomId,
                workspace.Name,
                workspace.Type,
                Id = id
            });

            if (rows == 0)
            {
                _logger.LogWarning("Рабочее место с id {Id} не найдено для обновления", id);
                return false;
            }

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
            using var connection = _context.CreateConnection();
            var sql = "DELETE FROM workspaces WHERE id = @Id";
            var rows = await connection.ExecuteAsync(sql, new { Id = id });

            if (rows == 0)
            {
                _logger.LogWarning("Рабочее место с id {Id} не найдено для удаления", id);
                return false;
            }

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