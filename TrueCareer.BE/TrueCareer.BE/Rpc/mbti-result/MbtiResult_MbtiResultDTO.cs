using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.mbti_result
{
    public class MbtiResult_MbtiResultDTO : DataDTO
    {
        public long UserId { get; set; }
        public long MbtiPersonalTypeId { get; set; }
        public long Id { get; set; }
        public MbtiResult_MbtiPersonalTypeDTO MbtiPersonalType { get; set; }
        public MbtiResult_AppUserDTO User { get; set; }
        public MbtiResult_MbtiResultDTO() {}
        public MbtiResult_MbtiResultDTO(MbtiResult MbtiResult)
        {
            this.UserId = MbtiResult.UserId;
            this.MbtiPersonalTypeId = MbtiResult.MbtiPersonalTypeId;
            this.Id = MbtiResult.Id;
            this.MbtiPersonalType = MbtiResult.MbtiPersonalType == null ? null : new MbtiResult_MbtiPersonalTypeDTO(MbtiResult.MbtiPersonalType);
            this.User = MbtiResult.User == null ? null : new MbtiResult_AppUserDTO(MbtiResult.User);
            this.Informations = MbtiResult.Informations;
            this.Warnings = MbtiResult.Warnings;
            this.Errors = MbtiResult.Errors;
        }
    }

    public class MbtiResult_MbtiResultFilterDTO : FilterDTO
    {
        public IdFilter UserId { get; set; }
        public IdFilter MbtiPersonalTypeId { get; set; }
        public IdFilter Id { get; set; }
        public MbtiResultOrder OrderBy { get; set; }
    }
}
