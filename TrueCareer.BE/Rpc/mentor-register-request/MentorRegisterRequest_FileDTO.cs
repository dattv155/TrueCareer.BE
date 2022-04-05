using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Entities;
using TrueSight.Common;

namespace TrueCareer.Rpc.mentor_register_request
{
    public class MentorRegisterRequest_FileDTO : DataDTO
    {

        public long Id { get; set; }
        public string Key { get; set; }

        public string OriginalName { get; set; }
        public string Name { get; set; }

        public string MimeType { get; set; }

        public bool IsFile { get; set; }

        public string Path { get; set; }

        public long Level { get; set; }

        public DateTime CreatedAt { get; set; }

        public long? Size { get; set; }


        public MentorRegisterRequest_FileDTO() { }
        public MentorRegisterRequest_FileDTO(File File)
        {

            this.Id = File.Id;

            this.Key = File.Key;

            this.OriginalName = File.OriginalName;
            this.Name = File.Name;

            this.MimeType = File.MimeType;

            this.IsFile = File.IsFile;

            this.Path = File.Path;

            this.Level = File.Level;

            this.CreatedAt = File.CreatedAt;

            this.Size = File.Size;

            this.Errors = File.Errors;
        }
    }

    public class Information_FileFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public IdFilter GridId { get; set; }

        public StringFilter Key { get; set; }

        public StringFilter Name { get; set; }

        public StringFilter MimeType { get; set; }

        public StringFilter Path { get; set; }

        public LongFilter Level { get; set; }

        public DateFilter CreatedAt { get; set; }

        public LongFilter Size { get; set; }

        public FileOrder OrderBy { get; set; }
    }
}
