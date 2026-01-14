select
	Users.*, Roles.Name RoleName
from Idp.Users 
left join Idp.Roles on Roles.Id = Users.RoleId