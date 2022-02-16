using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Common;
using TrueCareer.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using TrueCareer.Entities;
using TrueCareer.Services.MMentorMenteeConnection;
using TrueCareer.Services.MMentorConnection;
using TrueCareer.Services.MConnectionStatus;
using TrueCareer.Services.MAppUser;

namespace TrueCareer.Rpc.mentor_mentee_connection
{
    public class MentorMenteeConnectionRoute : Root
    {
        public const string Parent = Module + "/mentor-mentee-connection";
        public const string Master = Module + "/mentor-mentee-connection/mentor-mentee-connection-master";
        public const string Detail = Module + "/mentor-mentee-connection/mentor-mentee-connection-detail";
        public const string Preview = Module + "/mentor-mentee-connection/mentor-mentee-connection-preview";
        private const string Default = Rpc + Module + "/mentor-mentee-connection";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-template";
        public const string BulkDelete = Default + "/bulk-delete";
        
        public const string FilterListMentorConnection = Default + "/filter-list-mentor-connection";
        public const string FilterListConnectionStatus = Default + "/filter-list-connection-status";
        public const string FilterListAppUser = Default + "/filter-list-app-user";

        public const string SingleListMentorConnection = Default + "/single-list-mentor-connection";
        public const string SingleListConnectionStatus = Default + "/single-list-connection-status";
        public const string SingleListAppUser = Default + "/single-list-app-user";


        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(MentorMenteeConnectionFilter.MentorId), FieldTypeEnum.ID.Id },
            { nameof(MentorMenteeConnectionFilter.MenteeId), FieldTypeEnum.ID.Id },
            { nameof(MentorMenteeConnectionFilter.ConnectionId), FieldTypeEnum.ID.Id },
            { nameof(MentorMenteeConnectionFilter.FirstMessage), FieldTypeEnum.STRING.Id },
            { nameof(MentorMenteeConnectionFilter.ConnectionStatusId), FieldTypeEnum.ID.Id },
            { nameof(MentorMenteeConnectionFilter.ActiveTimeId), FieldTypeEnum.ID.Id },
            { nameof(MentorMenteeConnectionFilter.Id), FieldTypeEnum.ID.Id },
        };

        private static List<string> FilterList = new List<string> { 
            FilterListMentorConnection,FilterListConnectionStatus,FilterListAppUser,
        };
        private static List<string> SingleList = new List<string> { 
            SingleListMentorConnection, SingleListConnectionStatus, SingleListAppUser, 
        };
        private static List<string> CountList = new List<string> { 
            
        };
        
        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "Tìm kiếm", new List<string> { 
                    Parent,
                    Master, Preview, Count, List,
                    Get,  
                }.Concat(FilterList)
            },
            { "Thêm", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    Detail, Create, 
                }.Concat(SingleList).Concat(FilterList).Concat(CountList)
            },

            { "Sửa", new List<string> { 
                    Parent,            
                    Master, Preview, Count, List, Get,
                    Detail, Update, 
                }.Concat(SingleList).Concat(FilterList).Concat(CountList)
            },

            { "Xoá", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    Delete, 
                }.Concat(SingleList).Concat(FilterList) 
            },

            { "Xoá nhiều", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    BulkDelete 
                }.Concat(FilterList) 
            },

            { "Xuất excel", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    Export 
                }.Concat(FilterList) 
            },

            { "Nhập excel", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    ExportTemplate, Import 
                }.Concat(FilterList) 
            },
        };
    }
}
