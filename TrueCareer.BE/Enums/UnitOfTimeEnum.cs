using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueSight.Common;

namespace TrueCareer.Enums
{
    public class UnitOfTimeEnum
    {
        public static GenericEnum SIX_TO_SEVEN = new GenericEnum { Id = 1, Code = "SIX_TO_SEVEN", Name = "06:00 - 07:00" };
        public static GenericEnum SEVEN_TO_EIGHT = new GenericEnum { Id = 2, Code = "SEVEN_TO_EIGHT", Name = "07:00 - 08:00" };
        public static GenericEnum EIGHT_TO_NINE = new GenericEnum { Id = 3, Code = "EIGHT_TO_NINE", Name = "08:00 - 9:00" };
        public static GenericEnum NINE_TO_TEN = new GenericEnum { Id = 4, Code = "NINE_TO_TEN", Name = "09:00 - 10:00" };
        public static GenericEnum TEN_TO_ELEVEN = new GenericEnum { Id = 5, Code = "TEN_TO_ELEVEN", Name = "10:00 - 11:00" };
        public static GenericEnum ELEVEN_TO_TWELVE = new GenericEnum { Id = 6, Code = "ELEVEN_TO_TWELVE", Name = "11:00 - 12:00" };
        public static GenericEnum TWELVE_TO_THIRTEEN = new GenericEnum { Id = 7, Code = "TWELVE_TO_THIRTEEN", Name = "12:00 - 13:00" };
        public static GenericEnum THIRTEEN_TO_FOURTEEN = new GenericEnum { Id = 8, Code = "THIRTEEN_TO_FOURTEEN", Name = "13:00 - 14:00" };
        public static GenericEnum FOURTEEN_TO_FIFTEEN = new GenericEnum { Id = 9, Code = "FOURTEEN_TO_FIFTEEN", Name = "14:00 - 15:00" };
        public static GenericEnum FIFTEEN_TO_SIXTEEN = new GenericEnum { Id = 10, Code = "FIFTEEN_TO_SIXTEEN", Name = "15:00 - 16:00" };
        public static GenericEnum SIXTEEN_TO_SEVENTEEN = new GenericEnum { Id = 11, Code = "SIXTEEN_TO_SEVENTEEN", Name = "16:00 - 17:00" };
        public static GenericEnum SEVENTEEN_TO_EIGHTEEN = new GenericEnum { Id = 12, Code = "SEVENTEEN_TO_EIGHTEEN", Name = "17:00 - 18:00" };
        public static GenericEnum EIGHTEEN_TO_NINETEEN = new GenericEnum { Id = 13, Code = "EIGHTEEN_TO_NINETEEN", Name = "18:00 - 19:00" };
        public static GenericEnum NINETEEN_TO_TWENTY = new GenericEnum { Id = 14, Code = "NINETEEN_TO_TWENTY", Name = "19:00 - 20:00" };
        public static GenericEnum TWENTY_TO_TWENTY_ONE = new GenericEnum { Id = 15, Code = "TWENTY_TO_TWENTY_ONE", Name = "20:00 - 21:00" };
        public static GenericEnum TWENTY_ONE_TO_TWENTY_TWO = new GenericEnum { Id = 16, Code = "TWENTY_ONE_TO_TWENTY_TWO", Name = "21:00 - 22:00" };
        public static GenericEnum TWENTY_TWO_TO_TWENTY_THREE = new GenericEnum { Id = 17, Code = "TWENTY_TWO_TO_TWENTY_THREE", Name = "22:00 - 23:00" };
        public static GenericEnum TWENTY_THREE_TO_ZERO = new GenericEnum { Id = 18, Code = "TWENTY_THREE_TO_ZERO", Name = "23:00 - 00:00" };
        public static List<GenericEnum> UnitOfTimeEnumList = new List<GenericEnum>
        {
            SIX_TO_SEVEN, SEVEN_TO_EIGHT, EIGHT_TO_NINE, NINE_TO_TEN, TEN_TO_ELEVEN, ELEVEN_TO_TWELVE, TWELVE_TO_THIRTEEN, THIRTEEN_TO_FOURTEEN, FOURTEEN_TO_FIFTEEN, FIFTEEN_TO_SIXTEEN, SIXTEEN_TO_SEVENTEEN, SEVENTEEN_TO_EIGHTEEN, EIGHTEEN_TO_NINETEEN, NINETEEN_TO_TWENTY, TWENTY_TO_TWENTY_ONE, TWENTY_ONE_TO_TWENTY_TWO, TWENTY_TWO_TO_TWENTY_THREE, TWENTY_THREE_TO_ZERO
        };
    }
}
