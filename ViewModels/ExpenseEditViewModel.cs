using Microsoft.AspNetCore.Mvc.Rendering;
using Wigo.Models.Entities;
using Wigo.Models.Enums;

namespace Wigo.ViewModels;

public class ExpenseEditViewModel
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int? CategoryId { get; set; }
    public decimal Amount { get; set; }
    public ExpenseStatus Status { get; set; }
    public string SubmittedBy { get; set; } = "";
    public string ReviewNotes { get; set; } = "";
    public List<SelectListItem> Projects { get; set; } = new();
    public List<SelectListItem>? Categories { get; set; }
    public string AiExtractedFields { get; set; } = "";
}