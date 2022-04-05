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
    }
}
