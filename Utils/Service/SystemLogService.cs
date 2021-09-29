using Utils.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Entities;
using Utils.Repositories;

namespace Utils.Service
{
    public interface ISystemLogService : IServiceScoped
    {
        Task<long> Count(SystemLogFilter SystemLogFilter);
        Task<List<MongoSystemLog>> List(SystemLogFilter SystemLogFilter);
        Task<MongoSystemLog> Get(string Id);
        Task<MongoSystemLog> Delete(MongoSystemLog Systemlog);
        Task<bool> BulkDelete(SystemLogFilter SystemLogFilter);
    }
    public class SystemLogService : ISystemLogService
    {
        public IUOW UOW;
        public SystemLogService(IUOW UOW)
        {
            this.UOW = UOW;
        }

        public async Task<long> Count(SystemLogFilter SystemLogFilter)
        {
            return await UOW.SystemLogRepository.Count(SystemLogFilter);
        }
        public async Task<List<MongoSystemLog>> List(SystemLogFilter SystemLogFilter)
        {
            return await UOW.SystemLogRepository.List(SystemLogFilter);
        }
        public async Task<MongoSystemLog> Get(string Id)
        {
            return await UOW.SystemLogRepository.Get(Id);
        }
        public async Task<MongoSystemLog> Delete(MongoSystemLog Systemlog)
        {
            try
            {
                await UOW.SystemLogRepository.Delete(Systemlog);
                return Systemlog;
            }
            catch (Exception ex)
            {
            }
            return null;
        }
        public async Task<bool> BulkDelete(SystemLogFilter SystemLogFilter)
        {
            try
            {
                return await UOW.SystemLogRepository.BulkDelete(SystemLogFilter);
            }
            catch (Exception ex)
            {
            }
            return false;
        }
    }
}
