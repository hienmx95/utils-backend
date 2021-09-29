using Utils.Common;
using Utils.Helpers;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Entities;
using Utils.Models;
using Utils.Repositories;

namespace Utils.Handlers
{
    public class MailHandler : Handler
    {
        private string SyncKey => $"{Name}.Send";
        public override string Name => nameof(Mail);

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
            List<Mail> Mails = JsonConvert.DeserializeObject<List<Mail>>(json);
            await UOW.MailRepository.BulkMerge(Mails);
        }
    }
}
