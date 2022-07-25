using System.Collections.Generic;
using TrueCareer.Entities;
using TrueSight.Common;

namespace TrueCareer.BE.Entities
{
    public class MentorInfo : DataEntity
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public long ConnectionId { get; set; }
        public string ConnectionUrl { get; set; }
        public long MajorId { get; set; }
        public string TopicDescription { get; set; }
        public List<ActiveTime> ActiveTimes { get; set; }
    }

    public class MentorInfoFilter : FilterEntity
    {
        public IdFilter AppUserId { get; set; }
        public IdFilter MajorId { get; set; }
    }
}
