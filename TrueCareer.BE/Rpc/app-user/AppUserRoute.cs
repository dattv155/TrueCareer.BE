using TrueSight.Common;
using System.Collections.Generic;
using System.Linq;
using TrueCareer.Entities;

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
        public const string CreateToken = Default + "/create-firebase-token";
        public const string DeleteToken = Default + "/delete-firebase-token";

        public const string FilterListSex = Default + "/filter-list-sex";

        public const string SingleListSex = Default + "/single-list-sex";


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
            FilterListSex,
        };
        private static List<string> SingleList = new List<string> {
            SingleListSex,
        };
        private static List<string> CountList = new List<string>
        {

        };

        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "T??m ki???m", new List<string> {
                    Parent,
                    Master, Preview, Count, List,
                    Get,
                }.Concat(FilterList)
            },
            { "Th??m", new List<string> {
                    Parent,
                    Master, Preview, Count, List, Get,
                    Detail, Create,
                }.Concat(SingleList).Concat(FilterList).Concat(CountList)
            },

            { "S???a", new List<string> {
                    Parent,
                    Master, Preview, Count, List, Get,
                    Detail, Update,
                }.Concat(SingleList).Concat(FilterList).Concat(CountList)
            },

            { "Xo??", new List<string> {
                    Parent,
                    Master, Preview, Count, List, Get,
                    Delete,
                }.Concat(SingleList).Concat(FilterList)
            },

            { "Xo?? nhi???u", new List<string> {
                    Parent,
                    Master, Preview, Count, List, Get,
                    BulkDelete
                }.Concat(FilterList)
            },

            { "Xu???t excel", new List<string> {
                    Parent,
                    Master, Preview, Count, List, Get,
                    Export
                }.Concat(FilterList)
            },

            { "Nh???p excel", new List<string> {
                    Parent,
                    Master, Preview, Count, List, Get,
                    ExportTemplate, Import
                }.Concat(FilterList)
            },
        };
    }
}
