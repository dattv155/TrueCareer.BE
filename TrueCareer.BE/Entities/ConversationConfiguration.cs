using TrueSight.Common;
using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class ConversationConfiguration : DataEntity,  IEquatable<ConversationConfiguration>
    {
        public long Id { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string AppName { get; set; }
        public string OaId { get; set; }
        public string OaToken { get; set; }
        public string OaSecretKey { get; set; }
        public long ConversationTypeId { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public long StatusId { get; set; }
        public ConversationType ConversationType { get; set; }
        public Status Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(ConversationConfiguration other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.AppSecret != other.AppSecret) return false;
            if (this.AppName != other.AppName) return false;
            if (this.OaToken != other.OaToken) return false;
            if (this.OaSecretKey != other.OaSecretKey) return false;
            if (this.ConversationTypeId != other.ConversationTypeId) return false;
            if (this.ExpiredAt != other.ExpiredAt) return false;
            if (this.StatusId != other.StatusId) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ConversationConfigurationFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter AppId { get; set; }
        public StringFilter AppSecret { get; set; }
        public StringFilter AppName { get; set; }
        public StringFilter OaId { get; set; }
        public StringFilter OaToken { get; set; }
        public StringFilter OaSecretKey { get; set; }
        public IdFilter ConversationTypeId { get; set; }
        public DateFilter ExpiredAt { get; set; }
        public IdFilter StatusId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<ConversationConfigurationFilter> OrFilter { get; set; }
        public ConversationConfigurationOrder OrderBy {get; set;}
        public ConversationConfigurationSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ConversationConfigurationOrder
    {
        Id = 0,
        AppSecret = 2,
        AppName = 3,
        OaToken = 5,
        OaSecretKey = 6,
        ConversationType = 7,
        ExpiredAt = 8,
        Status = 9,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum ConversationConfigurationSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        AppSecret = E._2,
        AppName = E._3,
        OaToken = E._5,
        OaSecretKey = E._6,
        ConversationType = E._7,
        ExpiredAt = E._8,
        Status = E._9,
    }
}
