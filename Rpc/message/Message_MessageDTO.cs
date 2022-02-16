using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.message
{
    public class Message_MessageDTO : DataDTO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Content { get; set; }
        public long ConversationId { get; set; }
        public Message_ConversationDTO Conversation { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Message_MessageDTO() {}
        public Message_MessageDTO(Message Message)
        {
            this.Id = Message.Id;
            this.UserId = Message.UserId;
            this.Content = Message.Content;
            this.ConversationId = Message.ConversationId;
            this.Conversation = Message.Conversation == null ? null : new Message_ConversationDTO(Message.Conversation);
            this.CreatedAt = Message.CreatedAt;
            this.UpdatedAt = Message.UpdatedAt;
            this.Informations = Message.Informations;
            this.Warnings = Message.Warnings;
            this.Errors = Message.Errors;
        }
    }

    public class Message_MessageFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter UserId { get; set; }
        public StringFilter Content { get; set; }
        public IdFilter ConversationId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public MessageOrder OrderBy { get; set; }
    }
}
