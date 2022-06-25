using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueSight.Common;
using TrueCareer.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using TrueCareer.Entities;
using TrueCareer.Services.MConversation;
using TrueCareer.Services.MConversationMessage;
using TrueCareer.Services.MConversationType;
using TrueCareer.Services.MGlobalUser;

namespace TrueCareer.Rpc.conversation
{
    public class ConversationRoute : Root
    {
        public const string Parent = Module + "/conversation";
        public const string Master = Module + "/conversation/conversation-master";
        public const string Detail = Module + "/conversation/conversation-detail";
        public const string Preview = Module + "/conversation/conversation-preview";
        private const string Default = Rpc + Module + "/conversation";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";

        public const string UploadAvatar = Default + "/upload-avatar";

        public const string UploadFile = Default + "/upload-file";
        public const string MultiUploadFile = Default + "/multi-upload-file";

        public const string SingleListConversationMessage = Default + "/single-list-conversation-message";
        public const string SingleListConversationType = Default + "/single-list-conversation-type";
        public const string SingleListGlobalUser = Default + "/single-list-global-user";
        public const string GetGlobalUser = Default + "/get-global-user";

 
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
        };


        private static List<string> SingleList = new List<string> {
            SingleListConversationMessage, SingleListConversationType, SingleListGlobalUser,
        };
        private static List<string> CountList = new List<string>
        {

        };

        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "Tìm kiếm", new List<string> {
                    Parent,
                    Master, Preview, Count, List,
                    Get,  GetGlobalUser,
                }
            },
            { "Thêm", new List<string> {
                    Parent,
                    Master, Preview, Count, List, Get,
                    Detail, Create,
                }.Concat(SingleList).Concat(CountList)
            },

            { "Sửa", new List<string> {
                    Parent,
                    Master, Preview, Count, List, Get,
                    Detail, Update,
                }.Concat(SingleList).Concat(CountList)
            },

            { "Xoá", new List<string> {
                    Parent,
                    Master, Preview, Count, List, Get,
                    Delete,
                }.Concat(SingleList)
            },
        };
    }
}
