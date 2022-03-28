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
using TrueCareer.Services.MConversationMessage;
using TrueCareer.Services.MConversation;
using TrueCareer.Services.MConversationType;
using TrueCareer.Services.MGlobalUser;

namespace TrueCareer.Rpc.conversation_message
{
    public class ConversationMessageRoute : Root
    {
        public const string Parent = Module + "/conversation-message";
        public const string Master = Module + "/conversation-message/conversation-message-master";
        public const string Detail = Module + "/conversation-message/conversation-message-detail";
        public const string Preview = Module + "/conversation-message/conversation-message-preview";
        private const string Default = Rpc + Module + "/conversation-message";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Read = Default + "/read";
        public const string UploadFile = Default + "/upload-file";
        public const string MultiUploadFile = Default + "/multi-upload-file";
        

        public const string SingleListConversation = Default + "/single-list-conversation";
        public const string SingleListConversationType = Default + "/single-list-conversation-type";
        public const string SingleListGlobalUser = Default + "/single-list-global-user";


        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
        };


        private static List<string> SingleList = new List<string> { 
            SingleListConversation, SingleListConversationType, SingleListGlobalUser, 
        };

        
        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "Tìm kiếm", new List<string> { 
                    Parent,
                    Master, Preview, Count, List,
                    Get, Read,
                }
            },
            { "Thêm", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get, 
                    Detail, Create,  Read,
                }.Concat(SingleList)
            },

            { "Sửa", new List<string> { 
                    Parent,            
                    Master, Preview, Count, List, Get,
                    Detail, Update,  Read,
                }.Concat(SingleList)
            },

            { "Xoá", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    Delete,  Read,
                }.Concat(SingleList)
            },
        };
    }
}
