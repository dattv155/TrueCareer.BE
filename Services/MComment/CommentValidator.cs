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

namespace TrueCareer.Services.MComment
{
    public interface ICommentValidator : IServiceScoped
    {
        Task Get(Comment Comment);
        Task<bool> Create(Comment Comment);
        Task<bool> Update(Comment Comment);
        Task<bool> Delete(Comment Comment);
        Task<bool> BulkDelete(List<Comment> Comments);
        Task<bool> Import(List<Comment> Comments);
    }

    public class CommentValidator : ICommentValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private CommentMessage CommentMessage;

        public CommentValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.CommentMessage = new CommentMessage();
        }

        public async Task Get(Comment Comment)
        {
        }

        public async Task<bool> Create(Comment Comment)
        {
            await ValidateContent(Comment);
            await ValidateCreator(Comment);
            return Comment.IsValidated;
        }

        public async Task<bool> Update(Comment Comment)
        {
            if (await ValidateId(Comment))
            {
                await ValidateContent(Comment);
                await ValidateCreator(Comment);
            }
            return Comment.IsValidated;
        }

        public async Task<bool> Delete(Comment Comment)
        {
            if (await ValidateId(Comment))
            {
            }
            return Comment.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Comment> Comments)
        {
            foreach (Comment Comment in Comments)
            {
                await Delete(Comment);
            }
            return Comments.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<Comment> Comments)
        {
            return true;
        }
        
        private async Task<bool> ValidateId(Comment Comment)
        {
            CommentFilter CommentFilter = new CommentFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Comment.Id },
                Selects = CommentSelect.Id
            };

            int count = await UOW.CommentRepository.CountAll(CommentFilter);
            if (count == 0)
                Comment.AddError(nameof(CommentValidator), nameof(Comment.Id), CommentMessage.Error.IdNotExisted, CommentMessage);
            return Comment.IsValidated;
        }

        private async Task<bool> ValidateContent(Comment Comment)
        {
            if(string.IsNullOrEmpty(Comment.Content))
            {
                Comment.AddError(nameof(CommentValidator), nameof(Comment.Content), CommentMessage.Error.ContentEmpty, CommentMessage);
            }
            else if(Comment.Content.Count() > 500)
            {
                Comment.AddError(nameof(CommentValidator), nameof(Comment.Content), CommentMessage.Error.ContentOverLength, CommentMessage);
            }
            return Comment.IsValidated;
        }
        private async Task<bool> ValidateCreator(Comment Comment)
        {       
            if(Comment.CreatorId == 0)
            {
                Comment.AddError(nameof(CommentValidator), nameof(Comment.Creator), CommentMessage.Error.CreatorEmpty, CommentMessage);
            }
            else
            {
                int count = await UOW.AppUserRepository.CountAll(new AppUserFilter
                {
                    Id = new IdFilter{ Equal =  Comment.CreatorId },
                });
                if(count == 0)
                {
                    Comment.AddError(nameof(CommentValidator), nameof(Comment.Creator), CommentMessage.Error.CreatorNotExisted, CommentMessage);
                }
            }
            return true;
        }
    }
}
