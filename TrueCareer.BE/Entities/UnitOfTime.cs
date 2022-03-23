using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TrueSight.Common;

namespace TrueCareer.Entities
{
    public class UnitOfTime : DataEntity, IEquatable<UnitOfTime>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public long? StartAt { get; set; }
        public long? EndAt { get; set; }

        public bool Equals(UnitOfTime other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class UnitOfTimeFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public List<UnitOfTimeFilter> OrFilter { get; set; }
        public UnitOfTimeOrder OrderBy { get; set; }
        public UnitOfTimeSelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum UnitOfTimeOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
    }

    [Flags]
    public enum UnitOfTimeSelect : long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
    }
}
