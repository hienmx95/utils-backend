using Utils.Common;
using Utils.Entities;
using Utils.Models;
using Utils.Repositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.SignalR;
using Utils.Hub;

namespace Utils.Handlers
{
    public class AppUserNotificationHandler : Handler
    {
        private string SendKey => $"{Name}.Send";
        public override string Name => nameof(AppUserNotification);
        public IHubContext<UserNotificationHub> SignalR;

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"{Name}.*", null);
        }
        public override async Task Handle(IUOW UOW, string routingKey, string content)
        {
            if (routingKey == SendKey)
                await Send(UOW, content);
        }

        private async Task Send(IUOW UOW, string json)
        {
            try
            {
                List<AppUserNotification> AppUserNotifications = JsonConvert.DeserializeObject<List<AppUserNotification>>(json);
                AppUserNotifications.ForEach(x => x.Unread = true);
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
                    _ = SignalR.Clients.User(Recipient.RowId.ToString()).SendAsync("Receive", AppUserNotification);
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
            }
            catch (Exception ex)
            {
                Log(ex, nameof(AppUserNotificationHandler));
            }
        }
    }
}
