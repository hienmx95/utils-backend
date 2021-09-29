using Utils.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Entities;
using Utils.Repositories;
using Utils.Helpers;

namespace Utils.Service
{
    public interface IAuditLogService : IServiceScoped
    {
        Task<long> Count(AuditLogFilter AuditLogFilter);
        Task<List<MongoAuditLog>> List(AuditLogFilter AuditLogFilter);
        Task<MongoAuditLog> Get(string Id);
        Task<MongoAuditLog> Delete(MongoAuditLog Auditlog);
        Task<bool> BulkDelete(AuditLogFilter AuditLogFilter);
    }
    public class AuditLogService : IAuditLogService
    {
        public IUOW UOW;
        public ILogging Logging;
        public AuditLogService(IUOW UOW, ILogging Logging)
        {
            this.UOW = UOW;
            this.Logging = Logging;
        }

        public async Task<long> Count(AuditLogFilter AuditLogFilter)
        {
            try
            {
                return await UOW.AuditLogRepository.Count(AuditLogFilter);
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(AuditLogService));
            }
            return 0;
        }
        public async Task<List<MongoAuditLog>> List(AuditLogFilter AuditLogFilter)
        {
            try
            {
                return await UOW.AuditLogRepository.List(AuditLogFilter);
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(AuditLogService));
            }
            return null;
        }
        public async Task<MongoAuditLog> Get(string Id)
        {
            return await UOW.AuditLogRepository.Get(Id);
        }
        public async Task<MongoAuditLog> Delete(MongoAuditLog Auditlog)
        {
            try
            {
                await UOW.AuditLogRepository.Delete(Auditlog);
                return Auditlog;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(AuditLogService));
            }
            return null;
        }
        public async Task<bool> BulkDelete(AuditLogFilter AuditLogFilter)
        {
            try
            {
                return await UOW.AuditLogRepository.BulkDelete(AuditLogFilter);
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(AuditLogService));
            }
            return false;
        }
    }
}
