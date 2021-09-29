using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;

namespace Utils.Repositories
{
    public interface ISystemLogRepository
    {
        Task<long> Count(SystemLogFilter SystemLogFilter);
        Task<List<MongoSystemLog>> List(SystemLogFilter SystemLogFilter);
        Task<MongoSystemLog> Get(string Id);
        Task<bool> BulkMerge(List<MongoSystemLog> MongoSystemLogs);
        Task<bool> Delete(MongoSystemLog SystemLog);
        Task<bool> BulkDelete(SystemLogFilter SystemLogFilter);
    }
    public class SystemLogRepository : ISystemLogRepository
    {
        protected IMongoDatabase MongoDatabase;
        protected IMongoCollection<MongoSystemLog> Collection;
        string DatabaseName = "UTILS";
        public SystemLogRepository(IConfiguration Configuration, IMongoClient MongoClient)
        {
            this.MongoDatabase = MongoClient.GetDatabase(DatabaseName);
            this.Collection = MongoDatabase.GetCollection<MongoSystemLog>(nameof(MongoSystemLog));
        }

        private FilterDefinition<MongoSystemLog> DynamicFilter(FilterDefinition<MongoSystemLog> BuilderFilter, SystemLogFilter filter)
        {
            BuilderFilter = BuilderFilter.MgWhere(q => q.AppUserId, filter.AppUserId);
            BuilderFilter = BuilderFilter.MgWhere(q => q.AppUser, filter.AppUser);
            BuilderFilter = BuilderFilter.MgWhere(q => q.ClassName, filter.ClassName);
            BuilderFilter = BuilderFilter.MgWhere(q => q.Exception, filter.Exception);
            BuilderFilter = BuilderFilter.MgWhere(q => q.ModuleName, filter.ModuleName);
            BuilderFilter = BuilderFilter.MgWhere(q => q.MethodName, filter.MethodName);
            BuilderFilter = BuilderFilter.MgWhere(q => q.Time, filter.Time);
            return BuilderFilter;
        }

        public async Task<long> Count(SystemLogFilter filter)
        {
            FilterDefinition<MongoSystemLog> BuilderFilter = Builders<MongoSystemLog>.Filter.Empty;
            BuilderFilter = DynamicFilter(BuilderFilter, filter);
            return await Collection.CountDocumentsAsync(BuilderFilter);
        }

        public async Task<List<MongoSystemLog>> List(SystemLogFilter filter)
        {
            if (filter == null) return new List<MongoSystemLog>();
            FilterDefinition<MongoSystemLog> BuilderFilter = Builders<MongoSystemLog>.Filter.Empty;
            BuilderFilter = DynamicFilter(BuilderFilter, filter);

            List<MongoSystemLog> MongoSystemLogs = await Collection.Find(BuilderFilter)
                .SortByDescending(x => x.Time)
                .Skip(filter.Skip)
                .Limit(filter.Take)
                .ToListAsync();
            return MongoSystemLogs;
        }

        public async Task<MongoSystemLog> Get(string Id)
        {
            FilterDefinition<MongoSystemLog> BuilderFilter = Builders<MongoSystemLog>.Filter.Empty;
            StringFilter StringFilter = new StringFilter
            {
                Equal = Id
            };
            BuilderFilter = BuilderFilter.MgWhere(x => x.Id, StringFilter);
            MongoSystemLog MongoSystemLog = await Collection
                .Find(BuilderFilter)
                .FirstOrDefaultAsync();

            return MongoSystemLog;
        }

        public async Task<bool> BulkMerge(List<MongoSystemLog> MongoSystemLogs)
        {
            await Collection.InsertManyAsync(MongoSystemLogs);
            return true;
        }
        public async Task<bool> Delete(MongoSystemLog MongoSystemLog)
        {
            await Collection.DeleteOneAsync(x => x.Id == MongoSystemLog.Id);
            return true;
        }

        public async Task<bool> BulkDelete(SystemLogFilter filter)
        {
            FilterDefinition<MongoSystemLog> BuilderFilter = Builders<MongoSystemLog>.Filter.Empty;
            BuilderFilter = DynamicFilter(BuilderFilter, filter);
            await Collection.DeleteManyAsync(BuilderFilter);
            return true;
        }
    }
}
