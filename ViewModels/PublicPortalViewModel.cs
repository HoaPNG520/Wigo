namespace Wigo.ViewModels;

using Wigo.Models.Enums;

public class PublicPortalViewModel
{
    public int ProjectId { get; set; }
    public string Title { get; set; } = "";
    public string Budget { get; set; } = "";
    public string BaseSpent { get; set; } = "";
    public ProjectStatus Status { get; set; }
    public bool PublicEnabled { get; set; }
    public bool SubmissionsOpen { get; set; }
}