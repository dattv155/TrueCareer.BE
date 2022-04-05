using TrueSight.Common;
using TrueCareer.Common;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using TrueCareer.BE.Models;
using TrueCareer.Repositories;
using System;
using MongoDB.Driver;

namespace TrueCareer.Repositories
{
    public interface IUOW : IServiceScoped, IDisposable
    {
        Task Begin();
        Task Commit();
        Task Rollback();

        IActiveTimeRepository ActiveTimeRepository { get; }
        IAppUserRepository AppUserRepository { get; }
        IChoiceRepository ChoiceRepository { get; }
        ICommentRepository CommentRepository { get; }
        IConnectionStatusRepository ConnectionStatusRepository { get; }
        IConnectionTypeRepository ConnectionTypeRepository { get; }
        IConversationRepository ConversationRepository { get; }
        IFavouriteMentorRepository FavouriteMentorRepository { get; }
        IFavouriteNewsRepository FavouriteNewsRepository { get; }
        IImageRepository ImageRepository { get; }
        IInformationRepository InformationRepository { get; }
        IInformationTypeRepository InformationTypeRepository { get; }
        IMajorRepository MajorRepository { get; }
        IMbtiPersonalTypeRepository MbtiPersonalTypeRepository { get; }
        IMbtiResultRepository MbtiResultRepository { get; }
        IMbtiSingleTypeRepository MbtiSingleTypeRepository { get; }
        IMentorConnectionRepository MentorConnectionRepository { get; }
        IMentorMenteeConnectionRepository MentorMenteeConnectionRepository { get; }
        IMentorReviewRepository MentorReviewRepository { get; }
        INewsRepository NewsRepository { get; }
        INewsStatusRepository NewsStatusRepository { get; }
        INotificationRepository NotificationRepository { get; }
        IQuestionRepository QuestionRepository { get; }
        IRoleRepository RoleRepository { get; }
        ISchoolRepository SchoolRepository { get; }
        ISexRepository SexRepository { get; }
        ITopicRepository TopicRepository { get; }
        IFileRepository FileRepository { get; }
        IConversationConfigurationRepository ConversationConfigurationRepository { get; }
        IConversationMessageRepository ConversationMessageRepository { get; }
        IConversationTypeRepository ConversationTypeRepository { get; }
        IConversationReadHistoryRepository ConversationReadHistoryRepository { get; }
        IGlobalUserRepository GlobalUserRepository { get; }
        IMentorRegisterRequestRepository MentorRegisterRequestRepository { get; }
    }

    public class UOW : IUOW
    {
        private DataContext DataContext;
        protected IMongoClient MongoClient = null;
        public IConversationRepository ConversationRepository { get; private set; }
        public IConversationConfigurationRepository ConversationConfigurationRepository { get; private set; }
        public IConversationMessageRepository ConversationMessageRepository { get; private set; }
        public IConversationTypeRepository ConversationTypeRepository { get; private set; }
        public IConversationReadHistoryRepository ConversationReadHistoryRepository { get; private set; }
        public IGlobalUserRepository GlobalUserRepository { get; private set; }
        public IActiveTimeRepository ActiveTimeRepository { get; private set; }
        public IAppUserRepository AppUserRepository { get; private set; }
        public IChoiceRepository ChoiceRepository { get; private set; }
        public ICommentRepository CommentRepository { get; private set; }
        public IConnectionStatusRepository ConnectionStatusRepository { get; private set; }
        public IConnectionTypeRepository ConnectionTypeRepository { get; private set; }
        public IFavouriteMentorRepository FavouriteMentorRepository { get; private set; }
        public IFavouriteNewsRepository FavouriteNewsRepository { get; private set; }
        public IImageRepository ImageRepository { get; private set; }
        public IInformationRepository InformationRepository { get; private set; }
        public IInformationTypeRepository InformationTypeRepository { get; private set; }
        public IMajorRepository MajorRepository { get; private set; }
        public IMbtiPersonalTypeRepository MbtiPersonalTypeRepository { get; private set; }
        public IMbtiResultRepository MbtiResultRepository { get; private set; }
        public IMbtiSingleTypeRepository MbtiSingleTypeRepository { get; private set; }
        public IMentorConnectionRepository MentorConnectionRepository { get; private set; }
        public IMentorMenteeConnectionRepository MentorMenteeConnectionRepository { get; private set; }
        public IMentorReviewRepository MentorReviewRepository { get; private set; }
        public INewsRepository NewsRepository { get; private set; }
        public INewsStatusRepository NewsStatusRepository { get; private set; }
        public INotificationRepository NotificationRepository { get; private set; }
        public IQuestionRepository QuestionRepository { get; private set; }
        public IRoleRepository RoleRepository { get; private set; }
        public ISchoolRepository SchoolRepository { get; private set; }
        public ISexRepository SexRepository { get; private set; }
        public ITopicRepository TopicRepository { get; private set; }
        public IFileRepository FileRepository { get; private set; }
        public IMentorRegisterRequestRepository MentorRegisterRequestRepository { get; private set; }

        public UOW(DataContext DataContext, IConfiguration Configuration)
        {
            this.DataContext = DataContext;
            MongoClient = new MongoClient(Configuration["MongoConnection:ConnectionString"]);
            ActiveTimeRepository = new ActiveTimeRepository(DataContext);
            AppUserRepository = new AppUserRepository(DataContext);
            ChoiceRepository = new ChoiceRepository(DataContext);
            CommentRepository = new CommentRepository(DataContext);
            ConnectionStatusRepository = new ConnectionStatusRepository(DataContext);
            ConnectionTypeRepository = new ConnectionTypeRepository(DataContext);
            ConversationRepository = new ConversationRepository(DataContext);
            FavouriteMentorRepository = new FavouriteMentorRepository(DataContext);
            FavouriteNewsRepository = new FavouriteNewsRepository(DataContext);
            ImageRepository = new ImageRepository(DataContext);
            InformationRepository = new InformationRepository(DataContext);
            InformationTypeRepository = new InformationTypeRepository(DataContext);
            MajorRepository = new MajorRepository(DataContext);
            MbtiPersonalTypeRepository = new MbtiPersonalTypeRepository(DataContext);
            MbtiResultRepository = new MbtiResultRepository(DataContext);
            MbtiSingleTypeRepository = new MbtiSingleTypeRepository(DataContext);
            MentorConnectionRepository = new MentorConnectionRepository(DataContext);
            MentorMenteeConnectionRepository = new MentorMenteeConnectionRepository(DataContext);
            MentorReviewRepository = new MentorReviewRepository(DataContext);
            NewsRepository = new NewsRepository(DataContext);
            NewsStatusRepository = new NewsStatusRepository(DataContext);
            NotificationRepository = new NotificationRepository(DataContext);
            QuestionRepository = new QuestionRepository(DataContext);
            RoleRepository = new RoleRepository(DataContext);
            SchoolRepository = new SchoolRepository(DataContext);
            SexRepository = new SexRepository(DataContext);
            TopicRepository = new TopicRepository(DataContext);
            FileRepository = new FileRepository(DataContext, MongoClient);
            ConversationRepository = new ConversationRepository(DataContext);
            ConversationConfigurationRepository = new ConversationConfigurationRepository(DataContext);
            ConversationMessageRepository = new ConversationMessageRepository(DataContext);
            ConversationTypeRepository = new ConversationTypeRepository(DataContext);
            ConversationReadHistoryRepository = new ConversationReadHistoryRepository(DataContext);
            GlobalUserRepository = new GlobalUserRepository(DataContext);
            MentorRegisterRequestRepository = new MentorRegisterRequestRepository(DataContext);
        }
        public async Task Begin()
        {
            return;
            await DataContext.Database.BeginTransactionAsync();
        }

        public Task Commit()
        {
            //DataContext.Database.CommitTransaction();
            return Task.CompletedTask;
        }

        public Task Rollback()
        {
            //DataContext.Database.RollbackTransaction();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (this.DataContext == null)
            {
                return;
            }

            this.DataContext.Dispose();
            this.DataContext = null;
        }
    }
}