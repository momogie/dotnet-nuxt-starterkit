using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Identity.Entities.Views;

[SqlView]
public class AppUserAccountView : IDataTable
{
    public string Code { get; set; }

    public string Name { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    [NotMapped]
    public string PasswordHash { get; set; }

    public long? RoleId { get; set; }

    public string RoleName { get; set; }

    public bool IsAdministrator { get; set; }

    public bool IsActive { get; set; }
}
