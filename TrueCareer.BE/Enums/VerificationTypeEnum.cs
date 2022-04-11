using System.Collections.Generic;
using TrueSight.Common;

namespace TrueCareer.Enums
{
    public class VerificationTypeEnum
    {
        public static GenericEnum PHONE = new GenericEnum { Id = 1, Code = "PHONE", Name = "Số điện thoại" };
        public static GenericEnum EMAIL = new GenericEnum { Id = 2, Code = "EMAIL", Name = "Email" };
        public static GenericEnum GOOGLE = new GenericEnum { Id = 3, Code = "GG", Name = "Google" };
        public static GenericEnum FACEBOOK = new GenericEnum { Id = 4, Code = "FB", Name = "Facebook" };
        public static GenericEnum APPLE = new GenericEnum { Id = 5, Code = "AP", Name = "Apple" };
        public static List<GenericEnum> VerificationTypeEnumList = new List<GenericEnum>()
        {
            PHONE, EMAIL,GOOGLE, FACEBOOK, APPLE
        };
    }
}