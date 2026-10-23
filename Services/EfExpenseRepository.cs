using Microsoft.EntityFrameworkCore;
using Wigo.Data;
using Wigo.Models.Entities;
using Wigo.Models.Enums;

namespace Wigo.Services;

public class EfExpenseRepository : IExpenseRepository
{
    private readonly UiiDbContext _context;

    public EfExpenseRepository(UiiDbContext context)
    {
        _context = context;
    }

    public async Task<List<Expense>> GetAllAsync()
    {
        return await _context.Expenses
            .Include(e => e.Project)
            .Include(e => e.Category)
            .Include(e => e.TrackingToken)
            .ToListAsync();
    }

    public async Task<Expense?> GetByIdAsync(int id)
    {
        return await _context.Expenses
            .Include(e => e.Project)
            .Include(e => e.Category)
            .Include(e => e.TrackingToken)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Expense> AddAsync(Expense expense)
    {
        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();
        return expense;
    }

    public async Task UpdateAsync(Expense expense)
    {
        _context.Entry(expense).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var expense = await _context.Expenses.FindAsync(id);
        if (expense != null)
        {
            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Expense>> GetByProjectAsync(int projectId)
    {
        return await _context.Expenses
            .Where(e => e.ProjectId == projectId)
            .Include(e => e.Project)
            .Include(e => e.Category)
            .Include(e => e.TrackingToken)
            .ToListAsync();
    }

    public async Task<List<Expense>> GetByStatusAsync(ExpenseStatus status)
    {
        return await _context.Expenses
            .Where(e => e.Status == status)
            .Include(e => e.Project)
            .Include(e => e.Category)
            .Include(e => e.TrackingToken)
            .ToListAsync();
    }

    public async Task<List<Expense>> GetByTrackingTokenAsync(int trackingTokenId)
    {
        return await _context.Expenses
            .Where(e => e.TrackingTokenId == trackingTokenId)
            .Include(e => e.Project)
            .Include(e => e.Category)
            .Include(e => e.TrackingToken)
            .ToListAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}