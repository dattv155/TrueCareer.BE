using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Entities;
using TrueSight.Common;

namespace TrueCareer.Rpc.mentor_register_request
{
    public class MentorRegisterRequest_ConnectionTypeDTO:DataDTO
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public MentorRegisterRequest_ConnectionTypeDTO() { }
        public MentorRegisterRequest_ConnectionTypeDTO(ConnectionType ConnectionType)
        {

            this.Id = ConnectionType.Id;

            this.Code = ConnectionType.Code;

            this.Name = ConnectionType.Name;

            this.Informations = ConnectionType.Informations;
            this.Warnings = ConnectionType.Warnings;
            this.Errors = ConnectionType.Errors;
        }
    }

    public class MentorRegisterRequest_ConnectionTypeFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public ConnectionTypeOrder OrderBy { get; set; }
    }
}

