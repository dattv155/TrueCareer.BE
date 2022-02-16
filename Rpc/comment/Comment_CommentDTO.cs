using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.comment
{
    public class Comment_CommentDTO : DataDTO
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public long CreatorId { get; set; }
        public Guid? DiscussionId { get; set; }
        public Comment_AppUserDTO Creator { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Comment_CommentDTO() {}
        public Comment_CommentDTO(Comment Comment)
        {
            this.Id = Comment.Id;
            this.Content = Comment.Content;
            this.CreatorId = Comment.CreatorId;
            this.DiscussionId = Comment.DiscussionId;
            this.Creator = Comment.Creator == null ? null : new Comment_AppUserDTO(Comment.Creator);
            this.CreatedAt = Comment.CreatedAt;
            this.UpdatedAt = Comment.UpdatedAt;
            this.Informations = Comment.Informations;
            this.Warnings = Comment.Warnings;
            this.Errors = Comment.Errors;
        }
    }

    public class Comment_CommentFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Content { get; set; }
        public IdFilter CreatorId { get; set; }
        public GuidFilter DiscussionId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public CommentOrder OrderBy { get; set; }
    }
}
