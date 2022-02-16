using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.app_user
{
    public class AppUser_InformationDTO : DataDTO
    {
        public long Id { get; set; }
        public long InformationTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartAt { get; set; }
        public string Role { get; set; }
        public string Image { get; set; }
        public long TopicId { get; set; }
        public long UserId { get; set; }
        public DateTime EndAt { get; set; }
        public AppUser_InformationTypeDTO InformationType { get; set; }   
        public AppUser_TopicDTO Topic { get; set; }   
        public Guid RowId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public AppUser_InformationDTO() {}
        public AppUser_InformationDTO(Information Information)
        {
            this.Id = Information.Id;
            this.InformationTypeId = Information.InformationTypeId;
            this.Name = Information.Name;
            this.Description = Information.Description;
            this.StartAt = Information.StartAt;
            this.Role = Information.Role;
            this.Image = Information.Image;
            this.TopicId = Information.TopicId;
            this.UserId = Information.UserId;
            this.EndAt = Information.EndAt;
            this.InformationType = Information.InformationType == null ? null : new AppUser_InformationTypeDTO(Information.InformationType);
            this.Topic = Information.Topic == null ? null : new AppUser_TopicDTO(Information.Topic);
            this.RowId = Information.RowId;
            this.Errors = Information.Errors;
        }
    }

    public class AppUser_InformationFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter InformationTypeId { get; set; }
        
        public StringFilter Name { get; set; }
        
        public StringFilter Description { get; set; }
        
        public DateFilter StartAt { get; set; }
        
        public StringFilter Role { get; set; }
        
        public StringFilter Image { get; set; }
        
        public IdFilter TopicId { get; set; }
        
        public IdFilter UserId { get; set; }
        
        public DateFilter EndAt { get; set; }
        
        public InformationOrder OrderBy { get; set; }
    }
}