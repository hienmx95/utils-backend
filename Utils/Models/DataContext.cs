using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Utils.Models
{
    public partial class DataContext : DbContext
    {
        public virtual DbSet<AggregatedCounterDAO> AggregatedCounter { get; set; }
        public virtual DbSet<AppUserDAO> AppUser { get; set; }
        public virtual DbSet<AppUserCommentMappingDAO> AppUserCommentMapping { get; set; }
        public virtual DbSet<AttachmentDAO> Attachment { get; set; }
        public virtual DbSet<ChatMessageDAO> ChatMessage { get; set; }
        public virtual DbSet<ChatMessageTypeDAO> ChatMessageType { get; set; }
        public virtual DbSet<CommentDAO> Comment { get; set; }
        public virtual DbSet<ConversationDAO> Conversation { get; set; }
        public virtual DbSet<ConversationMessageDAO> ConversationMessage { get; set; }
        public virtual DbSet<ConversationParticipantDAO> ConversationParticipant { get; set; }
        public virtual DbSet<ConversationTypeDAO> ConversationType { get; set; }
        public virtual DbSet<CounterDAO> Counter { get; set; }
        public virtual DbSet<FacebookMessageDAO> FacebookMessage { get; set; }
        public virtual DbSet<FacebookPageDAO> FacebookPage { get; set; }
        public virtual DbSet<FacebookPayloadDAO> FacebookPayload { get; set; }
        public virtual DbSet<FacebookUserDAO> FacebookUser { get; set; }
        public virtual DbSet<FileDAO> File { get; set; }
        public virtual DbSet<FirebaseTokenDAO> FirebaseToken { get; set; }
        public virtual DbSet<GlobalUserDAO> GlobalUser { get; set; }
        public virtual DbSet<HashDAO> Hash { get; set; }
        public virtual DbSet<JobDAO> Job { get; set; }
        public virtual DbSet<JobParameterDAO> JobParameter { get; set; }
        public virtual DbSet<JobQueueDAO> JobQueue { get; set; }
        public virtual DbSet<ListDAO> List { get; set; }
        public virtual DbSet<MailDAO> Mail { get; set; }
        public virtual DbSet<SchemaDAO> Schema { get; set; }
        public virtual DbSet<ServerDAO> Server { get; set; }
        public virtual DbSet<SetDAO> Set { get; set; }
        public virtual DbSet<StateDAO> State { get; set; }
        public virtual DbSet<UserNotificationDAO> UserNotification { get; set; }
        public virtual DbSet<ZaloAttachmentDAO> ZaloAttachment { get; set; }
        public virtual DbSet<ZaloAttachmentTypeDAO> ZaloAttachmentType { get; set; }
        public virtual DbSet<ZaloConfigurationDAO> ZaloConfiguration { get; set; }
        public virtual DbSet<ZaloMessageDAO> ZaloMessage { get; set; }
        public virtual DbSet<ZaloPayloadDAO> ZaloPayload { get; set; }
        public virtual DbSet<ZaloUserDAO> ZaloUser { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("data source=192.168.20.200;initial catalog=Utils;persist security info=True;user id=sa;password=123@123a;multipleactiveresultsets=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
                entity.Property(e => e.Id)
                    .HasComment("Id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .HasComment("Địa chỉ nhà");

                entity.Property(e => e.Avatar).HasComment("Ảnh đại diện");

                entity.Property(e => e.Birthday).HasColumnType("datetime");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày tạo");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày xoá");

                entity.Property(e => e.Department)
                    .HasMaxLength(500)
                    .HasComment("Phòng ban");

                entity.Property(e => e.DisplayName)
                    .HasMaxLength(500)
                    .HasComment("Tên hiển thị");

                entity.Property(e => e.Email)
                    .HasMaxLength(500)
                    .HasComment("Địa chỉ email");

                entity.Property(e => e.OrganizationId).HasComment("Đơn vị công tác");

                entity.Property(e => e.Phone)
                    .HasMaxLength(500)
                    .HasComment("Số điện thoại liên hệ");

                entity.Property(e => e.ProvinceId).HasComment("Tỉnh thành");

                entity.Property(e => e.RowId).HasComment("Trường để đồng bộ");

                entity.Property(e => e.StatusId).HasComment("Trạng thái");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày cập nhật");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasComment("Tên đăng nhập");
            });

            modelBuilder.Entity<AppUserCommentMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.AppUserId, e.CommentId });

                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.AppUserCommentMappings)
                    .HasForeignKey(d => d.AppUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppUserCommentMapping_AppUser");

                entity.HasOne(d => d.Comment)
                    .WithMany(p => p.AppUserCommentMappings)
                    .HasForeignKey(d => d.CommentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppUserCommentMapping_Comment");
            });

            modelBuilder.Entity<AttachmentDAO>(entity =>
            {
                entity.Property(e => e.ContentType).HasMaxLength(500);

                entity.Property(e => e.FileName).HasMaxLength(500);

                entity.Property(e => e.Url).HasMaxLength(4000);

                entity.HasOne(d => d.Mail)
                    .WithMany(p => p.Attachments)
                    .HasForeignKey(d => d.MailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Attachment_Mail");
            });

            modelBuilder.Entity<ChatMessageDAO>(entity =>
            {
                entity.Property(e => e.Content).HasMaxLength(4000);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.ChatMessageType)
                    .WithMany(p => p.ChatMessages)
                    .HasForeignKey(d => d.ChatMessageTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChatMessage_ChatMessageType");

                entity.HasOne(d => d.File)
                    .WithMany(p => p.ChatMessages)
                    .HasForeignKey(d => d.FileId)
                    .HasConstraintName("FK_ChatMessage_File");
            });

            modelBuilder.Entity<ChatMessageTypeDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<CommentDAO>(entity =>
            {
                entity.Property(e => e.Content).HasMaxLength(4000);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Url).HasMaxLength(4000);

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Comment_AppUser");
            });

            modelBuilder.Entity<ConversationDAO>(entity =>
            {
                entity.ToTable("Conversation", "CON");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<ConversationMessageDAO>(entity =>
            {
                entity.ToTable("ConversationMessage", "CON");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Conversation)
                    .WithMany(p => p.ConversationMessages)
                    .HasForeignKey(d => d.ConversationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ConversationMessage_Conversation");

                entity.HasOne(d => d.ConversationType)
                    .WithMany(p => p.ConversationMessages)
                    .HasForeignKey(d => d.ConversationTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ConversationMessage_ConversationType");

                entity.HasOne(d => d.GlobalUser)
                    .WithMany(p => p.ConversationMessages)
                    .HasForeignKey(d => d.GlobalUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ConversationMessage_GlobalUser");
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

            modelBuilder.Entity<FacebookMessageDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Page)
                    .WithMany(p => p.FacebookMessages)
                    .HasForeignKey(d => d.PageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FacebookMessage_FacebookPage");

                entity.HasOne(d => d.Recipient)
                    .WithMany(p => p.FacebookMessageRecipients)
                    .HasForeignKey(d => d.RecipientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FacebookMessage_FacebookUser1");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.FacebookMessageSenders)
                    .HasForeignKey(d => d.SenderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FacebookMessage_FacebookUser");
            });

            modelBuilder.Entity<FacebookPageDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.PageId)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<FacebookUserDAO>(entity =>
            {
                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(500);
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

                entity.Property(e => e.Path).HasMaxLength(4000);
            });

            modelBuilder.Entity<FirebaseTokenDAO>(entity =>
            {
                entity.Property(e => e.DeviceModel).HasMaxLength(4000);

                entity.Property(e => e.OsName).HasMaxLength(4000);

                entity.Property(e => e.OsVersion).HasMaxLength(4000);

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.FirebaseTokens)
                    .HasForeignKey(d => d.AppUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FirebaseToken_AppUser");
            });

            modelBuilder.Entity<GlobalUserDAO>(entity =>
            {
                entity.ToTable("GlobalUser", "CON");

                entity.HasIndex(e => e.RowId)
                    .IsUnique();

                entity.Property(e => e.DisplayName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(500);
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

                entity.Property(e => e.Recipients).IsRequired();
            });

            modelBuilder.Entity<SchemaDAO>(entity =>
            {
                entity.HasKey(e => e.Version)
                    .HasName("PK_HangFire_Schema");

                entity.ToTable("Schema", "HangFire");

                entity.Property(e => e.Version).ValueGeneratedNever();
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

            modelBuilder.Entity<UserNotificationDAO>(entity =>
            {
                entity.Property(e => e.ContentMobile)
                    .IsRequired()
                    .HasMaxLength(1000);

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
                    .WithMany(p => p.UserNotificationRecipients)
                    .HasForeignKey(d => d.RecipientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Notification_AppUser1");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.UserNotificationSenders)
                    .HasForeignKey(d => d.SenderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Notification_AppUser");
            });

            modelBuilder.Entity<ZaloAttachmentDAO>(entity =>
            {
                entity.Property(e => e.Checksum).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Size).HasMaxLength(50);

                entity.Property(e => e.Type).HasMaxLength(50);

                entity.Property(e => e.Url).IsRequired();

                entity.HasOne(d => d.ZaloAttachmentType)
                    .WithMany(p => p.ZaloAttachments)
                    .HasForeignKey(d => d.ZaloAttachmentTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ZaloAttachment_ZaloAttachmentType");

                entity.HasOne(d => d.ZaloMessage)
                    .WithMany(p => p.ZaloAttachments)
                    .HasForeignKey(d => d.ZaloMessageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ZaloAttachment_ZaloMessage");
            });

            modelBuilder.Entity<ZaloAttachmentTypeDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<ZaloConfigurationDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AppId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.AppName).HasMaxLength(500);

                entity.Property(e => e.AppSecret)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OASecretKey).HasMaxLength(50);
            });

            modelBuilder.Entity<ZaloMessageDAO>(entity =>
            {
                entity.Property(e => e.MsgId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Text).HasMaxLength(4000);

                entity.HasOne(d => d.Recipient)
                    .WithMany(p => p.ZaloMessageRecipients)
                    .HasForeignKey(d => d.RecipientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ZaloMessage_ZaloUser1");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.ZaloMessageSenders)
                    .HasForeignKey(d => d.SenderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ZaloMessage_ZaloUser");
            });

            modelBuilder.Entity<ZaloUserDAO>(entity =>
            {
                entity.HasIndex(e => e.UserId)
                    .HasName("IX_ZaloUser");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Username).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
