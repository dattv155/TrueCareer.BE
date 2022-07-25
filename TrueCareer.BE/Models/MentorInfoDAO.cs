using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class MentorInfoDAO
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public long ConnectionId { get; set; }
        public string ConnectionUrl { get; set; }
        public long MajorId { get; set; }
        public string TopicDescription { get; set; }
    }
}
