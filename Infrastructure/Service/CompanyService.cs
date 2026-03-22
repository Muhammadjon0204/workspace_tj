using Domain.Entities;
using Infrastructure.Context;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Service;

public class CompanyService(DBContext _db, ILogger<CompanyService> _logger) : ICompanyService
{
    public async Task<IEnumerable<Company>> GetAllAsync()
    {
        try
        {
            return await _db.Companies
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .ToListAsync();
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
            var company = await _db.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

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

            company.CreatedAt = DateTime.UtcNow;

            await _db.Companies.AddAsync(company);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Создана компания: {Name}, id {Id}", company.Name, company.Id);
            return company.Id;
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

            var existing = await _db.Companies.FindAsync(id);
            if (existing == null)
            {
                _logger.LogWarning("Компания с id {Id} не найдена", id);
                return false;
            }

            existing.Name = company.Name;
            existing.Phone = company.Phone;
            existing.Email = company.Email;

            await _db.SaveChangesAsync();

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
            var company = await _db.Companies.FindAsync(id);
            if (company == null)
            {
                _logger.LogWarning("Компания с id {Id} не найдена для удаления", id);
                return false;
            }

            _db.Companies.Remove(company);
            await _db.SaveChangesAsync();

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