using Microsoft.AspNetCore.Mvc.Rendering;
using Wigo.Models.Entities;
using Wigo.Models.ValueObjects;

namespace Wigo.ViewModels;

public class ProjectIndexViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Status { get; set; }
    public Money Budget { get; set; } = new();
    public Money BaseSpent { get; set; } = new();
    public bool PublicEnabled { get; set; }
    public bool SubmissionsOpen { get; set; }
    public int ExpenseCount { get; set; }
    public DateTime CreatedAt { get; set; }
}