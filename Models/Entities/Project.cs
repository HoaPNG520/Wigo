namespace Wigo.Models.Entities;

using Wigo.Models.Enums;
using Wigo.Models.ValueObjects;

public class Project
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public string Name { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Money Budget { get; set; } = new();
    public Money BaseSpent { get; set; } = new();
    public ProjectStatus Status { get; set; }
    public bool PublicEnabled { get; set; }
    public bool SubmissionsOpen { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual Organization Organization { get; set; } = null!;
    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}