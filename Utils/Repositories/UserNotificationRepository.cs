using Microsoft.EntityFrameworkCore;
using Utils.Common;
using Utils.Entities;
using Utils.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;

namespace Utils.Repositories
{
    public interface IUserNotificationRepository
    {
        Task<int> Count(UserNotificationFilter filter);
        Task<List<AppUserNotification>> List(UserNotificationFilter filter);
        Task<AppUserNotification> Get(long Id);
        Task<bool> Create(AppUserNotification notification);
        Task<bool> Read(long Id);
        Task<bool> Delete(long Id);
        Task<bool> BulkCreate(List<AppUserNotification> notifications);
    }
    public class UserNotificationRepository : IUserNotificationRepository
    {
        private readonly DataContext DataContext;
        public UserNotificationRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<UserNotificationDAO> DynamicFilter(IQueryable<UserNotificationDAO> query, UserNotificationFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);

            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.SenderId != null)
                query = query.Where(q => q.SenderId, filter.SenderId);
            if (filter.RecipientId != null)
                query = query.Where(q => q.RecipientId, filter.RecipientId);
            if (filter.Time != null)
                query = query.Where(q => q.Time, filter.Time);
            if (filter.SiteId != null)
                query = query.Where(q => q.SiteId, filter.SiteId);
            if (filter.Unread != null)
                query = query.Where(q => q.Unread == filter.Unread);
            return query;
        }
        private IQueryable<UserNotificationDAO> DynamicOrder(IQueryable<UserNotificationDAO> query, UserNotificationFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case UserNotificationOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case UserNotificationOrder.Time:
                            query = query.OrderBy(q => q.Time);
                            break;
                        default:
                            query = query.OrderBy(q => q.Id);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case UserNotificationOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case UserNotificationOrder.Time:
                            query = query.OrderByDescending(q => q.Time);
                            break;
                        default:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                    }
                    break;
                default:
                    query = query.OrderBy(q => q.Id);
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }
        public async Task<int> Count(UserNotificationFilter filter)
        {
            if (filter == null) return 0;
            IQueryable<UserNotificationDAO> UserNotificationDAOs = DataContext.UserNotification;
            UserNotificationDAOs = DynamicFilter(UserNotificationDAOs, filter);
            return await UserNotificationDAOs.CountAsync();
        }
        public async Task<List<AppUserNotification>> List(UserNotificationFilter filter)
        {
            if (filter == null) return new List<AppUserNotification>();
            IQueryable<UserNotificationDAO> UserNotificationDAOs = DataContext.UserNotification;
            UserNotificationDAOs = DynamicFilter(UserNotificationDAOs, filter);
            UserNotificationDAOs = DynamicOrder(UserNotificationDAOs, filter);
            List<AppUserNotification> UserNotifications = UserNotificationDAOs.Select(n => new AppUserNotification
            {
                SiteId = n.SiteId,
                SenderId = n.SenderId,
                RecipientId = n.RecipientId,
                TitleWeb = n.TitleWeb,
                ContentWeb = n.ContentWeb,
                Id = n.Id,
                Time = n.Time,
                Unread = n.Unread,
                LinkMobile = n.LinkMobile,
                LinkWebsite = n.LinkWebsite,
                Recipient = n.Recipient == null ? null : new AppUser
                {
                    Id = n.Recipient.Id,
                    Username = n.Recipient.Username,
                    DisplayName = n.Recipient.DisplayName,
                    Avatar = n.Recipient.Avatar,
                },
                Sender = n.Sender == null ? null : new AppUser
                {
                    Id = n.Sender.Id,
                    Username = n.Sender.Username,
                    DisplayName = n.Sender.DisplayName,
                    Avatar = n.Sender.Avatar,
                },
            }).ToList();


            List<long> Ids = UserNotifications.Select(a => a.RecipientId).ToList();
            List<FirebaseTokenDAO> FirebaseTokenDAOs = await DataContext.FirebaseToken.Where(f => Ids.Contains(f.AppUserId)).ToListAsync();
            foreach (AppUserNotification UserNotification in UserNotifications)
            {
                if (UserNotification.Recipient.Tokens == null)
                    UserNotification.Recipient.Tokens = new List<string>();
                UserNotification.Recipient.Tokens = FirebaseTokenDAOs
                    .Where(f => f.AppUserId == UserNotification.RecipientId)
                    .Select(f => f.Token)
                    .Distinct()
                    .ToList();
            }

            return UserNotifications;
        }

        public async Task<AppUserNotification> Get(long Id)
        {
            AppUserNotification UserNotification = await DataContext.UserNotification
                .Where(n => n.Id == Id)
                .Select(n => new AppUserNotification
                {
                    SiteId = n.SiteId,
                    SenderId = n.SenderId,
                    RecipientId = n.RecipientId,
                    TitleWeb = n.TitleWeb,
                    ContentWeb = n.ContentWeb,
                    Id = n.Id,
                    Time = n.Time,
                    Unread = n.Unread,
                    LinkMobile = n.LinkMobile,
                    LinkWebsite = n.LinkWebsite,
                    Sender = new AppUser
                    {
                        Id = n.Sender.Id,
                        DisplayName = n.Sender.DisplayName,
                    },
                    Recipient = new AppUser
                    {
                        Id = n.Recipient.Id,
                        DisplayName = n.Recipient.DisplayName,
                    }
                })
                .FirstOrDefaultAsync();
            return UserNotification;
        }

        public async Task<bool> Create(AppUserNotification UserNotification)
        {
            UserNotificationDAO notificationDAO = new UserNotificationDAO
            {
                SiteId = UserNotification.SiteId,
                SenderId = UserNotification.SenderId,
                RecipientId = UserNotification.RecipientId,
                TitleMobile = UserNotification.TitleMobile,
                ContentMobile = UserNotification.ContentMobile,
                TitleWeb = UserNotification.TitleWeb,
                ContentWeb = UserNotification.ContentWeb,
                Time = UserNotification.Time,
                LinkWebsite = UserNotification.LinkWebsite,
                LinkMobile = UserNotification.LinkMobile,
                Unread = true,
            };
            DataContext.UserNotification.Add(notificationDAO);
            await DataContext.SaveChangesAsync();
            UserNotification.Id = notificationDAO.Id;
            return true;
        }

        public async Task<bool> Delete(long Id)
        {
            await DataContext.UserNotification.Where(n => n.Id == Id).DeleteFromQueryAsync();
            return true;
        }

        public async Task<bool> BulkCreate(List<AppUserNotification> UserNotifications)
        {
            UserNotifications.ForEach(u => u.RowId = Guid.NewGuid());
            List<UserNotificationDAO> UserNotificationDAOs = UserNotifications.Select(x => new UserNotificationDAO
            {
                SiteId = x.SiteId,
                SenderId = x.SenderId,
                RecipientId = x.RecipientId,
                TitleMobile = x.TitleMobile,
                ContentMobile = x.ContentMobile,
                TitleWeb = x.TitleWeb,
                ContentWeb = x.ContentWeb,
                Time = x.Time,
                LinkMobile = x.LinkMobile,
                LinkWebsite = x.LinkWebsite,
                Unread = true,
                RowId = x.RowId,
            }).ToList();
            DataContext.UserNotification.AddRange(UserNotificationDAOs);
            await DataContext.SaveChangesAsync();
            foreach (UserNotificationDAO UserNotificationDAO in UserNotificationDAOs)
            {
                AppUserNotification UserNotification = UserNotifications.Where(u => u.RowId == UserNotificationDAO.RowId).FirstOrDefault();
                UserNotification.Id = UserNotificationDAO.Id;
            }
            return true;
        }

        public async Task<bool> Read(long Id)
        {
            await DataContext.UserNotification.Where(n => n.Id == Id)
               .UpdateFromQueryAsync(n => new UserNotificationDAO
               {
                   Unread = false,
               });
            return true;
        }
    }
}
