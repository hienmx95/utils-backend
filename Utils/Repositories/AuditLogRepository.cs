using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;

namespace Utils.Repositories
{
    public interface IAuditLogRepository
    {
        Task<long> Count(AuditLogFilter AuditLogFilter);
        Task<List<MongoAuditLog>> List(AuditLogFilter AuditLogFilter);
        Task<MongoAuditLog> Get(string Id);
        Task<bool> BulkMerge(List<MongoAuditLog> MongoAuditLog);
        Task<bool> Delete(MongoAuditLog AuditLog);
        Task<bool> BulkDelete(AuditLogFilter AuditLogFilter);
    }
    public class AuditLogRepository : IAuditLogRepository
    {
        protected IMongoDatabase MongoDatabase;
        protected IMongoCollection<MongoAuditLog> Collection;
        string DatabaseName = "UTILS";
        public AuditLogRepository(IConfiguration Configuration, IMongoClient MongoClient)
        {
            this.MongoDatabase = MongoClient.GetDatabase(DatabaseName);
            this.Collection = MongoDatabase.GetCollection<MongoAuditLog>(nameof(MongoAuditLog));
        }

        private FilterDefinition<MongoAuditLog> DynamicFilter(FilterDefinition<MongoAuditLog> BuilderFilter, AuditLogFilter filter)
        {
            BuilderFilter = BuilderFilter.MgWhere(q => q.AppUserId, filter.AppUserId);
            BuilderFilter = BuilderFilter.MgWhere(q => q.AppUser, filter.AppUser);
            BuilderFilter = BuilderFilter.MgWhere(q => q.ClassName, filter.ClassName);
            BuilderFilter = BuilderFilter.MgWhere(q => q.OldData, filter.OldData);
            BuilderFilter = BuilderFilter.MgWhere(q => q.NewData, filter.NewData);
            BuilderFilter = BuilderFilter.MgWhere(q => q.ModuleName, filter.ModuleName);
            BuilderFilter = BuilderFilter.MgWhere(q => q.MethodName, filter.MethodName);
            BuilderFilter = BuilderFilter.MgWhere(q => q.Time, filter.Time);
            return BuilderFilter;
        }

        public async Task<long> Count(AuditLogFilter filter)
        {
            FilterDefinition<MongoAuditLog> BuilderFilter = Builders<MongoAuditLog>.Filter.Empty;
            BuilderFilter = DynamicFilter(BuilderFilter, filter);
            return await Collection.CountDocumentsAsync(BuilderFilter);
        }

        public async Task<List<MongoAuditLog>> List(AuditLogFilter filter)
        {
            if (filter == null) return new List<MongoAuditLog>();
            FilterDefinition<MongoAuditLog> BuilderFilter = Builders<MongoAuditLog>.Filter.Empty;
            BuilderFilter = DynamicFilter(BuilderFilter, filter);

            List<MongoAuditLog> MongoAuditLogs = await Collection.Find(BuilderFilter)
                .SortByDescending(x => x.Time)
                .Skip(filter.Skip)
                .Limit(filter.Take)
                .ToListAsync();
            return MongoAuditLogs;
        }

        public async Task<MongoAuditLog> Get(string Id)
        {
            FilterDefinition<MongoAuditLog> BuilderFilter = Builders<MongoAuditLog>.Filter.Empty;
            StringFilter StringFilter = new StringFilter
            {
                Equal = Id
            };
            BuilderFilter = BuilderFilter.MgWhere(x => x.Id, StringFilter);
            MongoAuditLog MongoAuditLog = await Collection
                .Find(BuilderFilter)
                .FirstOrDefaultAsync();

            return MongoAuditLog;
        }

        public async Task<bool> BulkMerge(List<MongoAuditLog> MongoAuditLog)
        {
            await Collection.InsertManyAsync(MongoAuditLog);
            return true;
        }

        public async Task<bool> Delete(MongoAuditLog MongoAuditLog)
        {
            await Collection.DeleteOneAsync(x => x.Id == MongoAuditLog.Id);
            return true;
        }

        public async Task<bool> BulkDelete(AuditLogFilter filter)
        {
            FilterDefinition<MongoAuditLog> BuilderFilter = Builders<MongoAuditLog>.Filter.Empty;
            BuilderFilter = DynamicFilter(BuilderFilter, filter);
            await Collection.DeleteManyAsync(BuilderFilter);
            return true;
        }
    }
}
