using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TrueSight.Common;
using TrueCareer.Entities;
using TrueCareer.Services.MMentorRegisterRequest;
using TrueCareer.Services.MMentor;


namespace TrueCareer.Rpc.mentor
{
    public class MentorRoute : Root
    {
        private const string Default = Rpc + Module + "/mentor";
        public const string List = Default + "/list";
    }

    public class MentorController : RpcController
    {
        private IMentorService MentorService;

        public MentorController(IMentorService MentorService)
        {
            this.MentorService = MentorService;
        }


        [Route(MentorRoute.List), HttpPost]
        public async Task<ActionResult<List<Mentor_MentorDTO>>> List([FromBody] Mentor_MentorFilterDTO Mentor_MentorFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = Mentor_MentorFilterDTO.Id;
            AppUserFilter.Address = Mentor_MentorFilterDTO.Address;
            AppUserFilter.JobRole = Mentor_MentorFilterDTO.JobRole;
            AppUserFilter.DisplayName = Mentor_MentorFilterDTO.DisplayName;
            AppUserFilter.MajorId = Mentor_MentorFilterDTO.MajorId;
            AppUserFilter.RoleId = Mentor_MentorFilterDTO.RoleId;

            List<AppUser> AppUsers = await MentorService.List(AppUserFilter);
            List<Mentor_MentorDTO> Mentor_MentorDTOs = AppUsers
                .Select(c => new Mentor_MentorDTO(c)).ToList();
            return Mentor_MentorDTOs;
        }
    }
}