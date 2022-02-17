using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.news
{
    public class News_NewsStatusDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        
        public News_NewsStatusDTO() {}
        public News_NewsStatusDTO(NewsStatus NewsStatus)
        {
            
            this.Id = NewsStatus.Id;
            
            this.Code = NewsStatus.Code;
            
            this.Name = NewsStatus.Name;
            
            this.Informations = NewsStatus.Informations;
            this.Warnings = NewsStatus.Warnings;
            this.Errors = NewsStatus.Errors;
        }
    }

    public class News_NewsStatusFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public NewsStatusOrder OrderBy { get; set; }
    }
}