using Dapper;
using Domain.Entities;
using Infrastructure.Context;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Service;

public class CompanyService(DataContext _context, ILogger<CompanyService> _logger) : ICompanyService
{
    public async Task<IEnumerable<Company>> GetAllAsync()
    {
        try
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM companies ORDER BY id";
            return await connection.QueryAsync<Company>(sql);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка компаний");
            throw;
        }
    }

    public async Task<Company> GetByIdAsync(int id)
    {
        try
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM companies WHERE id = @Id";
            var company = await connection.QueryFirstOrDefaultAsync<Company>(sql, new { Id = id });

            if (company == null)
                _logger.LogWarning("Компания с id {Id} не найдена", id);

            return company!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении компании с id {Id}", id);
            throw;
        }
    }

    public async Task<int> CreateAsync(Company company)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(company.Name))
                throw new ArgumentException("Название компании обязательно");

            if (string.IsNullOrWhiteSpace(company.Email))
                throw new ArgumentException("Email компании обязателен");

            using var connection = _context.CreateConnection();
            var sql = """
                INSERT INTO companies (name, phone, email, created_at)
                VALUES (@Name, @Phone, @Email, @CreatedAt)
                RETURNING id
                """;

            company.CreatedAt = DateTime.UtcNow;
            var id = await connection.ExecuteScalarAsync<int>(sql, company);

            _logger.LogInformation("Создана компания: {Name}, id {Id}", company.Name, id);
            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании компании {Name}", company.Name);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(int id, Company company)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(company.Name))
                throw new ArgumentException("Название компании обязательно");

            using var connection = _context.CreateConnection();
            var sql = """
                UPDATE companies
                SET name = @Name, phone = @Phone, email = @Email
                WHERE id = @Id
                """;

            var rows = await connection.ExecuteAsync(sql, new
            {
                company.Name,
                company.Phone,
                company.Email,
                Id = id
            });

            if (rows == 0)
            {
                _logger.LogWarning("Компания с id {Id} не найдена для обновления", id);
                return false;
            }

            _logger.LogInformation("Обновлена компания с id {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении компании с id {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            using var connection = _context.CreateConnection();
            var sql = "DELETE FROM companies WHERE id = @Id";
            var rows = await connection.ExecuteAsync(sql, new { Id = id });

            if (rows == 0)
            {
                _logger.LogWarning("Компания с id {Id} не найдена для удаления", id);
                return false;
            }

            _logger.LogInformation("Удалена компания с id {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении компании с id {Id}", id);
            throw;
        }
    }
}