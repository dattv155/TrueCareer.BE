using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer;
using TrueCareer.Common;
using TrueCareer.Enums;
using TrueCareer.Entities;
using TrueCareer.Repositories;
using System.Security.Cryptography;

namespace TrueCareer.Services.MAppUser
{
    public interface IAppUserValidator : IServiceScoped
    {
        Task Get(AppUser AppUser);
        Task<bool> Create(AppUser AppUser);
        Task<bool> Update(AppUser AppUser);
        Task<bool> Delete(AppUser AppUser);
        Task<bool> BulkDelete(List<AppUser> AppUsers);
        Task<bool> Import(List<AppUser> AppUsers);
        Task<bool> Login(AppUser AppUser);
        Task<bool> Register(AppUser AppUser);
    }

    public class AppUserValidator : IAppUserValidator
    {

        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private AppUserMessage AppUserMessage;

        public AppUserValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.AppUserMessage = new AppUserMessage();
        }

       
        private async Task<bool> ValidateId(AppUser AppUser)
        {
            AppUserFilter AppUserFilter = new AppUserFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = AppUser.Id },
                Selects = AppUserSelect.Id
            };

            int count = await UOW.AppUserRepository.CountAll(AppUserFilter);
            if (count == 0)
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Id), AppUserMessage.Error.IdNotExisted, AppUserMessage);
            return AppUser.IsValidated;
        }

        private async Task<bool> ValidateUsername(AppUser AppUser)
        {
            if(string.IsNullOrEmpty(AppUser.Username))
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Username), AppUserMessage.Error.UsernameEmpty, AppUserMessage);
            }
            else if(AppUser.Username.Count() > 500)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Username), AppUserMessage.Error.UsernameOverLength, AppUserMessage);
            }
            return AppUser.IsValidated;
        }
        private async Task<bool> ValidateEmail(AppUser AppUser)
        {
            if(string.IsNullOrEmpty(AppUser.Email))
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Email), AppUserMessage.Error.EmailEmpty, AppUserMessage);
            }
            else if(AppUser.Email.Count() > 500)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Email), AppUserMessage.Error.EmailOverLength, AppUserMessage);
            }
            return AppUser.IsValidated;
        }
        private async Task<bool> ValidatePhone(AppUser AppUser)
        {
            if(string.IsNullOrEmpty(AppUser.Phone))
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Phone), AppUserMessage.Error.PhoneEmpty, AppUserMessage);
            }
            else if(AppUser.Phone.Count() > 500)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Phone), AppUserMessage.Error.PhoneOverLength, AppUserMessage);
            }
            return AppUser.IsValidated;
        }
        private async Task<bool> ValidatePassword(AppUser AppUser)
        {
            if(string.IsNullOrEmpty(AppUser.Password))
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Password), AppUserMessage.Error.PasswordEmpty, AppUserMessage);
            }
            else if(AppUser.Password.Count() > 500)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Password), AppUserMessage.Error.PasswordOverLength, AppUserMessage);
            }
            return AppUser.IsValidated;
        }
        private async Task<bool> ValidateDisplayName(AppUser AppUser)
        {
            if(string.IsNullOrEmpty(AppUser.DisplayName))
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.DisplayName), AppUserMessage.Error.DisplayNameEmpty, AppUserMessage);
            }
            else if(AppUser.DisplayName.Count() > 500)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.DisplayName), AppUserMessage.Error.DisplayNameOverLength, AppUserMessage);
            }
            return AppUser.IsValidated;
        }
        private async Task<bool> ValidateBirthday(AppUser AppUser)
        {       
            if(AppUser.Birthday.HasValue && AppUser.Birthday <= new DateTime(2000, 1, 1))
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Birthday), AppUserMessage.Error.BirthdayEmpty, AppUserMessage);
            }
            return true;
        }
        private async Task<bool> ValidateAvatar(AppUser AppUser)
        {
            if(string.IsNullOrEmpty(AppUser.Avatar))
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Avatar), AppUserMessage.Error.AvatarEmpty, AppUserMessage);
            }
            else if(AppUser.Avatar.Count() > 500)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Avatar), AppUserMessage.Error.AvatarOverLength, AppUserMessage);
            }
            return AppUser.IsValidated;
        }
        private async Task<bool> ValidateCoverImage(AppUser AppUser)
        {
            if(string.IsNullOrEmpty(AppUser.CoverImage))
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.CoverImage), AppUserMessage.Error.CoverImageEmpty, AppUserMessage);
            }
            else if(AppUser.CoverImage.Count() > 500)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.CoverImage), AppUserMessage.Error.CoverImageOverLength, AppUserMessage);
            }
            return AppUser.IsValidated;
        }
        private async Task<bool> ValidateSex(AppUser AppUser)
        {       
            if(AppUser.SexId == 0)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Sex), AppUserMessage.Error.SexEmpty, AppUserMessage);
            }
            else
            {
                if(!SexEnum.SexEnumList.Any(x => AppUser.SexId == x.Id))
                {
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Sex), AppUserMessage.Error.SexNotExisted, AppUserMessage);
                }
            }
            return true;
        }

        private bool VerifyPassword(string oldPassword, string newPassword)
        {
            byte[] hashBytes = Convert.FromBase64String(oldPassword);
            /* Get the salt */
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            /* Compute the hash on the password the user entered */
            var pbkdf2 = new Rfc2898DeriveBytes(newPassword, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            /* Compare the results */
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return false;
            return true;
        }

        public async Task<bool> Login(AppUser AppUser)
        {
            if (string.IsNullOrWhiteSpace(AppUser.Username))
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Username), AppUserMessage.Error.UsernameNotExisted, AppUserMessage);
                return false;
            }
            List<AppUser> AppUsers = await UOW.AppUserRepository.List(new AppUserFilter
            {
                Skip = 0,
                Take = 1,
                Username = new StringFilter { Equal = AppUser.Username },
                Selects = AppUserSelect.ALL,
                
            });
            if (AppUsers.Count == 0)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Username), AppUserMessage.Error.UsernameNotExisted, AppUserMessage);
                return false;
            } else
            {
                AppUser appUser = AppUsers.FirstOrDefault();
                bool verify = VerifyPassword(appUser.Password, AppUser.Password);
                if (verify == false)
                {
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Password), AppUserMessage.Error.PasswordNotMatch, AppUserMessage);
                    return false;
                }
                AppUser.Id = appUser.Id;
            }
            return AppUser.IsValidated;
        }

        public async Task Get(AppUser AppUser)
        {
        }

        public async Task<bool> Create(AppUser AppUser)
        {
            await ValidateUsername(AppUser);
            await ValidateEmail(AppUser);
            await ValidatePhone(AppUser);
            await ValidatePassword(AppUser);
            await ValidateDisplayName(AppUser);
            await ValidateBirthday(AppUser);
            await ValidateAvatar(AppUser);
            await ValidateCoverImage(AppUser);
            await ValidateSex(AppUser);
            return AppUser.IsValidated;
        }

        public async Task<bool> Update(AppUser AppUser)
        {
            if (await ValidateId(AppUser))
            {
                await ValidateUsername(AppUser);
                await ValidateEmail(AppUser);
                await ValidatePhone(AppUser);
                await ValidatePassword(AppUser);
                await ValidateDisplayName(AppUser);
                await ValidateBirthday(AppUser);
                await ValidateAvatar(AppUser);
                await ValidateCoverImage(AppUser);
                await ValidateSex(AppUser);
            }
            return AppUser.IsValidated;
        }

        public async Task<bool> Delete(AppUser AppUser)
        {
            if (await ValidateId(AppUser))
            {
            }
            return AppUser.IsValidated;
        }

        public async Task<bool> BulkDelete(List<AppUser> AppUsers)
        {
            foreach (AppUser AppUser in AppUsers)
            {
                await Delete(AppUser);
            }
            return AppUsers.All(x => x.IsValidated);
        }

        public async Task<bool> Import(List<AppUser> AppUsers)
        {
            return true;
        }

        public async Task<bool> Register(AppUser AppUser)
        {
            await ValidateUsername(AppUser);
            await ValidateEmail(AppUser);
            await ValidatePhone(AppUser);
            await ValidatePassword(AppUser);
            await ValidateDisplayName(AppUser);
            await ValidateBirthday(AppUser);
            await ValidateAvatar(AppUser);
            await ValidateCoverImage(AppUser);
            await ValidateSex(AppUser);
            // kiểm tra xem user này đã có chưa
            // - kiểm tra username
            int CountAppUser = await UOW.AppUserRepository.Count(new AppUserFilter
            {
                Skip = 0,
                Take = 1,
                Username = new StringFilter { Equal = AppUser.Username },
                Selects = AppUserSelect.ALL,

            });
            // user đã tồn tại 
            if (CountAppUser > 0)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Username), AppUserMessage.Error.UserExisted, AppUserMessage);
                return false;

            }
            // kiểm tra xem password và password confirm có trùng nhau chưa
            if (AppUser.PasswordConfirmation != AppUser.Password)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Username), AppUserMessage.Error.PasswordConfirmationNotMatch, AppUserMessage);
                return false;
            }
            return AppUser.IsValidated;
        }
    }
}
