using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Utils.Common
{
    public interface IMongoDBContext
    {
        //IMongoCollection<T> GetCollection(string collection);
    }
    public class MongoDBContext : IMongoDBContext
    {
        private IMongoDatabase Db { get; set; }
        private MongoClient MongoClient { get; set; }
        public MongoDBContext(IConfiguration Configuration)
        {
            MongoClient = new MongoClient(Configuration["MongoConnection:ConnectionString"]);
            Db = MongoClient.GetDatabase(Configuration["MongoConnection:Database"]);
        }

        //public IMongoCollection<T> GetCollection(string collection)
        //{
        //    return Db.GetCollection<T>(collection);
        //}
    }
}
