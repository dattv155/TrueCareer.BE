using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class ConversationTypeDAO
    {
        public ConversationTypeDAO()
        {
            ConversationConfigurations = new HashSet<ConversationConfigurationDAO>();
            Conversations = new HashSet<ConversationDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ConversationConfigurationDAO> ConversationConfigurations { get; set; }
        public virtual ICollection<ConversationDAO> Conversations { get; set; }
    }
}
