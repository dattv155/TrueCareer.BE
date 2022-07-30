using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Rpc.mentor_register_request
{
    public class MentorRegisterRequestRoute : Root
    {
        private const string Default = Rpc + Module + "/mentor-register-request";

        public const string List = Default + "/list";
        public const string Count = Default + "/count";
        public const string Get = Default + "/get";
        public const string Approve = Default + "/approve";
        public const string Reject = Default + "/reject";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";


        public const string SingleListConnectionType = Default + "/single-list-connection-type";
        public const string SingleListMajor = Default + "/single-list-major";
        public const string SingleListUnitOfTime = Default + "/single-list-unit-of-time";
        public const string UploadImage = Default + "/upload-image";
        public const string Send = Default + "/send";
        public const string SaveTopic = Default + "/save-topic";
        public const string SaveMentorConnection = Default + "/save-mentor-connection";
        public const string SaveActiveTime = Default + "/save-active-time";

    }
}
