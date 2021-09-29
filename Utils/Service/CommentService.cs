using Utils.Common;
using Utils.Helpers;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Entities;
using Utils.Repositories;

namespace Utils.Service
{
    public interface ICommentService : IServiceScoped
    {
        Task<int> Count(Guid DiscussionId);
        Task<List<Comment>> List(Guid DiscussionId, OrderType OrderType);
        Task<Comment> Create(Comment Comment);
        Task<Comment> Update(Comment Comment);
        Task<bool> Delete(Comment Comment);
    }
    public class CommentService : ICommentService
    {
        private IUOW UOW;
        private IUserNotificationService UserNotificationService;
        private ICurrentContext CurrentContext;
        public CommentService(IUOW UOW, IUserNotificationService UserNotificationService, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.UserNotificationService = UserNotificationService;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(Guid DiscussionId)
        {
            return await UOW.CommentRepository.Count(DiscussionId);
        }
        public async Task<List<Comment>> List(Guid DiscussionId, OrderType OrderType)
        {
            List<Comment> Comments = await UOW.CommentRepository.List(DiscussionId, OrderType);
            foreach(Comment Comment in Comments)
            {
                if (Comment.CreatorId == CurrentContext.UserId)
                    Comment.IsOwner = true;
                else
                    Comment.IsOwner = false;

            }    
            return Comments;
        }

        public async Task<Comment> Create(Comment Comment)
        {
            Comment.CreatorId = CurrentContext.UserId;
            await UOW.CommentRepository.Create(Comment);
            await SendNotifications(Comment);
            await UOW.CommentRepository.Update(Comment);
            return await UOW.CommentRepository.Get(Comment.Id);
        }

        public async Task<Comment> Update(Comment Comment)
        {
            await UOW.CommentRepository.Update(Comment);
            await SendNotifications(Comment);
            return await UOW.CommentRepository.Get(Comment.Id);
        }

        public async Task<bool> Delete(Comment Comment)
        {
            await UOW.CommentRepository.Delete(Comment);
            return true;
        }

        private async Task SendNotifications(Comment Comment)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(Comment.Content);

                var links = doc.DocumentNode.SelectNodes("//input[@data-id]").ToList();
                List<long> Ids = links.Select(l => l.Attributes["data-id"].Value).Select(x => long.TryParse(x, out long result) ? result : 0).Distinct().ToList();
                List<AppUser> AppUsers = await UOW.AppUserRepository.List(new AppUserFilter
                {
                    Id = new IdFilter { In = Ids },
                    Skip = 0,
                    Take = Ids.Count,
                    Selects = AppUserSelect.ALL
                });
                List<AppUserNotification> UserNotifications = new List<AppUserNotification>();
                List<AppUserCommentMapping> AppUserCommentMappings = new List<AppUserCommentMapping>();
                foreach (AppUser AppUser in AppUsers)
                {
                    AppUserNotification UserNotification = new AppUserNotification
                    {
                        SenderId = CurrentContext.UserId,
                        RecipientId = AppUser.Id,
                        TitleWeb = "Bình luận",
                        ContentWeb = $"{CurrentContext.UserName} đã nhắc đến bạn trong một bình luận.",
                        LinkMobile = Comment.Url,
                        LinkWebsite = Comment.Url,
                        Time = StaticParams.DateTimeNow,
                        Unread = true,
                    };

                    if (Comment.Url != null)
                    {
                        if (Comment.Url.ToLower().Contains("portal"))
                            UserNotification.SiteId = 1;
                        if (Comment.Url.ToLower().Contains("dms"))
                            UserNotification.SiteId = 2;
                        if (Comment.Url.ToLower().Contains("crm"))
                            UserNotification.SiteId = 3;
                    }
                    UserNotifications.Add(UserNotification);

                    AppUserCommentMapping AppUserCommentMapping = new AppUserCommentMapping
                    {
                        AppUserId = AppUser.Id
                    };
                    AppUserCommentMappings.Add(AppUserCommentMapping);
                }

                if (Comment.AppUserCommentMappings == null)
                {
                    Comment.AppUserCommentMappings = new List<AppUserCommentMapping>();
                    Comment.AppUserCommentMappings.AddRange(AppUserCommentMappings);
                }
                else
                    Comment.AppUserCommentMappings.AddRange(AppUserCommentMappings);
                await UserNotificationService.BulkCreate(UserNotifications);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
