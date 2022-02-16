using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.mentor_connection
{
    public class MentorConnection_MentorConnectionDTO : DataDTO
    {
        public long Id { get; set; }
        public long MentorId { get; set; }
        public string Url { get; set; }
        public long ConnectionTypeId { get; set; }
        public MentorConnection_ConnectionTypeDTO ConnectionType { get; set; }
        public MentorConnection_AppUserDTO Mentor { get; set; }
        public MentorConnection_MentorConnectionDTO() {}
        public MentorConnection_MentorConnectionDTO(MentorConnection MentorConnection)
        {
            this.Id = MentorConnection.Id;
            this.MentorId = MentorConnection.MentorId;
            this.Url = MentorConnection.Url;
            this.ConnectionTypeId = MentorConnection.ConnectionTypeId;
            this.ConnectionType = MentorConnection.ConnectionType == null ? null : new MentorConnection_ConnectionTypeDTO(MentorConnection.ConnectionType);
            this.Mentor = MentorConnection.Mentor == null ? null : new MentorConnection_AppUserDTO(MentorConnection.Mentor);
            this.Informations = MentorConnection.Informations;
            this.Warnings = MentorConnection.Warnings;
            this.Errors = MentorConnection.Errors;
        }
    }

    public class MentorConnection_MentorConnectionFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter MentorId { get; set; }
        public StringFilter Url { get; set; }
        public IdFilter ConnectionTypeId { get; set; }
        public MentorConnectionOrder OrderBy { get; set; }
    }
}
