namespace Wigo.Models.Entities;

using Wigo.Models.Enums;
using Wigo.Models.ValueObjects;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public UserRole Role { get; set; }
    public int? OrganizationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    public virtual Organization? Organization { get; set; }
}