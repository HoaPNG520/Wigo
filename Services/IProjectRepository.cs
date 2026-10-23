using Wigo.Models.Entities;
using Wigo.Models.ValueObjects;

namespace Wigo.Services;

public interface IProjectRepository
{
    Task<List<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(int id);
    Task<Project> AddAsync(Project project);
    Task UpdateAsync(Project project);
}