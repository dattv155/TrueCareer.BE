using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class AppUser : DataEntity, IEquatable<AppUser>
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string OtpCode { get; set; }
        public DateTime? OtpExpired { get; set; }
        public string Token { get; set; }
        public string DisplayName { get; set; }
        public long SexId { get; set; }
        public DateTime? Birthday { get; set; }
        public string Avatar { get; set; }
        public string CoverImage { get; set; }
        public Sex Sex { get; set; }
        public Guid RowId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string PasswordConfirmation { get; set; }

        public string GIdToken { get; set; }

        public string FbIdToken { get; set; }

        public string AIdToken { get; set; }

        public long? RoleId { get; set; }
        public int LikeCount { get; set; }
        public int MenteeCount { get; set; }

        public string JobRole { get; set; }
        public string CompanyName { get; set; }

        public bool Equals(AppUser other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class AppUserFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Username { get; set; }
        public StringFilter Email { get; set; }
        public StringFilter Phone { get; set; }
        public StringFilter GIdToken { get; set; }
        public StringFilter FbIdToken { get; set; }
        public StringFilter AIdToken { get; set; }
        public StringFilter Password { get; set; }
        public string OtpCode { get; set; }
        public DateTime? OtpExpired { get; set; }
        public string Token { get; set; }
        public StringFilter DisplayName { get; set; }
        public IdFilter SexId { get; set; }
        public DateFilter Birthday { get; set; }
        public StringFilter Avatar { get; set; }
        public StringFilter CoverImage { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public IdFilter MajorId { get; set; }

        public StringFilter Address { get; set; }

        public StringFilter JobRole { get; set; }
        public IdFilter RoleId { get; set; }

        public StringFilter CompanyName { get; set; }
        public List<AppUserFilter> OrFilter { get; set; }
        public AppUserOrder OrderBy { get; set; }
        public AppUserSelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AppUserOrder
    {
        Id = 0,
        Username = 1,
        Email = 2,
        Phone = 3,
        Password = 4,
        DisplayName = 5,
        Sex = 6,
        Birthday = 7,
        Avatar = 8,
        CoverImage = 9,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum AppUserSelect : long
    {
        ALL = E.ALL,
        Id = E._0,
        Username = E._1,
        Email = E._2,
        Phone = E._3,
        Password = E._4,
        DisplayName = E._5,
        Sex = E._6,
        Birthday = E._7,
        Avatar = E._8,
        CoverImage = E._9,
    }
}
