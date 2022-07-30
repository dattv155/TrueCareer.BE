using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueSight.Common;
using Microsoft.AspNetCore.Mvc;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.mentor_register_request
{
    public partial class MentorRegisterRequestController
    {
        [Route(MentorRegisterRequestRoute.SingleListConnectionType), HttpPost]
        public async Task<List<MentorRegisterRequest_ConnectionTypeDTO>> SingleListConnectionType([FromBody] MentorRegisterRequest_ConnectionTypeFilterDTO MentorRegisterRequest_ConnectionTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ConnectionTypeFilter ConnectionTypeFilter = new ConnectionTypeFilter();
            ConnectionTypeFilter.Skip = 0;
            ConnectionTypeFilter.Take = 20;
            ConnectionTypeFilter.OrderBy = ConnectionTypeOrder.Id;
            ConnectionTypeFilter.OrderType = OrderType.ASC;
            ConnectionTypeFilter.Selects = ConnectionTypeSelect.ALL;

            List<ConnectionType> ConnectionTypes = await ConnectionTypeService.List(ConnectionTypeFilter);
            List<MentorRegisterRequest_ConnectionTypeDTO> MentorRegisterRequest_ConnectionTypeDTOs = ConnectionTypes
                .Select(x => new MentorRegisterRequest_ConnectionTypeDTO(x)).ToList();
            return MentorRegisterRequest_ConnectionTypeDTOs;
        }

        [Route(MentorRegisterRequestRoute.SingleListMajor), HttpPost]
        public async Task<List<MentorRegisterRequest_MajorDTO>> SingleListMajor([FromBody] MentorRegisterRequest_MajorFilterDTO MentorRegisterRequest_MajorFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MajorFilter MajorFilter = new MajorFilter();
            MajorFilter.Skip = 0;
            MajorFilter.Take = 20;
            MajorFilter.OrderBy = MajorOrder.Id;
            MajorFilter.OrderType = OrderType.ASC;
            MajorFilter.Selects = MajorSelect.ALL;

            List<Major> Majors = await MajorService.List(MajorFilter);
            List<MentorRegisterRequest_MajorDTO> MentorRegisterRequest_MajorDTOs = Majors
                .Select(x => new MentorRegisterRequest_MajorDTO(x)).ToList();
            return MentorRegisterRequest_MajorDTOs;
        }

        [Route(MentorRegisterRequestRoute.SingleListMajor), HttpPost]
        public async Task<List<MentorRegisterRequest_UnitOfTimeDTO>> SingleListUnitOfTime([FromBody] MentorRegisterRequest_UnitOfTimeFilterDTO MentorRegisterRequest_UnitOfTimeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            UnitOfTimeFilter UnitOfTimeFilter = new UnitOfTimeFilter();
            UnitOfTimeFilter.Skip = 0;
            UnitOfTimeFilter.Take = 20;
            UnitOfTimeFilter.OrderBy = UnitOfTimeOrder.Id;
            UnitOfTimeFilter.OrderType = OrderType.ASC;
            UnitOfTimeFilter.Selects = UnitOfTimeSelect.ALL;

            List<UnitOfTime> UnitOfTimes = await UnitOfTimeService.List(UnitOfTimeFilter);
            List<MentorRegisterRequest_UnitOfTimeDTO> MentorRegisterRequest_UnitOfTimeDTOs = UnitOfTimes
                .Select(x => new MentorRegisterRequest_UnitOfTimeDTO(x)).ToList();
            return MentorRegisterRequest_UnitOfTimeDTOs;
        }
    }
}
