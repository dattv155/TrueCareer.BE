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
            FavouriteNews_NewsEmpty,
            FavouriteNews_NewsNotExisted,
            Information_NameEmpty,
            Information_NameOverLength,
            Information_DescriptionEmpty,
            Information_DescriptionOverLength,
            Information_StartAtEmpty,
            Information_StartAtInvalid,
            Information_RoleEmpty,
            Information_RoleOverLength,
            Information_ImageEmpty,
            Information_ImageOverLength,
            Information_EndAtEmpty,
            Information_EndAtInvalid,
            Information_InformationTypeEmpty,
            Information_InformationTypeNotExisted,
            Information_TopicEmpty,
            Information_TopicNotExisted,
        }
    }
}
