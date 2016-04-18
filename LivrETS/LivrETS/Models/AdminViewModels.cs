using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LivrETS.Models
{
    public class AjaxManageUsersViewModel
    {
        private string _newRole;
        public string NewRole
        {
            get
            {
                return _newRole;
            }
            set
            {
                _newRole = (value == "User") ? null : value;
            }
        }

        public string UserId { get; set; }
        public string UserIds { get; set; }
        public List<string> UserIdsList
        {
            get
            {
                return UserIds.Split(',').ToList();
            }
        }
    }
}