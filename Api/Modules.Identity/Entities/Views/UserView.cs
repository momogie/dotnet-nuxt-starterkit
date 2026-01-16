namespace Modules.Identity.Entities.Views;

[SqlView]
public class UserView : IDataTable
{
    public string Id { get; set; }
    [Filterable]
    [DataColumn(Name = "Username")]
    public string UserName { get; set; }
    [Filterable]
    [DataColumn(Name = "Name")]
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public string NormalizedUserName { get; set; }
    [Filterable]
    [DataColumn(Name = "Email")]
    public string Email { get; set; }
    [DataColumn(Name = "Active")]
    public bool IsActive { get; set; }
    public string NormalizedEmail { get; set; }
    public bool EmailConfirmed { get; set; }
    public string PasswordHash { get; set; }
    public string SecurityStamp { get; set; }
    public string ConcurrencyStamp { get; set; }
    public string PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public bool LockoutEnabled { get; set; }
    public bool AccessFailedCount { get; set; }
}
