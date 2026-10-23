namespace Wigo.ViewModels;

public class PublicExpenseViewModel
{
    public int ProjectId { get; set; }
    public int ExpenseId { get; set; }
    public string MaskedAmount { get; set; } = "";
    public string Description { get; set; } = "";
}