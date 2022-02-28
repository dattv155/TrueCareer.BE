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
        public const string Register = "rpc/truecareer/account/register";
        public const string Get = "rpc/truecareer/profile/get";
        public const string Update = "rpc/truecareer/profile/update";
        public const string SingleListSex = "rpc/truecareer/profile/single-list-sex";
        public const string ChangePassword = "rpc/truecareer/profile/change-password";
        public const string ForgotPassword = "rpc/truecareer/profile/forgot-password";
        public const string VerifyOtpCode = "rpc/truecareer/profile/verify-otp-code";
        public const string RecoveryPassword = "rpc/truecareer/profile/recovery-password";

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
        [AllowAnonymous]
        [Route(ProfileRoot.Register), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> Register([FromBody] AppUser_RegisterDTO AppUser_RegisterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            AppUser AppUser = new AppUser
            {
                Username = AppUser_RegisterDTO.Username,
                Password = AppUser_RegisterDTO.Password,
                Phone = AppUser_RegisterDTO.Phone,
                Email = AppUser_RegisterDTO.Email,
                DisplayName = AppUser_RegisterDTO.DisplayName,
                SexId = 1,
                Avatar= "https://picsum.photos/200",
                CoverImage= "https://picsum.photos/200/300",
                PasswordConfirmation = AppUser_RegisterDTO.PasswordConfirmation
            };
            AppUser = await AppUserService.Register(AppUser);
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
            {
                return AppUser_AppUserDTO;
            }
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [Route(ProfileRoot.ChangePassword), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> ChangePassword([FromBody] AppUser_ProfileChangePasswordDTO AppUser_ProfileChangePasswordDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            if (!IsAppUser())
                return Unauthorized();
            this.CurrentContext.UserId = ExtractUserId();
            AppUser AppUser = new AppUser
            {
                Id = CurrentContext.UserId,
                Password = AppUser_ProfileChangePasswordDTO.OldPassword,
                NewPassword = AppUser_ProfileChangePasswordDTO.NewPassword,
            };
            AppUser.BaseLanguage = CurrentContext.Language;
            AppUser = await AppUserService.ChangePassword(AppUser);
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
                return AppUser_AppUserDTO;
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        #region Forgot Password
        [AllowAnonymous]
        [Route(ProfileRoot.ForgotPassword), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> ForgotPassword([FromBody] AppUser_ForgotPassword AppUser_ForgotPassword)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUser AppUser = new AppUser
            {
                Email = AppUser_ForgotPassword.Email,
            };
            AppUser.BaseLanguage = CurrentContext.Language;

            AppUser = await AppUserService.ForgotPassword(AppUser);
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
            {
                return AppUser_AppUserDTO;
            }
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [AllowAnonymous]
        [Route(ProfileRoot.VerifyOtpCode), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> VerifyCode([FromBody] AppUser_VerifyOtpDTO AppUser_VerifyOtpDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUser AppUser = new AppUser
            {
                Email = AppUser_VerifyOtpDTO.Email,
                OtpCode = AppUser_VerifyOtpDTO.OtpCode,
            };
            AppUser.BaseLanguage = CurrentContext.Language;
            AppUser = await AppUserService.VerifyOtpCode(AppUser);
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
            {
                HttpContext.Response.Cookies.Append("Token", AppUser.Token);
                return AppUser_AppUserDTO;
            }

            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [Route(ProfileRoot.RecoveryPassword), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> RecoveryPassword([FromBody] AppUser_RecoveryPassword AppUser_RecoveryPassword)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            if (!IsAppUser())
                return Unauthorized();
            var UserId = ExtractUserId();
            AppUser AppUser = new AppUser
            {
                Id = UserId,
                Password = AppUser_RecoveryPassword.Password,
            };
            AppUser.BaseLanguage = CurrentContext.Language;
            AppUser = await AppUserService.RecoveryPassword(AppUser);
            if (AppUser == null)
                return Unauthorized();
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            return AppUser_AppUserDTO;
        }
        #endregion

        [Route(ProfileRoot.Update), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> UpdateMe([FromBody] AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            if (!IsAppUser())
                return Unauthorized();
            this.CurrentContext.UserId = ExtractUserId();
            AppUser OldData = await AppUserService.Get(this.CurrentContext.UserId);
            AppUser AppUser = ConvertDTOToEntity(AppUser_AppUserDTO);
            AppUser.Id = CurrentContext.UserId;
            AppUser.AppUserSiteMappings = OldData.AppUserSiteMappings;
            AppUser = await AppUserService.Update(AppUser);
            AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
                return AppUser_AppUserDTO;
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [Route(ProfileRoot.SingleListSex), HttpPost]
        public async Task<List<AppUser_SexDTO>> SingleListSex([FromBody] AppUser_SexFilterDTO AppUser_SexFilterDTO)
        {
            SexFilter SexFilter = new SexFilter();
            SexFilter.Skip = 0;
            SexFilter.Take = 20;
            SexFilter.OrderBy = SexOrder.Id;
            SexFilter.OrderType = OrderType.ASC;
            SexFilter.Selects = SexSelect.ALL;
            SexFilter.Id = AppUser_SexFilterDTO.Id;
            SexFilter.Code = AppUser_SexFilterDTO.Code;
            SexFilter.Name = AppUser_SexFilterDTO.Name;
            List<Sex> Sexes = await SexService.List(SexFilter);
            List<AppUser_SexDTO> AppUser_SexDTOs = Sexes
                .Select(x => new AppUser_SexDTO(x)).ToList();
            return AppUser_SexDTOs;
        }


        private long ExtractUserId()
        {
            return long.TryParse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value, out long u) ? u : 0;
        }

    }
}
