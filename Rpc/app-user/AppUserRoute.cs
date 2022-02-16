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
using TrueCareer.Services.MAppUser;
using TrueCareer.Services.MSex;
using TrueCareer.Services.MFavouriteMentor;
using TrueCareer.Services.MFavouriteNews;
using TrueCareer.Services.MNews;
using TrueCareer.Services.MInformation;
using TrueCareer.Services.MInformationType;
using TrueCareer.Services.MTopic;

namespace TrueCareer.Rpc.app_user
{
    public class AppUserRoute : Root
    {
        public const string Parent = Module + "/app-user";
        public const string Master = Module + "/app-user/app-user-master";
        public const string Detail = Module + "/app-user/app-user-detail";
        public const string Preview = Module + "/app-user/app-user-preview";
        private const string Default = Rpc + Module + "/app-user";
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
        
        public const string FilterListSex = Default + "/filter-list-sex";
        public const string FilterListFavouriteMentor = Default + "/filter-list-favourite-mentor";
        public const string FilterListFavouriteNews = Default + "/filter-list-favourite-news";
        public const string FilterListNews = Default + "/filter-list-news";
        public const string FilterListInformation = Default + "/filter-list-information";
        public const string FilterListInformationType = Default + "/filter-list-information-type";
        public const string FilterListTopic = Default + "/filter-list-topic";

        public const string SingleListSex = Default + "/single-list-sex";
        public const string SingleListFavouriteMentor = Default + "/single-list-favourite-mentor";
        public const string SingleListFavouriteNews = Default + "/single-list-favourite-news";
        public const string SingleListNews = Default + "/single-list-news";
        public const string SingleListInformation = Default + "/single-list-information";
        public const string SingleListInformationType = Default + "/single-list-information-type";
        public const string SingleListTopic = Default + "/single-list-topic";


        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(AppUserFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(AppUserFilter.Username), FieldTypeEnum.STRING.Id },
            { nameof(AppUserFilter.Email), FieldTypeEnum.STRING.Id },
            { nameof(AppUserFilter.Phone), FieldTypeEnum.STRING.Id },
            { nameof(AppUserFilter.Password), FieldTypeEnum.STRING.Id },
            { nameof(AppUserFilter.DisplayName), FieldTypeEnum.STRING.Id },
            { nameof(AppUserFilter.SexId), FieldTypeEnum.ID.Id },
            { nameof(AppUserFilter.Birthday), FieldTypeEnum.DATE.Id },
            { nameof(AppUserFilter.Avatar), FieldTypeEnum.STRING.Id },
            { nameof(AppUserFilter.CoverImage), FieldTypeEnum.STRING.Id },
        };

        private static List<string> FilterList = new List<string> { 
            FilterListSex,FilterListFavouriteMentor,FilterListFavouriteNews,FilterListNews,FilterListInformation,FilterListInformationType,FilterListTopic,
        };
        private static List<string> SingleList = new List<string> { 
            SingleListSex, SingleListFavouriteMentor, SingleListFavouriteNews, SingleListNews, SingleListInformation, SingleListInformationType, SingleListTopic, 
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
