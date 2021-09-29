using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Utils.Common
{
    public class SystemLog : DataEntity, IEquatable<SystemLog>
    {
        public long Id { get; set; }
        public long? AppUserId { get; set; }
        public string AppUser { get; set; }
        public string Exception { get; set; }
        public string ModuleName { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public DateTime Time { get; set; }
        public Guid RowId { get; set; }
        public bool Equals(SystemLog other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class MongoSystemLog : DataEntity, IEquatable<MongoSystemLog>
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public long? AppUserId { get; set; }
        public string AppUser { get; set; }
        public string Exception { get; set; }
        public string ModuleName { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public DateTime Time { get; set; }
        public Guid RowId { get; set; }
        public bool Equals(MongoSystemLog other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class SystemLogFilter : FilterEntity
    {
        public IdFilter AppUserId { get; set; }
        public StringFilter AppUser { get; set; }
        public StringFilter Exception { get; set; }
        public StringFilter ModuleName { get; set; }
        public StringFilter ClassName { get; set; }
        public StringFilter MethodName { get; set; }
        public DateFilter Time { get; set; }
        public SystemLogOrder OrderBy { get; set; }
        public SystemLogSelect Selects { get; set; }
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SystemLogOrder
    {
        AppUserId = 1,
        AppUser = 2,
        Exception = 3,
        ModuleName = 5,
        ClassName = 6,
        MethodName = 7,
        Time = 8,
    }

    [Flags]
    public enum SystemLogSelect : long
    {
        ALL = E.ALL,
        AppUserId = E._0,
        AppUser = E._1,
        Exception = E._2,
        ModuleName = E._3,
        ClassName = E._4,
        MethodName = E._5,
        Time = E._6,
        Id = E._7,
    }
}
