using System;
using System.Collections.Generic;
using System.Text;

namespace DataSets.ViewModels
{
    public class RoleVM
    {
        public string roleid { get; set; }
        public string rolename { get; set; }
    }

    public class RoleUserVm
    {
        public string UserId { get; set; }
        public string Role { get; set; }
    }

    public class UserRoleMapping
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
    }
}
