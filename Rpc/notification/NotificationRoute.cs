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
using TrueCareer.Services.MNotification;
using TrueCareer.Services.MAppUser;

namespace TrueCareer.Rpc.notification
{
    public class NotificationRoute : Root
    {
        public const string Parent = Module + "/notification";
        public const string Master = Module + "/notification/notification-master";
        public const string Detail = Module + "/notification/notification-detail";
        public const string Preview = Module + "/notification/notification-preview";
        private const string Default = Rpc + Module + "/notification";
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
        
        public const string FilterListAppUser = Default + "/filter-list-app-user";

        public const string SingleListAppUser = Default + "/single-list-app-user";


        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(NotificationFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(NotificationFilter.TitleWeb), FieldTypeEnum.STRING.Id },
            { nameof(NotificationFilter.ContentWeb), FieldTypeEnum.STRING.Id },
            { nameof(NotificationFilter.SenderId), FieldTypeEnum.ID.Id },
            { nameof(NotificationFilter.RecipientId), FieldTypeEnum.ID.Id },
            { nameof(NotificationFilter.Time), FieldTypeEnum.DATE.Id },
            { nameof(NotificationFilter.LinkWebsite), FieldTypeEnum.STRING.Id },
        };

        private static List<string> FilterList = new List<string> { 
            FilterListAppUser,
        };
        private static List<string> SingleList = new List<string> { 
            SingleListAppUser, 
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
