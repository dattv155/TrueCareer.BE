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
using TrueCareer.Services.MChoice;
using TrueCareer.Services.MMbtiSingleType;
using TrueCareer.Services.MQuestion;

namespace TrueCareer.Rpc.choice
{
    public class ChoiceRoute : Root
    {
        public const string Parent = Module + "/choice";
        public const string Master = Module + "/choice/choice-master";
        public const string Detail = Module + "/choice/choice-detail";
        public const string Preview = Module + "/choice/choice-preview";
        private const string Default = Rpc + Module + "/choice";
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
        
        public const string FilterListMbtiSingleType = Default + "/filter-list-mbti-single-type";
        public const string FilterListQuestion = Default + "/filter-list-question";

        public const string SingleListMbtiSingleType = Default + "/single-list-mbti-single-type";
        public const string SingleListQuestion = Default + "/single-list-question";


        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(ChoiceFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(ChoiceFilter.ChoiceContent), FieldTypeEnum.STRING.Id },
            { nameof(ChoiceFilter.Description), FieldTypeEnum.STRING.Id },
            { nameof(ChoiceFilter.QuestionId), FieldTypeEnum.ID.Id },
            { nameof(ChoiceFilter.MbtiSingleTypeId), FieldTypeEnum.ID.Id },
        };

        private static List<string> FilterList = new List<string> { 
            FilterListMbtiSingleType,FilterListQuestion,
        };
        private static List<string> SingleList = new List<string> { 
            SingleListMbtiSingleType, SingleListQuestion, 
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
