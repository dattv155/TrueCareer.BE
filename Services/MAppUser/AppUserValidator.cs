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
            await ValidateFavouriteMentorMentors(AppUser);
            await ValidateFavouriteMentorUsers(AppUser);
            await ValidateFavouriteNews(AppUser);
            await ValidateInformation(AppUser);
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
                await ValidateFavouriteMentorMentors(AppUser);
                await ValidateFavouriteMentorUsers(AppUser);
                await ValidateFavouriteNews(AppUser);
                await ValidateInformation(AppUser);
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
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Birthday), AppUserMessage.Error.BirthdayInvalid, AppUserMessage);
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
        private async Task<bool> ValidateFavouriteMentorMentors(AppUser AppUser)
        {   
            if(AppUser.FavouriteMentorMentors?.Any() ?? false)
            {
                #region fetch data
                #endregion

                #region validate
                foreach(FavouriteMentor FavouriteMentor in AppUser.FavouriteMentorMentors)
                {
                }
                #endregion
            }
            else 
            {

            }
            return true;
        }
        private async Task<bool> ValidateFavouriteMentorUsers(AppUser AppUser)
        {   
            if(AppUser.FavouriteMentorUsers?.Any() ?? false)
            {
                #region fetch data
                #endregion

                #region validate
                foreach(FavouriteMentor FavouriteMentor in AppUser.FavouriteMentorUsers)
                {
                }
                #endregion
            }
            else 
            {

            }
            return true;
        }
        private async Task<bool> ValidateFavouriteNews(AppUser AppUser)
        {   
            if(AppUser.FavouriteNews?.Any() ?? false)
            {
                #region fetch data
                List<long> NewsIds = new List<long>();
                NewsIds.AddRange(AppUser.FavouriteNewss.Select(x => x.NewsId));
                List<News> News = await UOW.NewsRepository.List(new NewsFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = NewsSelect.Id,

                    Id = new IdFilter { In = NewsIds },
                });
                #endregion

                #region validate
                foreach(FavouriteNews FavouriteNews in AppUser.FavouriteNews)
                {
                    if(FavouriteNews.NewsId == 0)
                    {
                        FavouriteNews.AddError(nameof(AppUserValidator), nameof(FavouriteNews.News), AppUserMessage.Error.FavouriteNews_NewsEmpty, AppUserMessage);
                    }
                    else
                    {
                        News News = News.FirstOrDefault(x => x.Id == FavouriteNews.NewsId);
                        if(News == null)
                        {
                            FavouriteNews.AddError(nameof(AppUserValidator), nameof(FavouriteNews.News), AppUserMessage.Error.FavouriteNews_NewsNotExisted, AppUserMessage);
                        }
                    }
                    
                }
                #endregion
            }
            else 
            {

            }
            return true;
        }
        private async Task<bool> ValidateInformation(AppUser AppUser)
        {   
            if(AppUser.Information?.Any() ?? false)
            {
                #region fetch data
                List<long> InformationTypeIds = new List<long>();
                List<long> TopicIds = new List<long>();
                InformationTypeIds.AddRange(AppUser.Informations.Select(x => x.InformationTypeId));
                TopicIds.AddRange(AppUser.Informations.Select(x => x.TopicId));
                List<InformationType> InformationTypes = await UOW.InformationTypeRepository.List(new InformationTypeFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = InformationTypeSelect.Id,

                    Id = new IdFilter { In = InformationTypeIds },
                });
                List<Topic> Topics = await UOW.TopicRepository.List(new TopicFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = TopicSelect.Id,

                    Id = new IdFilter { In = TopicIds },
                });
                #endregion

                #region validate
                foreach(Information Information in AppUser.Information)
                {
                    if(string.IsNullOrEmpty(Information.Name))
                    {
                        Information.AddError(nameof(AppUserValidator), nameof(Information.Name), AppUserMessage.Error.Information_NameEmpty, AppUserMessage);
                    }
                    else if(Information.Name.Count() > 500)
                    {
                        Information.AddError(nameof(AppUserValidator), nameof(Information.Name), AppUserMessage.Error.Information_NameOverLength, AppUserMessage);
                    }

                    if(string.IsNullOrEmpty(Information.Description))
                    {
                        Information.AddError(nameof(AppUserValidator), nameof(Information.Description), AppUserMessage.Error.Information_DescriptionEmpty, AppUserMessage);
                    }
                    else if(Information.Description.Count() > 500)
                    {
                        Information.AddError(nameof(AppUserValidator), nameof(Information.Description), AppUserMessage.Error.Information_DescriptionOverLength, AppUserMessage);
                    }

                    if(Information.StartAt <= new DateTime(2000, 1, 1))
                    {
                        Information.AddError(nameof(AppUserValidator), nameof(Information.StartAt), AppUserMessage.Error.Information_StartAtEmpty, AppUserMessage);
                    }
                    
                    if(string.IsNullOrEmpty(Information.Role))
                    {
                        Information.AddError(nameof(AppUserValidator), nameof(Information.Role), AppUserMessage.Error.Information_RoleEmpty, AppUserMessage);
                    }
                    else if(Information.Role.Count() > 500)
                    {
                        Information.AddError(nameof(AppUserValidator), nameof(Information.Role), AppUserMessage.Error.Information_RoleOverLength, AppUserMessage);
                    }

                    if(string.IsNullOrEmpty(Information.Image))
                    {
                        Information.AddError(nameof(AppUserValidator), nameof(Information.Image), AppUserMessage.Error.Information_ImageEmpty, AppUserMessage);
                    }
                    else if(Information.Image.Count() > 500)
                    {
                        Information.AddError(nameof(AppUserValidator), nameof(Information.Image), AppUserMessage.Error.Information_ImageOverLength, AppUserMessage);
                    }

                    if(Information.EndAt <= new DateTime(2000, 1, 1))
                    {
                        Information.AddError(nameof(AppUserValidator), nameof(Information.EndAt), AppUserMessage.Error.Information_EndAtEmpty, AppUserMessage);
                    }
                    
                    if(Information.InformationTypeId == 0)
                    {
                        Information.AddError(nameof(AppUserValidator), nameof(Information.InformationType), AppUserMessage.Error.Information_InformationTypeEmpty, AppUserMessage);
                    }
                    else
                    {
                        InformationType InformationType = InformationTypes.FirstOrDefault(x => x.Id == Information.InformationTypeId);
                        if(InformationType == null)
                        {
                            Information.AddError(nameof(AppUserValidator), nameof(Information.InformationType), AppUserMessage.Error.Information_InformationTypeNotExisted, AppUserMessage);
                        }
                    }
                    
                    if(Information.TopicId == 0)
                    {
                        Information.AddError(nameof(AppUserValidator), nameof(Information.Topic), AppUserMessage.Error.Information_TopicEmpty, AppUserMessage);
                    }
                    else
                    {
                        Topic Topic = Topics.FirstOrDefault(x => x.Id == Information.TopicId);
                        if(Topic == null)
                        {
                            Information.AddError(nameof(AppUserValidator), nameof(Information.Topic), AppUserMessage.Error.Information_TopicNotExisted, AppUserMessage);
                        }
                    }
                    
                }
                #endregion
            }
            else 
            {

            }
            return true;
        }
    }
}
