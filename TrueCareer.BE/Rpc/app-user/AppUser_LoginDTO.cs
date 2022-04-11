using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Rpc.app_user
{
    public class AppUser_LoginDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        
        public string GIdToken { get; set; }
        
        public string FbIdToken { get; set; }
        
        public string AIdToken { get; set; }
    }
}
