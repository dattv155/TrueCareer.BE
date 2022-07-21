using TrueSight.Common;
using System;
using System.Collections.Generic;
using TrueCareer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueCareer.Entities
{
    public class Question : DataEntity,  IEquatable<Question>
    {
        public long Id { get; set; }
        public string QuestionContent { get; set; }
        public string Description { get; set; }
        public List<Choice> Choices { get; set; }
        
        public bool Equals(Question other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.QuestionContent != other.QuestionContent) return false;
            if (this.Description != other.Description) return false;
            if (this.Choices?.Count != other.Choices?.Count) return false;
            else if (this.Choices != null && other.Choices != null)
            {
                for (int i = 0; i < Choices.Count; i++)
                {
                    Choice Choice = Choices[i];
                    Choice otherChoice = other.Choices[i];
                    if (Choice == null && otherChoice != null)
                        return false;
                    if (Choice != null && otherChoice == null)
                        return false;
                    if (Choice.Equals(otherChoice) == false)
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

    public class QuestionFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter QuestionContent { get; set; }
        public StringFilter Description { get; set; }
        public List<QuestionFilter> OrFilter { get; set; }
        public QuestionOrder OrderBy {get; set;}
        public QuestionSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum QuestionOrder
    {
        Id = 0,
        QuestionContent = 1,
        Description = 2,
    }

    [Flags]
    public enum QuestionSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        QuestionContent = E._1,
        Description = E._2,
        Choice = E._3
    }
}
