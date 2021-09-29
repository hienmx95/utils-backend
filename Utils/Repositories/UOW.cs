using Utils.Common;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Models;
using Z.EntityFramework.Extensions;

namespace Utils.Repositories
{
    public interface IUOW : IServiceScoped, IDisposable
    {
        Task Begin();
        Task Commit();
        Task Rollback();
        IAppUserRepository AppUserRepository { get; }
        IAuditLogRepository AuditLogRepository { get; }
        ISystemLogRepository SystemLogRepository { get; }
        ICommentRepository CommentRepository { get; }
        IConversationRepository ConversationRepository { get; }
        IConversationMessageRepository ConversationMessageRepository { get; }
        IConversationTypeRepository ConversationTypeRepository { get; }
        IMailRepository MailRepository { get; }
        IUserNotificationRepository UserNotificationRepository { get; }
        IFileRepository FileRepository { get; }
        IGlobalUserRepository GlobalUserRepository { get; }
        IChatMessageRepository ChatMessageRepository { get; }
    }
    public class UOW : IUOW
    {
        private DataContext DataContext;
        protected IMongoClient MongoClient = null;
        public IAppUserRepository AppUserRepository { get; private set; }
        public IAuditLogRepository AuditLogRepository { get; private set; }
        public ISystemLogRepository SystemLogRepository { get; private set; }
        public ICommentRepository CommentRepository { get; private set; }
        public IConversationRepository ConversationRepository { get; private set; }
        public IConversationMessageRepository ConversationMessageRepository { get; private set; }
        public IConversationTypeRepository ConversationTypeRepository { get; private set; }
        public IUserNotificationRepository UserNotificationRepository { get; private set; }
        public IMailRepository MailRepository { get; private set; }
        public IFileRepository FileRepository { get; private set; }
        public IGlobalUserRepository GlobalUserRepository { get; private set; }
        public IChatMessageRepository ChatMessageRepository { get; }

        public UOW(DataContext DataContext, IConfiguration Configuration)
        {
            this.DataContext = DataContext;
            MongoClient = new MongoClient(Configuration["MongoConnection:ConnectionString"]);
            AuditLogRepository = new AuditLogRepository(Configuration, MongoClient);
            SystemLogRepository = new SystemLogRepository(Configuration, MongoClient);
            AppUserRepository = new AppUserRepository(DataContext);
            CommentRepository = new CommentRepository(DataContext);
            ConversationRepository = new ConversationRepository(DataContext);
            ConversationMessageRepository = new ConversationMessageRepository(DataContext);
            ConversationTypeRepository = new ConversationTypeRepository(DataContext);
            MailRepository = new MailRepository(DataContext);
            UserNotificationRepository = new UserNotificationRepository(DataContext);
            FileRepository = new FileRepository(DataContext, MongoClient);
            GlobalUserRepository = new GlobalUserRepository(DataContext);
            ChatMessageRepository = new ChatMessageRepository(DataContext);
        }

        public async Task Begin()
        {
            return;
        }

        public Task Commit()
        {
            return Task.CompletedTask;
        }

        public Task Rollback()
        {
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
