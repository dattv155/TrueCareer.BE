using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TrueCareer.Models
{
    public partial class DataContext : DbContext
    {
        public virtual DbSet<ActiveTimeDAO> ActiveTime { get; set; }
        public virtual DbSet<AppUserDAO> AppUser { get; set; }
        public virtual DbSet<AppUserRoleMappingDAO> AppUserRoleMapping { get; set; }
        public virtual DbSet<ChoiceDAO> Choice { get; set; }
        public virtual DbSet<CommentDAO> Comment { get; set; }
        public virtual DbSet<ConnectionStatusDAO> ConnectionStatus { get; set; }
        public virtual DbSet<ConnectionTypeDAO> ConnectionType { get; set; }
        public virtual DbSet<ConversationDAO> Conversation { get; set; }
        public virtual DbSet<ConversationParticipantDAO> ConversationParticipant { get; set; }
        public virtual DbSet<FavouriteMentorDAO> FavouriteMentor { get; set; }
        public virtual DbSet<FavouriteNewsDAO> FavouriteNews { get; set; }
        public virtual DbSet<ImageDAO> Image { get; set; }
        public virtual DbSet<InformationDAO> Information { get; set; }
        public virtual DbSet<InformationTypeDAO> InformationType { get; set; }
        public virtual DbSet<MajorDAO> Major { get; set; }
        public virtual DbSet<MbtiPersonalTypeDAO> MbtiPersonalType { get; set; }
        public virtual DbSet<MbtiPersonalTypeMajorMappingDAO> MbtiPersonalTypeMajorMapping { get; set; }
        public virtual DbSet<MbtiResultDAO> MbtiResult { get; set; }
        public virtual DbSet<MbtiSingleTypeDAO> MbtiSingleType { get; set; }
        public virtual DbSet<MentorConnectionDAO> MentorConnection { get; set; }
        public virtual DbSet<MentorMenteeConnectionDAO> MentorMenteeConnection { get; set; }
        public virtual DbSet<MentorReviewDAO> MentorReview { get; set; }
        public virtual DbSet<MessageDAO> Message { get; set; }
        public virtual DbSet<NewsDAO> News { get; set; }
        public virtual DbSet<NewsStatusDAO> NewsStatus { get; set; }
        public virtual DbSet<NotificationDAO> Notification { get; set; }
        public virtual DbSet<QuestionDAO> Question { get; set; }
        public virtual DbSet<RoleDAO> Role { get; set; }
        public virtual DbSet<SchoolDAO> School { get; set; }
        public virtual DbSet<SchoolMajorMappingDAO> SchoolMajorMapping { get; set; }
        public virtual DbSet<SexDAO> Sex { get; set; }
        public virtual DbSet<TopicDAO> Topic { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=LAPTOP-PO83DCLI\\SQLEXPRESS;Initial Catalog=TrueCareer;Persist Security Info=True;User ID=sa;Password=kandayuu1996;multipleactiveresultsets=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActiveTimeDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.EndAt).HasColumnType("datetime");

                entity.Property(e => e.StartAt).HasColumnType("datetime");

                entity.HasOne(d => d.Mentor)
                    .WithMany(p => p.ActiveTimes)
                    .HasForeignKey(d => d.MentorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActiveTime_AppUser");
            });

            modelBuilder.Entity<AppUserDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

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

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Sex)
                    .WithMany(p => p.AppUsers)
                    .HasForeignKey(d => d.SexId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Fk_AppUser_Sex");
            });

            modelBuilder.Entity<AppUserRoleMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.AppUserId, e.RoleId })
                    .HasName("PK_AppUserRoleMapping_1");

                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.AppUserRoleMappings)
                    .HasForeignKey(d => d.AppUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppUserRoleMapping_AppUser");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AppUserRoleMappings)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppUserRoleMapping_Role");
            });

            modelBuilder.Entity<ChoiceDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

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
                entity.Property(e => e.Id).ValueGeneratedNever();

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
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Hash).HasMaxLength(4000);

                entity.Property(e => e.LatestContent).HasMaxLength(4000);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<ConversationParticipantDAO>(entity =>
            {
                entity.HasNoKey();

                entity.HasOne(d => d.Conversation)
                    .WithMany()
                    .HasForeignKey(d => d.ConversationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ConversationParticipant_Conversation");

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ConversationParticipant_AppUser");
            });

            modelBuilder.Entity<FavouriteMentorDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

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
                entity.Property(e => e.Id).ValueGeneratedNever();

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

            modelBuilder.Entity<ImageDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

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
                entity.Property(e => e.Id).ValueGeneratedNever();

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
                    .OnDelete(DeleteBehavior.ClientSetNull)
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

            modelBuilder.Entity<MajorDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

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
                entity.Property(e => e.Id).ValueGeneratedNever();

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

            modelBuilder.Entity<MentorConnectionDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

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

            modelBuilder.Entity<MentorMenteeConnectionDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Connection)
                    .WithMany(p => p.MentorMenteeConnections)
                    .HasForeignKey(d => d.ConnectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MentorMenteeConnection_MentorConnection");

                entity.HasOne(d => d.ConnectionStatus)
                    .WithMany(p => p.MentorMenteeConnections)
                    .HasForeignKey(d => d.ConnectionStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MentorMenteeConnection_ConnectionStatus");

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

            modelBuilder.Entity<MentorReviewDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

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

            modelBuilder.Entity<MessageDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Conversation)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.ConversationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Message_Conversation");
            });

            modelBuilder.Entity<NewsDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.NewsContent).IsRequired();

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
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ContentWeb)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.LinkWebsite).HasMaxLength(4000);

                entity.Property(e => e.Time).HasColumnType("datetime");

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
                entity.Property(e => e.Id).ValueGeneratedNever();

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

            modelBuilder.Entity<SchoolDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(4000);
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

            modelBuilder.Entity<TopicDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
