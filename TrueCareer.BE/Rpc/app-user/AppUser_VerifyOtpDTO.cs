using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Rpc.app_user
{
    public class AppUser_VerifyOtpDTO
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }
    }
}
