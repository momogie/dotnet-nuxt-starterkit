namespace Modules.Identity;

public class Permissions
{
    public static readonly List<AppModule> Modules =
    [
        new AppModule()
        {
            Id = "USRMGR",
            Name = "User Managements",
            Description = "Manage User Accounts & Roles",
            Features = [
                new AppFeature { Id = "RL", Name = "Roles", Description = "List of user roles data" },
                new AppFeature { Id = "RC", Name = "Role Create", Description = "Create new user role" },
                new AppFeature { Id = "RU", Name = "Role Edit", Description = "Edit user role data" },
                new AppFeature { Id = "RR", Name = "Role Delete", Description = "Delete user roles data" },
                new AppFeature { Id = "UL", Name = "Users", Description = "List of users data" },
                new AppFeature { Id = "UC", Name = "User Create", Description = "Create new user" },
                new AppFeature { Id = "UU", Name = "User Edit", Description = "Edit user data" },
                new AppFeature { Id = "UR", Name = "User Delete", Description = "Delete user data" },
                new AppFeature { Id = "UA", Name = "User Activation", Description = "User activation/deactivation" },
            ]
        },

        //new AppModule()
        //{
        //    Id = "USR",
        //    Name = "Users",
        //    Description = "Users",
        //    Features = [
        //        new AppFeature { Id = "L", Name = "List", Description = "List of users data" },
        //        new AppFeature { Id = "C", Name = "Create", Description = "Create new users" },
        //        new AppFeature { Id = "U", Name = "Edit", Description = "Edit users data" },
        //        new AppFeature { Id = "R", Name = "Delete", Description = "Delete users data" },
        //        new AppFeature { Id = "A", Name = "Activate/Deactivate", Description = "User activation/deactivation" },
        //    ]
        //}
    ];
}
