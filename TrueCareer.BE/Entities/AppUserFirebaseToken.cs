using TrueSight.Common;

namespace TrueCareer.Entities
{
    public class AppUserFirebaseToken : DataEntity
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public string Token { get; set; }
        public string DeviceModel { get; set; }
        public string OsName { get; set; }
        public string OsVersion { get; set; }
    }
}

