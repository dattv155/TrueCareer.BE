using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Enums
{
  public class StatusEnum
  {
    public static GenericEnum ACTIVE = new GenericEnum { Id = 1, Code = "ACTIVE", Name = "Hoạt động" };
    public static GenericEnum INACTIVE = new GenericEnum { Id = 2, Code = "INACTIVE", Name = "Không hoạt động" };
    public static List<GenericEnum> StatusEnumList = new List<GenericEnum>
    {
      ACTIVE, INACTIVE
    };
  }
}