using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Rpc.app_user
{
    public class AppUser_RegisterDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
        public string DisplayName { get; set; }


    }
}
