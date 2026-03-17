using System;
using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IWorkspaceService
{

    Task<IEnumerable<Workspace>> GetAllAsync();
    Task<Workspace> GetByIdAsync(int id);
    Task<IEnumerable<Workspace>> GetByRoomAsync(int roomId);
    Task<int> CreateAsync(Workspace workspace);
    Task<bool> UpdateAsync(int id, Workspace workspace);
    Task<bool> DeleteAsync(int id);


}
