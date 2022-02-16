using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class AppUser : DataEntity,  IEquatable<AppUser>
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public long SexId { get; set; }
        public DateTime? Birthday { get; set; }
        public string Avatar { get; set; }
        public string CoverImage { get; set; }
        public Sex Sex { get; set; }
        public List<FavouriteMentor> FavouriteMentorMentors { get; set; }
        public List<FavouriteMentor> FavouriteMentorUsers { get; set; }
        public List<FavouriteNews> FavouriteNews { get; set; }
        public List<Information> Information { get; set; }
        public Guid RowId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(AppUser other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Username != other.Username) return false;
            if (this.Email != other.Email) return false;
            if (this.Phone != other.Phone) return false;
            if (this.Password != other.Password) return false;
            if (this.DisplayName != other.DisplayName) return false;
            if (this.SexId != other.SexId) return false;
            if (this.Birthday != other.Birthday) return false;
            if (this.Avatar != other.Avatar) return false;
            if (this.CoverImage != other.CoverImage) return false;
            if (this.FavouriteMentorMentors?.Count != other.FavouriteMentorMentors?.Count) return false;
            else if (this.FavouriteMentorMentors != null && other.FavouriteMentorMentors != null)
            {
                for (int i = 0; i < FavouriteMentorMentors.Count; i++)
                {
                    FavouriteMentor FavouriteMentor = FavouriteMentorMentors[i];
                    FavouriteMentor otherFavouriteMentor = other.FavouriteMentorMentors[i];
                    if (FavouriteMentor == null && otherFavouriteMentor != null)
                        return false;
                    if (FavouriteMentor != null && otherFavouriteMentor == null)
                        return false;
                    if (FavouriteMentor.Equals(otherFavouriteMentor) == false)
                        return false;
                }
            }
            if (this.FavouriteMentorUsers?.Count != other.FavouriteMentorUsers?.Count) return false;
            else if (this.FavouriteMentorUsers != null && other.FavouriteMentorUsers != null)
            {
                for (int i = 0; i < FavouriteMentorUsers.Count; i++)
                {
                    FavouriteMentor FavouriteMentor = FavouriteMentorUsers[i];
                    FavouriteMentor otherFavouriteMentor = other.FavouriteMentorUsers[i];
                    if (FavouriteMentor == null && otherFavouriteMentor != null)
                        return false;
                    if (FavouriteMentor != null && otherFavouriteMentor == null)
                        return false;
                    if (FavouriteMentor.Equals(otherFavouriteMentor) == false)
                        return false;
                }
            }
            if (this.FavouriteNews?.Count != other.FavouriteNews?.Count) return false;
            else if (this.FavouriteNews != null && other.FavouriteNews != null)
            {
                for (int i = 0; i < FavouriteNews.Count; i++)
                {
                    FavouriteNews FavouriteNews = FavouriteNews[i];
                    FavouriteNews otherFavouriteNews = other.FavouriteNews[i];
                    if (FavouriteNews == null && otherFavouriteNews != null)
                        return false;
                    if (FavouriteNews != null && otherFavouriteNews == null)
                        return false;
                    if (FavouriteNews.Equals(otherFavouriteNews) == false)
                        return false;
                }
            }
            if (this.Information?.Count != other.Information?.Count) return false;
            else if (this.Information != null && other.Information != null)
            {
                for (int i = 0; i < Information.Count; i++)
                {
                    Information Information = Information[i];
                    Information otherInformation = other.Information[i];
                    if (Information == null && otherInformation != null)
                        return false;
                    if (Information != null && otherInformation == null)
                        return false;
                    if (Information.Equals(otherInformation) == false)
                        return false;
                }
            }
            return true;
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
        public StringFilter Password { get; set; }
        public StringFilter DisplayName { get; set; }
        public IdFilter SexId { get; set; }
        public DateFilter Birthday { get; set; }
        public StringFilter Avatar { get; set; }
        public StringFilter CoverImage { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<AppUserFilter> OrFilter { get; set; }
        public AppUserOrder OrderBy {get; set;}
        public AppUserSelect Selects {get; set;}
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
    public enum AppUserSelect:long
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
