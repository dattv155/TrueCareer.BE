using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class ConversationAttachmentTypeDAO
    {
        public ConversationAttachmentTypeDAO()
        {
            ConversationAttachments = new HashSet<ConversationAttachmentDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ConversationAttachmentDAO> ConversationAttachments { get; set; }
    }
}
