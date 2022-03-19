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
using TrueCareer.Services.MInformation;
using TrueCareer.Services.MInformationType;
using TrueCareer.Services.MTopic;
using TrueCareer.Services.MAppUser;

namespace TrueCareer.Rpc.information
{
    public class InformationRoute : Root
    {
        public const string Parent = Module + "/information";
        public const string Master = Module + "/information/information-master";
        public const string Detail = Module + "/information/information-detail";
        public const string Preview = Module + "/information/information-preview";

        private const string Default = Rpc + Module + "/information";
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
        
        public const string FilterListInformationType = Default + "/filter-list-information-type";
        public const string FilterListTopic = Default + "/filter-list-topic";
        public const string FilterListAppUser = Default + "/filter-list-app-user";

        public const string SingleListInformationType = Default + "/single-list-information-type";
        public const string SingleListTopic = Default + "/single-list-topic";
        public const string SingleListAppUser = Default + "/single-list-app-user";

        public const string CountReview = Default + "/count-review";
        public const string ListReview = Default + "/list-review";
        public const string CountNews = Default + "/count-news";
        public const string ListNews = Default + "/list-news";
        public const string ListRecommendMentor = Default + "/list-recommend-mentor";
        public const string GetTopic = Default + "/get-topic";
        public const string UploadFile = Default + "/upload-file";
        public const string UploadImage = Default + "/upload-image";


        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(InformationFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(InformationFilter.InformationTypeId), FieldTypeEnum.ID.Id },
            { nameof(InformationFilter.Name), FieldTypeEnum.STRING.Id },
            { nameof(InformationFilter.Description), FieldTypeEnum.STRING.Id },
            { nameof(InformationFilter.StartAt), FieldTypeEnum.DATE.Id },
            { nameof(InformationFilter.Role), FieldTypeEnum.STRING.Id },
            { nameof(InformationFilter.Image), FieldTypeEnum.STRING.Id },
            { nameof(InformationFilter.TopicId), FieldTypeEnum.ID.Id },
            { nameof(InformationFilter.UserId), FieldTypeEnum.ID.Id },
            { nameof(InformationFilter.EndAt), FieldTypeEnum.DATE.Id },
        };

        private static List<string> FilterList = new List<string> { 
            FilterListInformationType,FilterListTopic,FilterListAppUser,
        };
        private static List<string> SingleList = new List<string> { 
            SingleListInformationType, SingleListTopic, SingleListAppUser, 
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
