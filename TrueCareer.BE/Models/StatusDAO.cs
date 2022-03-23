using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class StatusDAO
    {
        public StatusDAO()
        {
            ConversationConfigurations = new HashSet<ConversationConfigurationDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ConversationConfigurationDAO> ConversationConfigurations { get; set; }
    }
}
