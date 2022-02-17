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
using TrueCareer.Services.MQuestion;
using TrueCareer.Services.MChoice;
using TrueCareer.Services.MMbtiSingleType;

namespace TrueCareer.Rpc.question
{
    public class QuestionRoute : Root
    {
        public const string Parent = Module + "/question";
        public const string Master = Module + "/question/question-master";
        public const string Detail = Module + "/question/question-detail";
        public const string Preview = Module + "/question/question-preview";
        private const string Default = Rpc + Module + "/question";
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
        
        public const string FilterListChoice = Default + "/filter-list-choice";
        public const string FilterListMbtiSingleType = Default + "/filter-list-mbti-single-type";

        public const string SingleListChoice = Default + "/single-list-choice";
        public const string SingleListMbtiSingleType = Default + "/single-list-mbti-single-type";


        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(QuestionFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(QuestionFilter.QuestionContent), FieldTypeEnum.STRING.Id },
            { nameof(QuestionFilter.Description), FieldTypeEnum.STRING.Id },
        };

        private static List<string> FilterList = new List<string> { 
            FilterListChoice,FilterListMbtiSingleType,
        };
        private static List<string> SingleList = new List<string> { 
            SingleListChoice, SingleListMbtiSingleType, 
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
