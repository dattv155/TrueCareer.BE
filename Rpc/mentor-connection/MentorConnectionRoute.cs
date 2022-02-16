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
using TrueCareer.Services.MMentorConnection;
using TrueCareer.Services.MConnectionType;
using TrueCareer.Services.MAppUser;

namespace TrueCareer.Rpc.mentor_connection
{
    public class MentorConnectionRoute : Root
    {
        public const string Parent = Module + "/mentor-connection";
        public const string Master = Module + "/mentor-connection/mentor-connection-master";
        public const string Detail = Module + "/mentor-connection/mentor-connection-detail";
        public const string Preview = Module + "/mentor-connection/mentor-connection-preview";
        private const string Default = Rpc + Module + "/mentor-connection";
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
        
        public const string FilterListConnectionType = Default + "/filter-list-connection-type";
        public const string FilterListAppUser = Default + "/filter-list-app-user";

        public const string SingleListConnectionType = Default + "/single-list-connection-type";
        public const string SingleListAppUser = Default + "/single-list-app-user";


        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(MentorConnectionFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(MentorConnectionFilter.MentorId), FieldTypeEnum.ID.Id },
            { nameof(MentorConnectionFilter.Url), FieldTypeEnum.STRING.Id },
            { nameof(MentorConnectionFilter.ConnectionTypeId), FieldTypeEnum.ID.Id },
        };

        private static List<string> FilterList = new List<string> { 
            FilterListConnectionType,FilterListAppUser,
        };
        private static List<string> SingleList = new List<string> { 
            SingleListConnectionType, SingleListAppUser, 
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
