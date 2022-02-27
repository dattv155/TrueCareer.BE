using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Common;
using TrueCareer.Services.MAppUser;
using TrueSight.Common;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.app_user
{
    public class ProfileRoot
    {
        public const string Login = "rpc/truecarreer/account/login";

    }
    [Authorize]
    public class ProfileController:ControllerBase
    {
        private IAppUserService AppUserService;
        private ICurrentContext CurrentContext;
        public ProfileController(
           IAppUserService AppUserService,
           ICurrentContext CurrentContext
           )
        {
            this.AppUserService = AppUserService;
            this.CurrentContext = CurrentContext;
        }
        [AllowAnonymous]
        [Route(ProfileRoot.Login), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> Login([FromBody] AppUser_LoginDTO AppUser_LoginDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            AppUser AppUser = new AppUser
            {
                Username = AppUser_LoginDTO.Username,
                Password = AppUser_LoginDTO.Password,
                BaseLanguage = "vi",
            };
            AppUser.BaseLanguage = CurrentContext.Language;
            AppUser = await AppUserService.Login(AppUser);
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);

            if (AppUser.IsValidated)
            {
                Response.Cookies.Append("Token", AppUser.Token);
                AppUser_AppUserDTO.Token = AppUser.Token;
                return AppUser_AppUserDTO;
            }
            else
                return BadRequest(AppUser_AppUserDTO);
        }
    }
}
