using TrueSight.Common;
using TrueCareer.Entities;
namespace TrueCareer.Rpc.mentor_register_request
{
    public class MentorRegisterRequest_MentorConnectionDTO : DataDTO
    {
        public long Id { get; set; }
        public long MentorId { get; set; }
        public string Url { get; set; }
        public long ConnectionTypeId { get; set; }
        public MentorRegisterRequest_ConnectionTypeDTO ConnectionType { get; set; }
        public MentorRegisterRequest_AppUserDTO Mentor { get; set; }

        public MentorRegisterRequest_MentorConnectionDTO(MentorConnection MentorConnection)
        {
            this.Id = MentorConnection.Id;
            this.MentorId = MentorConnection.MentorId;
            this.Url = MentorConnection.Url;
            this.ConnectionTypeId = MentorConnection.ConnectionTypeId;
        }
    }

    public class MentorRegisterRequest_MentorConnectionFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter MentorId { get; set; }
        public StringFilter Url { get; set; }
        public IdFilter ConnectionTypeId { get; set; }
    }
}
