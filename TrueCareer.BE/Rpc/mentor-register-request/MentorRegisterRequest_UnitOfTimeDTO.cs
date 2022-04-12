using TrueCareer.Entities;
using TrueSight.Common;


namespace TrueCareer.Rpc.mentor_register_request
{
    public class MentorRegisterRequest_UnitOfTimeDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public long? StartAt { get; set; }
        public long? EndAt { get; set; }

        public MentorRegisterRequest_UnitOfTimeDTO(UnitOfTime UnitOfTime)
        {
            this.Id = UnitOfTime.Id;
            this.Code = UnitOfTime.Code;
            this.Name = UnitOfTime.Name;
            this.StartAt = UnitOfTime?.StartAt;
            this.EndAt = UnitOfTime?.EndAt;
            this.Informations = UnitOfTime.Informations;
            this.Warnings = UnitOfTime.Warnings;
            this.Errors = UnitOfTime.Errors;
        }
    }
}