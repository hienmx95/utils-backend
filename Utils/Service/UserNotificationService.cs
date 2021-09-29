using Utils.Common;
using Utils.Entities;
using Utils.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils.Hub;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Utils.Helpers;
using FirebaseAdmin.Messaging;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;

namespace Utils.Service
{
    public interface IUserNotificationService : IServiceScoped
    {
        Task<int> Count(UserNotificationFilter filter);
        Task<List<AppUserNotification>> List(UserNotificationFilter filter);
        Task<AppUserNotification> Get(long Id);
        Task<AppUserNotification> Create(AppUserNotification notification);
        Task<List<AppUserNotification>> BulkCreate(List<AppUserNotification> notification);
        Task Read(long UserNotificationId);
        Task<bool> Delete(long Id);
    }
    public class UserNotificationService : IUserNotificationService
    {
        private readonly IUOW UOW;
        private ICurrentContext CurrentContext;
        protected IHubContext<UserNotificationHub> signalR;

        public UserNotificationService(IUOW UOW, IHubContext<UserNotificationHub> signalR, ICurrentContext CurrentContext)
        {
            this.signalR = signalR;
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<int> Count(UserNotificationFilter filter)
        {
            return await UOW.UserNotificationRepository.Count(filter);
        }

        public async Task<List<AppUserNotification>> List(UserNotificationFilter filter)
        {
            return await UOW.UserNotificationRepository.List(filter);
        }

        public async Task<AppUserNotification> Get(long Id)
        {
            if (Id == 0) return null;
            return await UOW.UserNotificationRepository.Get(Id);
        }

        public async Task<AppUserNotification> Create(AppUserNotification UserNotification)
        {
            if (UserNotification == null) return null;
            try
            {
                await UOW.Begin();
                await UOW.UserNotificationRepository.Create(UserNotification);
                await UOW.Commit();
                return await Get(UserNotification.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                throw new MessageException(ex);
            }
        }

        public async Task<List<AppUserNotification>> BulkCreate(List<AppUserNotification> AppUserNotifications)
        {
            if (AppUserNotifications == null) return null;
            try
            {
                await UOW.UserNotificationRepository.BulkCreate(AppUserNotifications);
                List<long> Ids = AppUserNotifications.Select(u => u.Id).ToList();
                AppUserNotifications = await UOW.UserNotificationRepository.List(new UserNotificationFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Id = new IdFilter { In = Ids }
                });
                var Sender = await UOW.AppUserRepository.Get(AppUserNotifications.Select(x => x.SenderId).FirstOrDefault());
                List<long> RecipientIds = AppUserNotifications.Select(x => x.RecipientId).ToList();
                List<AppUser> Recipients = await UOW.AppUserRepository.List(new AppUserFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = AppUserSelect.ALL,
                    Id = new IdFilter { In = RecipientIds }
                });
                foreach (AppUserNotification AppUserNotification in AppUserNotifications)
                {
                    AppUser Recipient = Recipients.Where(x => x.Id == AppUserNotification.RecipientId).FirstOrDefault();
                    _ = signalR.Clients.User(Recipient.RowId.ToString()).SendAsync("Receive", AppUserNotification);
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add(nameof(AppUserNotification.Id), AppUserNotification.Id.ToString());
                    data.Add(nameof(AppUserNotification.SiteId), AppUserNotification.SiteId.ToString());
                    data.Add(nameof(AppUserNotification.ContentMobile), AppUserNotification.ContentMobile);
                    data.Add(nameof(AppUserNotification.ContentWeb), AppUserNotification.ContentWeb);
                    data.Add(nameof(AppUserNotification.LinkWebsite), AppUserNotification.LinkWebsite);
                    data.Add(nameof(AppUserNotification.LinkMobile), AppUserNotification.LinkMobile);
                    data.Add(nameof(AppUserNotification.Sender), AppUserNotification.Sender?.DisplayName);
                    data.Add(nameof(AppUserNotification.Unread), AppUserNotification.Unread.ToString());
                    data.Add(nameof(AppUserNotification.Time), AppUserNotification.Time.ToString("yyyy-MM-dd hh:mm:ss"));

                    if (AppUserNotification.Recipient.Tokens != null)
                    {
                        foreach (string Token in AppUserNotification.Recipient.Tokens)
                        {
                            var message = new Message()
                            {
                                Notification = new Notification
                                {
                                    Title = AppUserNotification.TitleMobile,
                                    Body = AppUserNotification.ContentMobile,
                                },
                                Data = data,
                                Token = Token,
                            };
                            var messaging = FirebaseMessaging.DefaultInstance;
                            _ = messaging.SendAsync(message);
                        }
                    }
                }

                return AppUserNotifications;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                throw new MessageException(ex);
            }
        }


        public async Task<bool> Delete(long Id)
        {
            if (Id == 0) return false;
            try
            {
                await UOW.Begin();
                await UOW.UserNotificationRepository.Delete(Id);
                await UOW.Commit();
                return true;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                throw new MessageException(ex);
            }
        }

        public async Task Read(long UserNotificationId)
        {
            try
            {
                await UOW.Begin();
                await UOW.UserNotificationRepository.Read(UserNotificationId);
                await UOW.Commit();
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                throw new MessageException(ex);
            }
        }
    }

}
