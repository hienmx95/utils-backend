using Utils.Entities;
using Utils.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;
using System.Runtime.CompilerServices;
using Utils.Helpers;
using Microsoft.Extensions.Configuration;
using Utils.Repositories;

namespace Utils.Handlers
{
    public interface IHandler
    {
        string Name { get; }
        IRabbitManager RabbitManager { get; set; }
        void QueueBind(IModel channel, string queue, string exchange);
        Task Handle(IUOW UOW, string routingKey, string content);
        Task Handle(IConfiguration Configuration, string routingKey, string content);
    }

    public abstract class Handler : IHandler
    {
        public abstract string Name { get; }
        public IRabbitManager RabbitManager { get; set; }
        public async virtual Task Handle(IUOW UOW, string routingKey, string content) { }
        public async virtual Task Handle(IConfiguration Configuration, string routingKey, string content) { }

        public abstract void QueueBind(IModel channel, string queue, string exchange);

        protected void Log(Exception ex, string className, [CallerMemberName] string methodName = "")
        {
            Console.WriteLine(ex.ToString());
            SystemLog SystemLog = new SystemLog
            {
                AppUserId = null,
                AppUser = "RABBITMQ",
                ClassName = className,
                MethodName = methodName,
                ModuleName = StaticParams.ModuleName,
                Exception = ex.ToString(),
                Time = StaticParams.DateTimeNow,
            };
            GenericEnum SystemLogSend = new GenericEnum { Code = "SystemLog.Send", Name = "SystemLog.Send" };
            RabbitManager.PublishSingle(SystemLog, SystemLogSend);
        }
    }
}
