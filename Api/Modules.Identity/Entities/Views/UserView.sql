select
	Users.*, Roles.Name RoleName
from Users 
left join Roles on Roles.Id = Users.RoleId