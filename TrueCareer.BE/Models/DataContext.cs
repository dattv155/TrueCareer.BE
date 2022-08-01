using System;
using Thinktecture;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TrueCareer.BE.Models
{
    public partial class DataContext : DbContext
    {
        public virtual DbSet<ActiveTimeDAO> ActiveTime { get; set; }
        public virtual DbSet<AggregatedCounterDAO> AggregatedCounter { get; set; }
        public virtual DbSet<AppUserDAO> AppUser { get; set; }
        public virtual DbSet<ChoiceDAO> Choice { get; set; }
        public virtual DbSet<CommentDAO> Comment { get; set; }
        public virtual DbSet<ConnectionStatusDAO> ConnectionStatus { get; set; }
        public virtual DbSet<ConnectionTypeDAO> ConnectionType { get; set; }
        public virtual DbSet<ConversationDAO> Conversation { get; set; }
        public virtual DbSet<ConversationAttachmentDAO> ConversationAttachment { get; set; }
        public virtual DbSet<ConversationAttachmentTypeDAO> ConversationAttachmentType { get; set; }
        public virtual DbSet<ConversationConfigurationDAO> ConversationConfiguration { get; set; }
        public virtual DbSet<ConversationMessageDAO> ConversationMessage { get; set; }
        public virtual DbSet<ConversationParticipantDAO> ConversationParticipant { get; set; }
        public virtual DbSet<ConversationReadHistoryDAO> ConversationReadHistory { get; set; }
        public virtual DbSet<ConversationTypeDAO> ConversationType { get; set; }
        public virtual DbSet<CounterDAO> Counter { get; set; }
        public virtual DbSet<FavouriteMentorDAO> FavouriteMentor { get; set; }
        public virtual DbSet<FavouriteNewsDAO> FavouriteNews { get; set; }
        public virtual DbSet<FileDAO> File { get; set; }
        public virtual DbSet<FirebaseTokenDAO> FirebaseToken { get; set; }
        public virtual DbSet<GlobalUserDAO> GlobalUser { get; set; }
        public virtual DbSet<GlobalUserTypeDAO> GlobalUserType { get; set; }
        public virtual DbSet<HashDAO> Hash { get; set; }
        public virtual DbSet<ImageDAO> Image { get; set; }
        public virtual DbSet<InformationDAO> Information { get; set; }
        public virtual DbSet<InformationTypeDAO> InformationType { get; set; }
        public virtual DbSet<JobDAO> Job { get; set; }
        public virtual DbSet<JobParameterDAO> JobParameter { get; set; }
        public virtual DbSet<JobQueueDAO> JobQueue { get; set; }
        public virtual DbSet<ListDAO> List { get; set; }
        public virtual DbSet<MailDAO> Mail { get; set; }
        public virtual DbSet<MajorDAO> Major { get; set; }
        public virtual DbSet<MbtiPersonalTypeDAO> MbtiPersonalType { get; set; }
        public virtual DbSet<MbtiPersonalTypeMajorMappingDAO> MbtiPersonalTypeMajorMapping { get; set; }
        public virtual DbSet<MbtiResultDAO> MbtiResult { get; set; }
        public virtual DbSet<MbtiSingleTypeDAO> MbtiSingleType { get; set; }
        public virtual DbSet<MentorApprovalStatusDAO> MentorApprovalStatus { get; set; }
        public virtual DbSet<MentorConnectionDAO> MentorConnection { get; set; }
        public virtual DbSet<MentorInfoDAO> MentorInfo { get; set; }
        public virtual DbSet<MentorMenteeConnectionDAO> MentorMenteeConnection { get; set; }
        public virtual DbSet<MentorRegisterRequestDAO> MentorRegisterRequest { get; set; }
        public virtual DbSet<MentorReviewDAO> MentorReview { get; set; }
        public virtual DbSet<NewsDAO> News { get; set; }
        public virtual DbSet<NewsStatusDAO> NewsStatus { get; set; }
        public virtual DbSet<NotificationDAO> Notification { get; set; }
        public virtual DbSet<QuestionDAO> Question { get; set; }
        public virtual DbSet<RoleDAO> Role { get; set; }
        public virtual DbSet<SchemaDAO> Schema { get; set; }
        public virtual DbSet<SchoolDAO> School { get; set; }
        public virtual DbSet<SchoolMajorMappingDAO> SchoolMajorMapping { get; set; }
        public virtual DbSet<ServerDAO> Server { get; set; }
        public virtual DbSet<SetDAO> Set { get; set; }
        public virtual DbSet<SexDAO> Sex { get; set; }
        public virtual DbSet<StateDAO> State { get; set; }
        public virtual DbSet<StatusDAO> Status { get; set; }
        public virtual DbSet<TopicDAO> Topic { get; set; }
        public virtual DbSet<UnitOfTimeDAO> UnitOfTime { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("data source=222.252.27.58,2022;initial catalog=TrueCareer;persist security info=True;user id=sa;password=123@123a;multipleactiveresultsets=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureTempTable<long>();
            modelBuilder.ConfigureTempTable<Guid>();

            modelBuilder.Entity<ActiveTimeDAO>(entity =>
            {
                entity.Property(e => e.ActiveDate).HasColumnType("datetime");

                entity.HasOne(d => d.Mentor)
                    .WithMany(p => p.ActiveTimes)
                    .HasForeignKey(d => d.MentorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActiveTime_AppUser");

                entity.HasOne(d => d.UnitOfTime)
                    .WithMany(p => p.ActiveTimes)
                    .HasForeignKey(d => d.UnitOfTimeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActiveTime_UnitOfTime");
            });

            modelBuilder.Entity<AggregatedCounterDAO>(entity =>
            {
                entity.HasKey(e => e.Key)
                    .HasName("PK_HangFire_CounterAggregated");

                entity.ToTable("AggregatedCounter", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_AggregatedCounter_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<AppUserDAO>(entity =>
            {
                entity.Property(e => e.Avatar).HasMaxLength(4000);

                entity.Property(e => e.Birthday).HasColumnType("datetime");

                entity.Property(e => e.CoverImage).HasMaxLength(4000);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.DisplayName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.OtpCode).HasMaxLength(50);

                entity.Property(e => e.OtpExpired).HasColumnType("datetime");

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AppUsers)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_AppUser_Role");

                entity.HasOne(d => d.Sex)
                    .WithMany(p => p.AppUsers)
                    .HasForeignKey(d => d.SexId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Fk_AppUser_Sex");
            });

            modelBuilder.Entity<ChoiceDAO>(entity =>
            {
                entity.Property(e => e.ChoiceContent)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.HasOne(d => d.MbtiSingleType)
                    .WithMany(p => p.Choices)
                    .HasForeignKey(d => d.MbtiSingleTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Choice_MbtiSingleType");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.Choices)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Choice_Question");
            });

            modelBuilder.Entity<CommentDAO>(entity =>
            {
                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Comment_AppUser");
            });

            modelBuilder.Entity<ConnectionStatusDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<ConnectionTypeDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<ConversationDAO>(entity =>
            {
                entity.ToTable("Conversation", "CON");

                entity.Property(e => e.Avatar).HasMaxLength(4000);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Hash).HasMaxLength(4000);

                entity.Property(e => e.LatestContent).HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.RowId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.StatusId).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.ConversationConfiguration)
                    .WithMany(p => p.Conversations)
                    .HasForeignKey(d => d.ConversationConfigurationId)
                    .HasConstraintName("FK_Conversation_ConversationConfiguration");

                entity.HasOne(d => d.ConversationType)
                    .WithMany(p => p.Conversations)
                    .HasForeignKey(d => d.ConversationTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Conversation_ConversationType");

                entity.HasOne(d => d.LatestGlobalUser)
                    .WithMany(p => p.Conversations)
                    .HasForeignKey(d => d.LatestGlobalUserId)
                    .HasConstraintName("FK_Conversation_GlobalUser");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Conversations)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Conversation_Status");
            });

            modelBuilder.Entity<ConversationAttachmentDAO>(entity =>
            {
                entity.ToTable("ConversationAttachment", "CON");

                entity.Property(e => e.Checksum).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Size).HasMaxLength(50);

                entity.Property(e => e.Thumbnail).HasMaxLength(4000);

                entity.Property(e => e.Type).HasMaxLength(50);

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.HasOne(d => d.ConversationAttachmentType)
                    .WithMany(p => p.ConversationAttachments)
                    .HasForeignKey(d => d.ConversationAttachmentTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ConversationAttachment_ConversationAttachmentType");

                entity.HasOne(d => d.ConversationMessage)
                    .WithMany(p => p.ConversationAttachments)
                    .HasForeignKey(d => d.ConversationMessageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ConversationAttachment_ConversationMessage");
            });

            modelBuilder.Entity<ConversationAttachmentTypeDAO>(entity =>
            {
                entity.ToTable("ConversationAttachmentType", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<ConversationConfigurationDAO>(entity =>
            {
                entity.ToTable("ConversationConfiguration", "CON");

                entity.Property(e => e.AppId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.AppName).HasMaxLength(500);

                entity.Property(e => e.AppSecret).HasMaxLength(50);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("((2021))");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.ExpiredAt).HasColumnType("datetime");

                entity.Property(e => e.OaId).HasMaxLength(50);

                entity.Property(e => e.OaSecretKey).HasMaxLength(50);

                entity.Property(e => e.OaToken).HasMaxLength(500);

                entity.Property(e => e.StatusId).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.ConversationType)
                    .WithMany(p => p.ConversationConfigurations)
                    .HasForeignKey(d => d.ConversationTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ConversationConfiguration_ConversationType");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.ConversationConfigurations)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ConversationConfiguration_Status");
            });

            modelBuilder.Entity<ConversationMessageDAO>(entity =>
            {
                entity.ToTable("ConversationMessage", "CON");

                entity.Property(e => e.Content).HasMaxLength(4000);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Conversation)
                    .WithMany(p => p.ConversationMessages)
                    .HasForeignKey(d => d.ConversationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ConversationMessage_Conversation");

                entity.HasOne(d => d.GlobalUser)
                    .WithMany(p => p.ConversationMessages)
                    .HasForeignKey(d => d.GlobalUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ConversationMessage_GlobalUser");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ConversationMessages)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ConversationMessage_AppUser");
            });

            modelBuilder.Entity<ConversationParticipantDAO>(entity =>
            {
                entity.ToTable("ConversationParticipant", "CON");

                entity.HasOne(d => d.Conversation)
                    .WithMany(p => p.ConversationParticipants)
                    .HasForeignKey(d => d.ConversationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Participant_Conversation");

                entity.HasOne(d => d.GlobalUser)
                    .WithMany(p => p.ConversationParticipants)
                    .HasForeignKey(d => d.GlobalUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Participant_GlobalUser");


            });

            modelBuilder.Entity<ConversationReadHistoryDAO>(entity =>
            {
                entity.ToTable("ConversationReadHistory", "CON");

                entity.Property(e => e.ReadAt).HasColumnType("datetime");

                entity.HasOne(d => d.Conversation)
                    .WithMany(p => p.ConversationReadHistories)
                    .HasForeignKey(d => d.ConversationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ConversationReadHistory_Conversation");

                entity.HasOne(d => d.GlobalUser)
                    .WithMany(p => p.ConversationReadHistories)
                    .HasForeignKey(d => d.GlobalUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ConversationReadHistory_GlobalUser");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ConversationReadHistories)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ConversationReadHistory_AppUser");
            });

            modelBuilder.Entity<ConversationTypeDAO>(entity =>
            {
                entity.ToTable("ConversationType", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<CounterDAO>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Counter", "HangFire");

                entity.HasIndex(e => e.Key)
                    .HasName("CX_HangFire_Counter")
                    .IsClustered();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<FavouriteMentorDAO>(entity =>
            {
                entity.HasOne(d => d.Mentor)
                    .WithMany(p => p.FavouriteMentorMentors)
                    .HasForeignKey(d => d.MentorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FavouriteMentor_AppUser1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.FavouriteMentorUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FavouriteMentor_AppUser");
            });

            modelBuilder.Entity<FavouriteNewsDAO>(entity =>
            {
                entity.HasOne(d => d.News)
                    .WithMany(p => p.FavouriteNews)
                    .HasForeignKey(d => d.NewsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FavouriteNews_News");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.FavouriteNews)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FavouriteNews_AppUser");
            });

            modelBuilder.Entity<FileDAO>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.GridId).HasMaxLength(500);

                entity.Property(e => e.Key).HasMaxLength(500);

                entity.Property(e => e.MimeType).HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.OriginalName).HasMaxLength(500);

                entity.Property(e => e.Path).HasMaxLength(4000);
            });

            modelBuilder.Entity<FirebaseTokenDAO>(entity =>
            {
                entity.ToTable("FirebaseToken", "MDM");

                entity.Property(e => e.DeviceModel).HasMaxLength(4000);

                entity.Property(e => e.OsName).HasMaxLength(4000);

                entity.Property(e => e.OsVersion).HasMaxLength(4000);

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.GlobalUser)
                    .WithMany(p => p.FirebaseTokens)
                    .HasForeignKey(d => d.GlobalUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FirebaseToken_GlobalUser");
            });

            modelBuilder.Entity<GlobalUserDAO>(entity =>
            {
                entity.ToTable("GlobalUser", "MDM");

                entity.HasIndex(e => e.RowId)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Avatar).HasMaxLength(4000);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.DisplayName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.GlobalUserType)
                    .WithMany(p => p.GlobalUsers)
                    .HasForeignKey(d => d.GlobalUserTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GlobalUser_GlobalUserType");
            });

            modelBuilder.Entity<GlobalUserTypeDAO>(entity =>
            {
                entity.ToTable("GlobalUserType", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<HashDAO>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Field })
                    .HasName("PK_HangFire_Hash");

                entity.ToTable("Hash", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_Hash_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Field).HasMaxLength(100);
            });

            modelBuilder.Entity<ImageDAO>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.ThumbnailUrl).HasMaxLength(4000);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<InformationDAO>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.EndAt).HasColumnType("datetime");

                entity.Property(e => e.Image)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.StartAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.InformationType)
                    .WithMany(p => p.Information)
                    .HasForeignKey(d => d.InformationTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Information_InformationType");

                entity.HasOne(d => d.Topic)
                    .WithMany(p => p.Information)
                    .HasForeignKey(d => d.TopicId)
                    .HasConstraintName("FK_Information_Topic");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Information)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Information_AppUser");
            });

            modelBuilder.Entity<InformationTypeDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<JobDAO>(entity =>
            {
                entity.ToTable("Job", "HangFire");

                entity.HasIndex(e => e.StateName)
                    .HasName("IX_HangFire_Job_StateName")
                    .HasFilter("([StateName] IS NOT NULL)");

                entity.HasIndex(e => new { e.StateName, e.ExpireAt })
                    .HasName("IX_HangFire_Job_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Arguments).IsRequired();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.InvocationData).IsRequired();

                entity.Property(e => e.StateName).HasMaxLength(20);
            });

            modelBuilder.Entity<JobParameterDAO>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Name })
                    .HasName("PK_HangFire_JobParameter");

                entity.ToTable("JobParameter", "HangFire");

                entity.Property(e => e.Name).HasMaxLength(40);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobParameters)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_JobParameter_Job");
            });

            modelBuilder.Entity<JobQueueDAO>(entity =>
            {
                entity.HasKey(e => new { e.Queue, e.Id })
                    .HasName("PK_HangFire_JobQueue");

                entity.ToTable("JobQueue", "HangFire");

                entity.Property(e => e.Queue).HasMaxLength(50);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.FetchedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<ListDAO>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Id })
                    .HasName("PK_HangFire_List");

                entity.ToTable("List", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_List_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<MailDAO>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Error).HasMaxLength(4000);

                entity.Property(e => e.Password).HasMaxLength(500);

                entity.Property(e => e.Recipients).IsRequired();

                entity.Property(e => e.Username).HasMaxLength(500);
            });

            modelBuilder.Entity<MajorDAO>(entity =>
            {
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<MbtiPersonalTypeDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<MbtiPersonalTypeMajorMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.MbtiPersonalTypeId, e.MajorId })
                    .HasName("PK_MbtiPersonalTypeMajorMapping_1");

                entity.HasOne(d => d.Major)
                    .WithMany(p => p.MbtiPersonalTypeMajorMappings)
                    .HasForeignKey(d => d.MajorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MbtiPersonalTypeMajorMapping_Major");

                entity.HasOne(d => d.MbtiPersonalType)
                    .WithMany(p => p.MbtiPersonalTypeMajorMappings)
                    .HasForeignKey(d => d.MbtiPersonalTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MbtiPersonalTypeMajorMapping_MbtiPersonalType");
            });

            modelBuilder.Entity<MbtiResultDAO>(entity =>
            {
                entity.HasOne(d => d.MbtiPersonalType)
                    .WithMany(p => p.MbtiResults)
                    .HasForeignKey(d => d.MbtiPersonalTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MbtiResult_MbtiPersonalType");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MbtiResults)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MbtiResult_AppUser");
            });

            modelBuilder.Entity<MbtiSingleTypeDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<MentorApprovalStatusDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<MentorConnectionDAO>(entity =>
            {
                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.HasOne(d => d.ConnectionType)
                    .WithMany(p => p.MentorConnections)
                    .HasForeignKey(d => d.ConnectionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MentorConnection_ConnectionType");

                entity.HasOne(d => d.Mentor)
                    .WithMany(p => p.MentorConnections)
                    .HasForeignKey(d => d.MentorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MentorConnection_AppUser");
            });

            modelBuilder.Entity<MentorInfoDAO>(entity =>
            {
                entity.Property(e => e.ConnectionUrl).HasMaxLength(4000);

                entity.Property(e => e.TopicDescription).HasMaxLength(4000);
            });

            modelBuilder.Entity<MentorMenteeConnectionDAO>(entity =>
            {
                entity.HasOne(d => d.ActiveTime)
                    .WithMany(p => p.MentorMenteeConnections)
                    .HasForeignKey(d => d.ActiveTimeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MentorMenteeConnection_ActiveTime");

                entity.HasOne(d => d.Connection)
                    .WithMany(p => p.MentorMenteeConnections)
                    .HasForeignKey(d => d.ConnectionId)
                    .HasConstraintName("FK_MentorMenteeConnection_MentorConnection");

                entity.HasOne(d => d.ConnectionStatus)
                    .WithMany(p => p.MentorMenteeConnections)
                    .HasForeignKey(d => d.ConnectionStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MentorMenteeConnection_ConnectionStatus");

                entity.HasOne(d => d.Conversation)
                    .WithMany(p => p.MentorMenteeConnections)
                    .HasForeignKey(d => d.ConversationId)
                    .HasConstraintName("FK_MentorMenteeConnection_Conversation");

                entity.HasOne(d => d.Mentee)
                    .WithMany(p => p.MentorMenteeConnectionMentees)
                    .HasForeignKey(d => d.MenteeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MentorMenteeConnection_AppUser1");

                entity.HasOne(d => d.Mentor)
                    .WithMany(p => p.MentorMenteeConnectionMentors)
                    .HasForeignKey(d => d.MentorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MentorMenteeConnection_AppUser");
            });

            modelBuilder.Entity<MentorRegisterRequestDAO>(entity =>
            {
                entity.HasOne(d => d.MentorApprovalStatus)
                    .WithMany(p => p.MentorRegisterRequests)
                    .HasForeignKey(d => d.MentorApprovalStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MentorRegisterRequest_MentorApprovalStatus");

                entity.HasOne(d => d.Topic)
                    .WithMany(p => p.MentorRegisterRequests)
                    .HasForeignKey(d => d.TopicId)
                    .HasConstraintName("FK_MentorRegisterRequest_Topic");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MentorRegisterRequests)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MentorRegisterRequest_AppUser");
            });

            modelBuilder.Entity<MentorReviewDAO>(entity =>
            {
                entity.Property(e => e.ContentReview).IsRequired();

                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.Time).HasColumnType("datetime");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.MentorReviewCreators)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MentorReview_AppUser1");

                entity.HasOne(d => d.Mentor)
                    .WithMany(p => p.MentorReviewMentors)
                    .HasForeignKey(d => d.MentorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MentorReview_AppUser");
            });

            modelBuilder.Entity<NewsDAO>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.NewsContent).IsRequired();

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.News)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_News_AppUser");

                entity.HasOne(d => d.NewsStatus)
                    .WithMany(p => p.News)
                    .HasForeignKey(d => d.NewsStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_News_NewsStatus");
            });

            modelBuilder.Entity<NewsStatusDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<NotificationDAO>(entity =>
            {
                entity.Property(e => e.ContentMobile)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ContentWeb)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.LinkMobile).HasMaxLength(4000);

                entity.Property(e => e.LinkWebsite).HasMaxLength(4000);

                entity.Property(e => e.Time).HasColumnType("datetime");

                entity.Property(e => e.TitleMobile)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.TitleWeb)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.HasOne(d => d.Recipient)
                    .WithMany(p => p.NotificationRecipients)
                    .HasForeignKey(d => d.RecipientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Notification_AppUser1");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.NotificationSenders)
                    .HasForeignKey(d => d.SenderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Notification_AppUser");
            });

            modelBuilder.Entity<QuestionDAO>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.QuestionContent)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<RoleDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code).HasMaxLength(4000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<SchemaDAO>(entity =>
            {
                entity.HasKey(e => e.Version)
                    .HasName("PK_HangFire_Schema");

                entity.ToTable("Schema", "HangFire");

                entity.Property(e => e.Version).ValueGeneratedNever();
            });

            modelBuilder.Entity<SchoolDAO>(entity =>
            {
                entity.Property(e => e.CompleteTime)
                    .HasMaxLength(4000)
                    .HasComment("Số năm học");

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.PhoneNumber).HasMaxLength(50);

                entity.Property(e => e.Rating).HasColumnType("decimal(20, 4)");
            });

            modelBuilder.Entity<SchoolMajorMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.SchoolId, e.MajorId })
                    .HasName("PK_SchoolMajorMapping_1");

                entity.HasOne(d => d.Major)
                    .WithMany(p => p.SchoolMajorMappings)
                    .HasForeignKey(d => d.MajorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SchoolMajorMapping_Major");

                entity.HasOne(d => d.School)
                    .WithMany(p => p.SchoolMajorMappings)
                    .HasForeignKey(d => d.SchoolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SchoolMajorMapping_School");
            });

            modelBuilder.Entity<ServerDAO>(entity =>
            {
                entity.ToTable("Server", "HangFire");

                entity.HasIndex(e => e.LastHeartbeat)
                    .HasName("IX_HangFire_Server_LastHeartbeat");

                entity.Property(e => e.Id).HasMaxLength(100);

                entity.Property(e => e.LastHeartbeat).HasColumnType("datetime");
            });

            modelBuilder.Entity<SetDAO>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Value })
                    .HasName("PK_HangFire_Set");

                entity.ToTable("Set", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_Set_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.HasIndex(e => new { e.Key, e.Score })
                    .HasName("IX_HangFire_Set_Score");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Value).HasMaxLength(256);

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<SexDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<StateDAO>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Id })
                    .HasName("PK_HangFire_State");

                entity.ToTable("State", "HangFire");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Reason).HasMaxLength(100);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.States)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_State_Job");
            });

            modelBuilder.Entity<StatusDAO>(entity =>
            {
                entity.ToTable("Status", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TopicDAO>(entity =>
            {
                entity.Property(e => e.Cost)
                    .HasColumnType("decimal(20, 4)")
                    .HasDefaultValueSql("((0.00))");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<UnitOfTimeDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
