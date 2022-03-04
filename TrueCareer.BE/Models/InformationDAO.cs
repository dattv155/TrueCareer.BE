using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class InformationDAO
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
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid RowId { get; set; }

        public virtual InformationTypeDAO InformationType { get; set; }
        public virtual TopicDAO Topic { get; set; }
        public virtual AppUserDAO User { get; set; }
    }
}
