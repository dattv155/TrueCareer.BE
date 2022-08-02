using TrueSight.Common;

namespace TrueCareer.Rpc.app_user
{
    public class AppUser_AppUserFirebaseTokenDTO : DataDTO
    {
        public string Token { get; set; }
        public string DeviceModel { get; set; }
        public string OsName { get; set; }
        public string OsVersion { get; set; }
    }
}

