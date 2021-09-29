using Utils.Common;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Entities;
using Utils.Models;
using Microsoft.Extensions.Configuration;
using Utils.Repositories;

namespace Utils.Handlers
{
    public class SystemLogHandler : Handler
    {
        private string SendKey => $"{Name}.Send";
        public override string Name => nameof(SystemLog);

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
                List<SystemLog> SystemLogs = JsonConvert.DeserializeObject<List<SystemLog>>(json);
                List<MongoSystemLog> MongoSystemLogs = SystemLogs.Select(x => new MongoSystemLog
                {
                    AppUserId = x.AppUserId,
                    AppUser = x.AppUser,
                    ClassName = x.ClassName,
                    Exception = x.Exception,
                    MethodName = x.MethodName,
                    ModuleName = x.ModuleName,
                    Time = x.Time,
                }).ToList();
                await UOW.SystemLogRepository.BulkMerge(MongoSystemLogs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
