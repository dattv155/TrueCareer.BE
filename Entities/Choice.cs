using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class Choice : DataEntity,  IEquatable<Choice>
    {
        public long Id { get; set; }
        public string ChoiceContent { get; set; }
        public string Description { get; set; }
        public long QuestionId { get; set; }
        public long MbtiSingleTypeId { get; set; }
        public MbtiSingleType MbtiSingleType { get; set; }
        public Question Question { get; set; }
        
        public bool Equals(Choice other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.ChoiceContent != other.ChoiceContent) return false;
            if (this.Description != other.Description) return false;
            if (this.QuestionId != other.QuestionId) return false;
            if (this.MbtiSingleTypeId != other.MbtiSingleTypeId) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ChoiceFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter ChoiceContent { get; set; }
        public StringFilter Description { get; set; }
        public IdFilter QuestionId { get; set; }
        public IdFilter MbtiSingleTypeId { get; set; }
        public List<ChoiceFilter> OrFilter { get; set; }
        public ChoiceOrder OrderBy {get; set;}
        public ChoiceSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ChoiceOrder
    {
        Id = 0,
        ChoiceContent = 1,
        Description = 2,
        Question = 3,
        MbtiSingleType = 4,
    }

    [Flags]
    public enum ChoiceSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        ChoiceContent = E._1,
        Description = E._2,
        Question = E._3,
        MbtiSingleType = E._4,
    }
}
