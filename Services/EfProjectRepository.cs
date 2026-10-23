using Microsoft.EntityFrameworkCore;
using Wigo.Data;
using Wigo.Models.Entities;

namespace Wigo.Services;

public class EfProjectRepository : IProjectRepository
{
    private readonly UiiDbContext _context;

    public EfProjectRepository(UiiDbContext context)
    {
        _context = context;
    }

    public async Task<List<Project>> GetAllAsync()
    {
        return await _context.Projects
            .Include(p => p.Organization)
            .Include(p => p.Expenses)
            .ToListAsync();
    }

    public async Task<Project?> GetByIdAsync(int id)
    {
        return await _context.Projects
            .Include(p => p.Organization)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Project> AddAsync(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task UpdateAsync(Project project)
    {
        _context.Entry(project).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}