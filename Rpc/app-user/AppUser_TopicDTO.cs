using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.app_user
{
    public class AppUser_TopicDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public double? Cost { get; set; }
        
        public AppUser_TopicDTO() {}
        public AppUser_TopicDTO(Topic Topic)
        {
            
            this.Id = Topic.Id;
            
            this.Title = Topic.Title;
            
            this.Description = Topic.Description;
            
            this.Cost = Topic.Cost;
            
            this.Informations = Topic.Informations;
            this.Warnings = Topic.Warnings;
            this.Errors = Topic.Errors;
        }
    }

    public class AppUser_TopicFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Title { get; set; }
        
        public StringFilter Description { get; set; }
        
        public DecimalFilter Cost { get; set; }
        
        public TopicOrder OrderBy { get; set; }
    }
}