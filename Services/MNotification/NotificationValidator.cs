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

namespace TrueCareer.Services.MNotification
{
    public interface INotificationValidator : IServiceScoped
    {
        Task Get(Notification Notification);
        Task<bool> Create(Notification Notification);
        Task<bool> Update(Notification Notification);
        Task<bool> Delete(Notification Notification);
        Task<bool> BulkDelete(List<Notification> Notifications);
        Task<bool> Import(List<Notification> Notifications);
    }

    public class NotificationValidator : INotificationValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private NotificationMessage NotificationMessage;

        public NotificationValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.NotificationMessage = new NotificationMessage();
        }

        public async Task Get(Notification Notification)
        {
        }

        public async Task<bool> Create(Notification Notification)
        {
            await ValidateTitleWeb(Notification);
            await ValidateContentWeb(Notification);
            await ValidateUnread(Notification);
            await ValidateTime(Notification);
            await ValidateLinkWebsite(Notification);
            await ValidateRecipient(Notification);
            await ValidateSender(Notification);
            return Notification.IsValidated;
        }

        public async Task<bool> Update(Notification Notification)
        {
            if (await ValidateId(Notification))
            {
                await ValidateTitleWeb(Notification);
                await ValidateContentWeb(Notification);
                await ValidateUnread(Notification);
                await ValidateTime(Notification);
                await ValidateLinkWebsite(Notification);
                await ValidateRecipient(Notification);
                await ValidateSender(Notification);
            }
            return Notification.IsValidated;
        }

        public async Task<bool> Delete(Notification Notification)
        {
            if (await ValidateId(Notification))
            {
            }
            return Notification.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Notification> Notifications)
        {
            foreach (Notification Notification in Notifications)
            {
                await Delete(Notification);
            }
            return Notifications.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<Notification> Notifications)
        {
            return true;
        }
        
        private async Task<bool> ValidateId(Notification Notification)
        {
            NotificationFilter NotificationFilter = new NotificationFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Notification.Id },
                Selects = NotificationSelect.Id
            };

            int count = await UOW.NotificationRepository.CountAll(NotificationFilter);
            if (count == 0)
                Notification.AddError(nameof(NotificationValidator), nameof(Notification.Id), NotificationMessage.Error.IdNotExisted, NotificationMessage);
            return Notification.IsValidated;
        }

        private async Task<bool> ValidateTitleWeb(Notification Notification)
        {
            if(string.IsNullOrEmpty(Notification.TitleWeb))
            {
                Notification.AddError(nameof(NotificationValidator), nameof(Notification.TitleWeb), NotificationMessage.Error.TitleWebEmpty, NotificationMessage);
            }
            else if(Notification.TitleWeb.Count() > 500)
            {
                Notification.AddError(nameof(NotificationValidator), nameof(Notification.TitleWeb), NotificationMessage.Error.TitleWebOverLength, NotificationMessage);
            }
            return Notification.IsValidated;
        }
        private async Task<bool> ValidateContentWeb(Notification Notification)
        {
            if(string.IsNullOrEmpty(Notification.ContentWeb))
            {
                Notification.AddError(nameof(NotificationValidator), nameof(Notification.ContentWeb), NotificationMessage.Error.ContentWebEmpty, NotificationMessage);
            }
            else if(Notification.ContentWeb.Count() > 500)
            {
                Notification.AddError(nameof(NotificationValidator), nameof(Notification.ContentWeb), NotificationMessage.Error.ContentWebOverLength, NotificationMessage);
            }
            return Notification.IsValidated;
        }
        private async Task<bool> ValidateUnread(Notification Notification)
        {   
            return true;
        }
        private async Task<bool> ValidateTime(Notification Notification)
        {       
            if(Notification.Time <= new DateTime(2000, 1, 1))
            {
                Notification.AddError(nameof(NotificationValidator), nameof(Notification.Time), NotificationMessage.Error.TimeEmpty, NotificationMessage);
            }
            return true;
        }
        private async Task<bool> ValidateLinkWebsite(Notification Notification)
        {
            if(string.IsNullOrEmpty(Notification.LinkWebsite))
            {
                Notification.AddError(nameof(NotificationValidator), nameof(Notification.LinkWebsite), NotificationMessage.Error.LinkWebsiteEmpty, NotificationMessage);
            }
            else if(Notification.LinkWebsite.Count() > 500)
            {
                Notification.AddError(nameof(NotificationValidator), nameof(Notification.LinkWebsite), NotificationMessage.Error.LinkWebsiteOverLength, NotificationMessage);
            }
            return Notification.IsValidated;
        }
        private async Task<bool> ValidateRecipient(Notification Notification)
        {       
            if(Notification.RecipientId == 0)
            {
                Notification.AddError(nameof(NotificationValidator), nameof(Notification.Recipient), NotificationMessage.Error.RecipientEmpty, NotificationMessage);
            }
            else
            {
                int count = await UOW.AppUserRepository.CountAll(new AppUserFilter
                {
                    Id = new IdFilter{ Equal =  Notification.RecipientId },
                });
                if(count == 0)
                {
                    Notification.AddError(nameof(NotificationValidator), nameof(Notification.Recipient), NotificationMessage.Error.RecipientNotExisted, NotificationMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateSender(Notification Notification)
        {       
            if(Notification.SenderId == 0)
            {
                Notification.AddError(nameof(NotificationValidator), nameof(Notification.Sender), NotificationMessage.Error.SenderEmpty, NotificationMessage);
            }
            else
            {
                int count = await UOW.AppUserRepository.CountAll(new AppUserFilter
                {
                    Id = new IdFilter{ Equal =  Notification.SenderId },
                });
                if(count == 0)
                {
                    Notification.AddError(nameof(NotificationValidator), nameof(Notification.Sender), NotificationMessage.Error.SenderNotExisted, NotificationMessage);
                }
            }
            return true;
        }
    }
}
