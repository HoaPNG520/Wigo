using Wigo.Models.Entities;
using Wigo.Models.Enums;

namespace Wigo.Services;

public interface IExpenseRepository
{
    Task<List<Expense>> GetAllAsync();
    Task<Expense?> GetByIdAsync(int id);
    Task<Expense> AddAsync(Expense expense);
    Task UpdateAsync(Expense expense);
    Task DeleteAsync(int id);
    Task<List<Expense>> GetByProjectAsync(int projectId);
    Task<List<Expense>> GetByStatusAsync(ExpenseStatus status);
    Task<List<Expense>> GetByTrackingTokenAsync(int trackingTokenId);
    Task<int> SaveChangesAsync();
}