using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Common;
using TrueCareer.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using TrueCareer.Entities;
using TrueCareer.Services.MMentorReview;
using TrueCareer.Services.MAppUser;

namespace TrueCareer.Rpc.mentor_review
{
    public partial class MentorReviewController 
    {
        [Route(MentorReviewRoute.FilterListAppUser), HttpPost]
        public async Task<List<MentorReview_AppUserDTO>> FilterListAppUser([FromBody] MentorReview_AppUserFilterDTO MentorReview_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = MentorReview_AppUserFilterDTO.Id;
            AppUserFilter.Username = MentorReview_AppUserFilterDTO.Username;
            AppUserFilter.Email = MentorReview_AppUserFilterDTO.Email;
            AppUserFilter.Phone = MentorReview_AppUserFilterDTO.Phone;
            AppUserFilter.Password = MentorReview_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = MentorReview_AppUserFilterDTO.DisplayName;
            AppUserFilter.SexId = MentorReview_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = MentorReview_AppUserFilterDTO.Birthday;
            AppUserFilter.Avatar = MentorReview_AppUserFilterDTO.Avatar;
            AppUserFilter.CoverImage = MentorReview_AppUserFilterDTO.CoverImage;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<MentorReview_AppUserDTO> MentorReview_AppUserDTOs = AppUsers
                .Select(x => new MentorReview_AppUserDTO(x)).ToList();
            return MentorReview_AppUserDTOs;
        }
    }
}

