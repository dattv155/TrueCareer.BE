using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Entities;
using TrueSight.Common;

namespace TrueCareer.Rpc.active_time
{
    public class ActiveTime_UnitOfTimeDTO:DataDTO
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public ActiveTime_UnitOfTimeDTO() { }
        public ActiveTime_UnitOfTimeDTO(UnitOfTime UnitOfTime)
        {

            this.Id = UnitOfTime.Id;

            this.Code = UnitOfTime.Code;

            this.Name = UnitOfTime.Name;

            this.Informations = UnitOfTime.Informations;
            this.Warnings = UnitOfTime.Warnings;
            this.Errors = UnitOfTime.Errors;
        }
    }
}
