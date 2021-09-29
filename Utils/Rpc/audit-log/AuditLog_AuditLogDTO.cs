using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;

namespace Utils.Rpc.audit_log
{
    public class AuditLog_AuditLogDTO : DataDTO
    {
        public string Id { get; set; }
        public long? AppUserId { get; set; }
        public string AppUser { get; set; }
        public string OldData { get; set; }
        public string NewData { get; set; }
        public string ModuleName { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public DateTime Time { get; set; }
        public AuditLog_AuditLogDTO() { }
        public AuditLog_AuditLogDTO(MongoAuditLog MongoAuditLog)
        {
            this.Id = MongoAuditLog.Id;
            this.AppUser = MongoAuditLog.AppUser;
            this.AppUserId = MongoAuditLog.AppUserId;
            this.OldData = MongoAuditLog.OldData;
            this.NewData = MongoAuditLog.NewData;
            this.ModuleName = MongoAuditLog.ModuleName;
            this.ClassName = MongoAuditLog.ClassName;
            this.MethodName = MongoAuditLog.MethodName;
            this.Time = MongoAuditLog.Time;
        }
    }
   
    public class AuditLog_AuditLogFilterDTO : FilterDTO
    {
        public IdFilter AppUserId { get; set; }
        public StringFilter AppUser { get; set; }
        public StringFilter OldData { get; set; }
        public StringFilter NewData { get; set; }
        public StringFilter ModuleName { get; set; }
        public StringFilter ClassName { get; set; }
        public StringFilter MethodName { get; set; }
        public DateFilter Time { get; set; }
        public AuditLogOrder OrderBy { get; set; }
        internal bool HasValue => (AppUserId != null && AppUserId.HasValue) ||
            (AppUser != null && AppUser.HasValue) ||
            (OldData != null && OldData.HasValue) ||
            (NewData != null && NewData.HasValue) ||
            (ModuleName != null && ModuleName.HasValue) ||
            (ClassName != null && ClassName.HasValue) ||
            (MethodName != null && MethodName.HasValue) ||
            (Time != null && Time.HasValue);
    }
}
