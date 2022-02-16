using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.information
{
    public class Information_TopicDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public double? Cost { get; set; }
        
        public Information_TopicDTO() {}
        public Information_TopicDTO(Topic Topic)
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

    public class Information_TopicFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Title { get; set; }
        
        public StringFilter Description { get; set; }
        
        public Decimal Cost { get; set; }
        
        public TopicOrder OrderBy { get; set; }
    }
}