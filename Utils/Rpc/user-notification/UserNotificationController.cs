using Microsoft.AspNetCore.Mvc;
using Utils.Entities;
using Utils.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;
using Microsoft.AspNetCore.Authorization;

namespace Utils.Rpc.user_notification
{
    public class UserNotificationRoute : Root
    {
        public const string Base = Rpc + Module + "/user-notification";
        public const string Create = Base + "/create";
        public const string Read = Base + "/Read";
        public const string Count = Base + "/count";
        public const string CountUnread = Base + "/count-unread";
        public const string CountRead= Base + "/count-read";
        public const string Get = Base + "/get";
        public const string List = Base + "/list";
        public const string ListUnread = Base + "/list-unread";
        public const string ListRead = Base + "/list-read";
        public const string Delete = Base + "/delete";
        public const string BulkCreate = Base + "/bulk-create";
    }
    public class UserNotificationController : RpcController
    {
        private readonly IUserNotificationService UserNotificationService;
        private readonly ICurrentContext CurrentContext;
        public UserNotificationController(IUserNotificationService UserNotificationService, ICurrentContext CurrentContext)
        {
            this.UserNotificationService = UserNotificationService;
            this.CurrentContext = CurrentContext;
        }

        [Route(UserNotificationRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] UserNotificationFilter filter)
        {
            if (filter == null) filter = new UserNotificationFilter();
            filter.RecipientId = new IdFilter { Equal = CurrentContext.UserId };
            return await UserNotificationService.Count(filter);
        }

        [Route(UserNotificationRoute.CountUnread), HttpPost]
        public async Task<int> CountUnread([FromBody] UserNotificationFilter filter)
        {
            if (filter == null) filter = new UserNotificationFilter();
            filter.RecipientId = new IdFilter { Equal = CurrentContext.UserId };
            filter.Unread = true;
            return await UserNotificationService.Count(filter);
        }

        [Route(UserNotificationRoute.CountRead), HttpPost]
        public async Task<int> CountRead([FromBody] UserNotificationFilter filter)
        {
            if (filter == null) filter = new UserNotificationFilter();
            filter.RecipientId = new IdFilter { Equal = CurrentContext.UserId };
            filter.Unread = false;
            return await UserNotificationService.Count(filter);
        }

        [Route(UserNotificationRoute.List), HttpPost]
        public async Task<List<AppUserNotification>> List([FromBody] UserNotificationFilter filter)
        {
            if (filter == null) filter = new UserNotificationFilter();
            filter.RecipientId = new IdFilter { Equal = CurrentContext.UserId };
            filter.OrderBy = UserNotificationOrder.Time;
            filter.OrderType = OrderType.DESC;
            return await UserNotificationService.List(filter);
        }

        [Route(UserNotificationRoute.ListUnread), HttpPost]
        public async Task<List<AppUserNotification>> ListUnread([FromBody] UserNotificationFilter filter)
        {
            if (filter == null) filter = new UserNotificationFilter();
            filter.RecipientId = new IdFilter { Equal = CurrentContext.UserId };
            filter.Unread = true;
            filter.OrderBy = UserNotificationOrder.Time;
            filter.OrderType = OrderType.DESC;
            return await UserNotificationService.List(filter);
        }

        [Route(UserNotificationRoute.ListRead), HttpPost]
        public async Task<List<AppUserNotification>> ListRead([FromBody] UserNotificationFilter filter)
        {
            if (filter == null) filter = new UserNotificationFilter();
            filter.RecipientId = new IdFilter { Equal = CurrentContext.UserId };
            filter.Unread = false;
            filter.OrderBy = UserNotificationOrder.Id;
            filter.OrderType = OrderType.DESC;
            return await UserNotificationService.List(filter);
        }

        [Route(UserNotificationRoute.Get), HttpPost]
        public async Task<AppUserNotification> Get([FromBody] AppUserNotification UserNotification)
        {
            if (!ModelState.IsValid)
                throw new BindException(UserNotification);
            await CheckPermission(UserNotification);
            return await UserNotificationService.Get(UserNotification.Id);
        }

        [Route(UserNotificationRoute.Create), HttpPost]
        public async Task<ActionResult<AppUserNotification>> Create([FromBody] AppUserNotification UserNotification)
        {
            if (UserNotification == null) UserNotification = new AppUserNotification();
            UserNotification = await UserNotificationService.Create(UserNotification);
            if (UserNotification == null)
                return BadRequest(UserNotification);
            return Ok(UserNotification);
        }

        [Route(UserNotificationRoute.Read), HttpPost]
        public async Task<ActionResult> Read([FromBody] AppUserNotification UserNotification)
        {
            await UserNotificationService.Read(UserNotification.Id);
            return Ok();
        }


        [Route(UserNotificationRoute.Delete), HttpPost]
        public async Task<ActionResult<bool>> Delete([FromBody] AppUserNotification notification)
        {
            if (notification == null) notification = new AppUserNotification();
            await CheckPermission(notification);
            return await UserNotificationService.Delete(notification.Id);
        }

        [Route(UserNotificationRoute.BulkCreate), HttpPost]
        public async Task<ActionResult<AppUserNotification>> BulkCreate([FromBody] List<AppUserNotification> UserNotifications)
        {
            if (UserNotifications == null) UserNotifications = new List<AppUserNotification>();
            UserNotifications = await UserNotificationService.BulkCreate(UserNotifications);
            if (UserNotifications == null)
                return BadRequest(UserNotifications);
            return Ok(UserNotifications);
        }

        private async Task CheckPermission(AppUserNotification notification)
        {
            UserNotificationFilter filter = new UserNotificationFilter
            {
                Id = new IdFilter { Equal = notification.Id },
                SenderId = new IdFilter { Equal = CurrentContext.UserId },
            };
            int count = await UserNotificationService.Count(filter);
            if (count == 0)
                throw new ForbiddenException();
        }

    }
}
