using TrueSight.Common;
using TrueCareer.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using TrueCareer.Entities;

namespace TrueCareer.Rpc.conversation
{
    public class Conversation_ConversationDTO : DataDTO
    {
        public long Id { get; set; }
        public string LatestContent { get; set; }
        public long? LatestUserId { get; set; }
        public string Hash { get; set; }
        public List<Conversation_MessageDTO> Messages { get; set; }
        public Guid RowId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Conversation_ConversationDTO() {}
        public Conversation_ConversationDTO(Conversation Conversation)
        {
            this.Id = Conversation.Id;
            this.LatestContent = Conversation.LatestContent;
            this.LatestUserId = Conversation.LatestUserId;
            this.Hash = Conversation.Hash;
            this.Messages = Conversation.Messages?.Select(x => new Conversation_MessageDTO(x)).ToList();
            this.RowId = Conversation.RowId;
            this.CreatedAt = Conversation.CreatedAt;
            this.UpdatedAt = Conversation.UpdatedAt;
            this.Informations = Conversation.Informations;
            this.Warnings = Conversation.Warnings;
            this.Errors = Conversation.Errors;
        }
    }

    public class Conversation_ConversationFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter LatestContent { get; set; }
        public IdFilter LatestUserId { get; set; }
        public StringFilter Hash { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public ConversationOrder OrderBy { get; set; }
    }
}
