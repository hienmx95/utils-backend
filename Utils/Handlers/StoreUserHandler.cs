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

namespace Utils.Handlers
{
    public class StoreUserHandler : Handler
    {
        private string SyncKey =>  $"{Name}.Sync";
        public override string Name => nameof(StoreUser);

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"{Name}.*", null);
        }
        public override async Task Handle(IUOW UOW, string routingKey, string content)
        {
            if (routingKey == SyncKey)
                await Sync(UOW, content);
        }

        private async Task Sync(IUOW UOW, string json)
        {
            try
            {
                List<StoreUser> StoreUsers = JsonConvert.DeserializeObject<List<StoreUser>>(json);
                List<GlobalUser> GlobalUsers = StoreUsers.Select(x => new GlobalUser
                {
                    RowId = x.RowId,
                    Username = x.Username,
                    DisplayName = x.DisplayName,
                }).ToList();
                await UOW.GlobalUserRepository.BulkMerge(GlobalUsers);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(StoreUserHandler));
            }
        }
    }
}
