using System;
using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface ICompanyService
{

    Task<IEnumerable<Company>> GetAllAsync();
    Task<Company> GetByIdAsync(int id);
    Task<int> CreateAsync(Company company);
    Task<bool> UpdateAsync(int id, Company company);
    Task<bool> DeleteAsync(int id);

}
