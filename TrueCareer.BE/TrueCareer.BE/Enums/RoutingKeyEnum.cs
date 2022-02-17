using TrueSight.Common;
using System.Collections.Generic;

namespace TrueCareer.Enums
{
    public class RoutingKeyEnum
    {
        public static GenericEnum AuditLogSend = new GenericEnum { Id = 5, Code = "AuditLog.Send", Name = "Audit Log" };
        public static GenericEnum SystemLogSend = new GenericEnum { Id = 6, Code = "SystemLog.Send", Name = "System Log" };
        public static List<GenericEnum> RoutingKeyEnumList = new List<GenericEnum>()
        {
            AuditLogSend, SystemLogSend
        };

        public static GenericEnum ActiveTimeSync = new GenericEnum { Id = 1000, Code = "ActiveTime.Sync", Name = "ActiveTime" };
        public static GenericEnum AppUserSync = new GenericEnum { Id = 1001, Code = "AppUser.Sync", Name = "AppUser" };
        public static GenericEnum ChoiceSync = new GenericEnum { Id = 1003, Code = "Choice.Sync", Name = "Choice" };
        public static GenericEnum CommentSync = new GenericEnum { Id = 1004, Code = "Comment.Sync", Name = "Comment" };
        public static GenericEnum ConnectionStatusSync = new GenericEnum { Id = 1005, Code = "ConnectionStatus.Sync", Name = "ConnectionStatus" };
        public static GenericEnum ConnectionTypeSync = new GenericEnum { Id = 1006, Code = "ConnectionType.Sync", Name = "ConnectionType" };
        public static GenericEnum ConversationSync = new GenericEnum { Id = 1007, Code = "Conversation.Sync", Name = "Conversation" };
        public static GenericEnum ConversationParticipantSync = new GenericEnum { Id = 1008, Code = "ConversationParticipant.Sync", Name = "ConversationParticipant" };
        public static GenericEnum FavouriteMentorSync = new GenericEnum { Id = 1009, Code = "FavouriteMentor.Sync", Name = "FavouriteMentor" };
        public static GenericEnum FavouriteNewsSync = new GenericEnum { Id = 1010, Code = "FavouriteNews.Sync", Name = "FavouriteNews" };
        public static GenericEnum ImageSync = new GenericEnum { Id = 1011, Code = "Image.Sync", Name = "Image" };
        public static GenericEnum InformationSync = new GenericEnum { Id = 1012, Code = "Information.Sync", Name = "Information" };
        public static GenericEnum InformationTypeSync = new GenericEnum { Id = 1013, Code = "InformationType.Sync", Name = "InformationType" };
        public static GenericEnum MajorSync = new GenericEnum { Id = 1014, Code = "Major.Sync", Name = "Major" };
        public static GenericEnum MbtiPersonalTypeSync = new GenericEnum { Id = 1015, Code = "MbtiPersonalType.Sync", Name = "MbtiPersonalType" };
        public static GenericEnum MbtiResultSync = new GenericEnum { Id = 1017, Code = "MbtiResult.Sync", Name = "MbtiResult" };
        public static GenericEnum MbtiSingleTypeSync = new GenericEnum { Id = 1018, Code = "MbtiSingleType.Sync", Name = "MbtiSingleType" };
        public static GenericEnum MentorConnectionSync = new GenericEnum { Id = 1019, Code = "MentorConnection.Sync", Name = "MentorConnection" };
        public static GenericEnum MentorMenteeConnectionSync = new GenericEnum { Id = 1020, Code = "MentorMenteeConnection.Sync", Name = "MentorMenteeConnection" };
        public static GenericEnum MentorReviewSync = new GenericEnum { Id = 1021, Code = "MentorReview.Sync", Name = "MentorReview" };
        public static GenericEnum MessageSync = new GenericEnum { Id = 1022, Code = "Message.Sync", Name = "Message" };
        public static GenericEnum NewsSync = new GenericEnum { Id = 1023, Code = "News.Sync", Name = "News" };
        public static GenericEnum NewsStatusSync = new GenericEnum { Id = 1024, Code = "NewsStatus.Sync", Name = "NewsStatus" };
        public static GenericEnum NotificationSync = new GenericEnum { Id = 1025, Code = "Notification.Sync", Name = "Notification" };
        public static GenericEnum QuestionSync = new GenericEnum { Id = 1026, Code = "Question.Sync", Name = "Question" };
        public static GenericEnum RoleSync = new GenericEnum { Id = 1027, Code = "Role.Sync", Name = "Role" };
        public static GenericEnum SchoolSync = new GenericEnum { Id = 1028, Code = "School.Sync", Name = "School" };
        public static GenericEnum SexSync = new GenericEnum { Id = 1030, Code = "Sex.Sync", Name = "Sex" };
        public static GenericEnum TopicSync = new GenericEnum { Id = 1031, Code = "Topic.Sync", Name = "Topic" };

        public static GenericEnum ActiveTimeUsed = new GenericEnum { Id = 2000, Code = "ActiveTime.Used", Name = "ActiveTime Used" };
        public static GenericEnum AppUserUsed = new GenericEnum { Id = 2001, Code = "AppUser.Used", Name = "AppUser Used" };
        public static GenericEnum ChoiceUsed = new GenericEnum { Id = 2003, Code = "Choice.Used", Name = "Choice Used" };
        public static GenericEnum CommentUsed = new GenericEnum { Id = 2004, Code = "Comment.Used", Name = "Comment Used" };
        public static GenericEnum ConnectionStatusUsed = new GenericEnum { Id = 2005, Code = "ConnectionStatus.Used", Name = "ConnectionStatus Used" };
        public static GenericEnum ConnectionTypeUsed = new GenericEnum { Id = 2006, Code = "ConnectionType.Used", Name = "ConnectionType Used" };
        public static GenericEnum ConversationUsed = new GenericEnum { Id = 2007, Code = "Conversation.Used", Name = "Conversation Used" };
        public static GenericEnum ConversationParticipantUsed = new GenericEnum { Id = 2008, Code = "ConversationParticipant.Used", Name = "ConversationParticipant Used" };
        public static GenericEnum FavouriteMentorUsed = new GenericEnum { Id = 2009, Code = "FavouriteMentor.Used", Name = "FavouriteMentor Used" };
        public static GenericEnum FavouriteNewsUsed = new GenericEnum { Id = 2010, Code = "FavouriteNews.Used", Name = "FavouriteNews Used" };
        public static GenericEnum ImageUsed = new GenericEnum { Id = 2011, Code = "Image.Used", Name = "Image Used" };
        public static GenericEnum InformationUsed = new GenericEnum { Id = 2012, Code = "Information.Used", Name = "Information Used" };
        public static GenericEnum InformationTypeUsed = new GenericEnum { Id = 2013, Code = "InformationType.Used", Name = "InformationType Used" };
        public static GenericEnum MajorUsed = new GenericEnum { Id = 2014, Code = "Major.Used", Name = "Major Used" };
        public static GenericEnum MbtiPersonalTypeUsed = new GenericEnum { Id = 2015, Code = "MbtiPersonalType.Used", Name = "MbtiPersonalType Used" };
        public static GenericEnum MbtiResultUsed = new GenericEnum { Id = 2017, Code = "MbtiResult.Used", Name = "MbtiResult Used" };
        public static GenericEnum MbtiSingleTypeUsed = new GenericEnum { Id = 2018, Code = "MbtiSingleType.Used", Name = "MbtiSingleType Used" };
        public static GenericEnum MentorConnectionUsed = new GenericEnum { Id = 2019, Code = "MentorConnection.Used", Name = "MentorConnection Used" };
        public static GenericEnum MentorMenteeConnectionUsed = new GenericEnum { Id = 2020, Code = "MentorMenteeConnection.Used", Name = "MentorMenteeConnection Used" };
        public static GenericEnum MentorReviewUsed = new GenericEnum { Id = 2021, Code = "MentorReview.Used", Name = "MentorReview Used" };
        public static GenericEnum MessageUsed = new GenericEnum { Id = 2022, Code = "Message.Used", Name = "Message Used" };
        public static GenericEnum NewsUsed = new GenericEnum { Id = 2023, Code = "News.Used", Name = "News Used" };
        public static GenericEnum NewsStatusUsed = new GenericEnum { Id = 2024, Code = "NewsStatus.Used", Name = "NewsStatus Used" };
        public static GenericEnum NotificationUsed = new GenericEnum { Id = 2025, Code = "Notification.Used", Name = "Notification Used" };
        public static GenericEnum QuestionUsed = new GenericEnum { Id = 2026, Code = "Question.Used", Name = "Question Used" };
        public static GenericEnum RoleUsed = new GenericEnum { Id = 2027, Code = "Role.Used", Name = "Role Used" };
        public static GenericEnum SchoolUsed = new GenericEnum { Id = 2028, Code = "School.Used", Name = "School Used" };
        public static GenericEnum SexUsed = new GenericEnum { Id = 2030, Code = "Sex.Used", Name = "Sex Used" };
        public static GenericEnum TopicUsed = new GenericEnum { Id = 2031, Code = "Topic.Used", Name = "Topic Used" };
    }
}
