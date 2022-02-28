using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Services.MAppUser
{
    public class AppUserMessage
    {
        public enum Information
        {

        }

        public enum Warning
        {

        }

        public enum Error
        {
            IdNotExisted,
            UsernameEmpty,
            UsernameOverLength,
            EmailEmpty,
            EmailOverLength,
            PhoneEmpty,
            PhoneOverLength,
            PasswordEmpty,
            PasswordOverLength,
            DisplayNameEmpty,
            DisplayNameOverLength,
            BirthdayEmpty,
            AvatarEmpty,
            AvatarOverLength,
            CoverImageEmpty,
            CoverImageOverLength,
            SexEmpty,
            SexNotExisted,
            UsernameNotExisted,
            PasswordNotMatch,
            UserExisted,
            PasswordConfirmationNotMatch
        }
    }
}
