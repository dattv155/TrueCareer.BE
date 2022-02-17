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
using TrueCareer.Services.MComment;
using TrueCareer.Services.MAppUser;

namespace TrueCareer.Rpc.comment
{
    public partial class CommentController 
    {
        [Route(CommentRoute.FilterListAppUser), HttpPost]
        public async Task<List<Comment_AppUserDTO>> FilterListAppUser([FromBody] Comment_AppUserFilterDTO Comment_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = Comment_AppUserFilterDTO.Id;
            AppUserFilter.Username = Comment_AppUserFilterDTO.Username;
            AppUserFilter.Email = Comment_AppUserFilterDTO.Email;
            AppUserFilter.Phone = Comment_AppUserFilterDTO.Phone;
            AppUserFilter.Password = Comment_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = Comment_AppUserFilterDTO.DisplayName;
            AppUserFilter.SexId = Comment_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = Comment_AppUserFilterDTO.Birthday;
            AppUserFilter.Avatar = Comment_AppUserFilterDTO.Avatar;
            AppUserFilter.CoverImage = Comment_AppUserFilterDTO.CoverImage;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<Comment_AppUserDTO> Comment_AppUserDTOs = AppUsers
                .Select(x => new Comment_AppUserDTO(x)).ToList();
            return Comment_AppUserDTOs;
        }
    }
}

