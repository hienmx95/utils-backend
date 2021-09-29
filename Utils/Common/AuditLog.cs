using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Utils.Common
{
    public class AuditLog : DataEntity, IEquatable<AuditLog>
    {
        public long Id { get; set; }
        public long? AppUserId { get; set; }
        public string AppUser { get; set; }
        public string OldData { get; set; }
        public string NewData { get; set; }
        public string ModuleName { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public DateTime Time { get; set; }
        public bool Equals(AuditLog other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class MongoAuditLog : DataEntity, IEquatable<MongoAuditLog>
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public long? AppUserId { get; set; }
        public string AppUser { get; set; }
        public string OldData { get; set; }
        public string NewData { get; set; }
        public string ModuleName { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public DateTime Time { get; set; }
        public bool Equals(MongoAuditLog other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class AuditLogFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter AppUserId { get; set; }
        public StringFilter AppUser { get; set; }
        public StringFilter OldData { get; set; }
        public StringFilter NewData { get; set; }
        public StringFilter ModuleName { get; set; }
        public StringFilter ClassName { get; set; }
        public StringFilter MethodName { get; set; }
        public DateFilter Time { get; set; }
        public AuditLogOrder OrderBy {get; set;}
        public AuditLogSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AuditLogOrder
    {
        AppUserId = 1,
        AppUser = 2,
        OldData = 3,
        NewData = 4,
        ModuleName = 5,
        ClassName = 6,
        MethodName = 7,
        Time = 8,
    }

    [Flags]
    public enum AuditLogSelect : long
    {
        ALL = E.ALL,
        Id = E._0,
        AppUserId = E._1,
        OldData = E._2,
        NewData = E._3,
        ModuleName = E._4,
        ClassName = E._5,
        MethodName = E._6,
        Time = E._7,
        AppUser = E._8
    }
}
