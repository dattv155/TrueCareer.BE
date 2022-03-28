using TrueSight.Common;
using TrueCareer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Rpc.conversation_message
{
    public class ConversationMessage_FileDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsFile { get; set; }
        public string Path { get; set; }
        public long Level { get; set; }
        public Guid RowId { get; set; }
        public ConversationMessage_FileDTO() { }
        public ConversationMessage_FileDTO(File File)
        {
            this.Id = File.Id;
            this.Name = File.Name;
            this.IsFile = File.IsFile;
            this.Path = File.Path;
            this.Level = File.Level;
            this.RowId = File.RowId;
        }
    }
}
