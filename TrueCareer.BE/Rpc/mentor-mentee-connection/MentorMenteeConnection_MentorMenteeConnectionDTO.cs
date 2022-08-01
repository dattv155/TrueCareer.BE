using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.mentor_mentee_connection
{
    public class MentorMenteeConnection_MentorMenteeConnectionDTO : DataDTO
    {
        public long MentorId { get; set; }
        public long MenteeId { get; set; }
        public long? ConnectionId { get; set; }
        public string FirstMessage { get; set; }
        public long ConnectionStatusId { get; set; }
        public long ActiveTimeId { get; set; }
        public long Id { get; set; }
        public MentorMenteeConnection_MentorConnectionDTO Connection { get; set; }
        public MentorMenteeConnection_ConnectionStatusDTO ConnectionStatus { get; set; }
        public MentorMenteeConnection_AppUserDTO Mentee { get; set; }
        public MentorMenteeConnection_AppUserDTO Mentor { get; set; }
        public MentorMenteeConnection_MentorMenteeConnectionDTO() { }
        public MentorMenteeConnection_MentorMenteeConnectionDTO(MentorMenteeConnection MentorMenteeConnection)
        {
            this.MentorId = MentorMenteeConnection.MentorId;
            this.MenteeId = MentorMenteeConnection.MenteeId;
            this.ConnectionId = MentorMenteeConnection.ConnectionId;
            this.FirstMessage = MentorMenteeConnection.FirstMessage;
            this.ConnectionStatusId = MentorMenteeConnection.ConnectionStatusId;
            this.ActiveTimeId = MentorMenteeConnection.ActiveTimeId;
            this.Id = MentorMenteeConnection.Id;
            this.Connection = MentorMenteeConnection.Connection == null ? null : new MentorMenteeConnection_MentorConnectionDTO(MentorMenteeConnection.Connection);
            this.ConnectionStatus = MentorMenteeConnection.ConnectionStatus == null ? null : new MentorMenteeConnection_ConnectionStatusDTO(MentorMenteeConnection.ConnectionStatus);
            this.Mentee = MentorMenteeConnection.Mentee == null ? null : new MentorMenteeConnection_AppUserDTO(MentorMenteeConnection.Mentee);
            this.Mentor = MentorMenteeConnection.Mentor == null ? null : new MentorMenteeConnection_AppUserDTO(MentorMenteeConnection.Mentor);
            this.Informations = MentorMenteeConnection.Informations;
            this.Warnings = MentorMenteeConnection.Warnings;
            this.Errors = MentorMenteeConnection.Errors;
        }
    }

    public class MentorMenteeConnection_MentorMenteeConnectionFilterDTO : FilterDTO
    {
        public IdFilter MentorId { get; set; }
        public IdFilter MenteeId { get; set; }
        public IdFilter ConnectionId { get; set; }
        public StringFilter FirstMessage { get; set; }
        public IdFilter ConnectionStatusId { get; set; }
        public IdFilter ActiveTimeId { get; set; }
        public IdFilter Id { get; set; }
        public MentorMenteeConnectionOrder OrderBy { get; set; }
    }
}
