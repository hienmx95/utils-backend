using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;

namespace Utils.Rpc.system_log
{
    public class SystemLog_SystemLogDTO
    {
        public string Id { get; set; }
        public long? AppUserId { get; set; }
        public string AppUser { get; set; }
        public string Exception { get; set; }
        public string ModuleName { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public DateTime Time { get; set; }
        public SystemLog_SystemLogDTO() { }
        public SystemLog_SystemLogDTO(MongoSystemLog MongoSystemLog)
        {
            this.Id = MongoSystemLog.Id;
            this.AppUser = MongoSystemLog.AppUser;
            this.AppUserId = MongoSystemLog.AppUserId;
            this.Exception = MongoSystemLog.Exception;
            this.ModuleName = MongoSystemLog.ModuleName;
            this.ClassName = MongoSystemLog.ClassName;
            this.MethodName = MongoSystemLog.MethodName;
            this.Time = MongoSystemLog.Time;
        }
    }
    public class SystemLog_SystemLogFilterDTO : FilterDTO
    {
        public IdFilter AppUserId { get; set; }
        public StringFilter AppUser { get; set; }
        public StringFilter Exception { get; set; }
        public StringFilter ModuleName { get; set; }
        public StringFilter ClassName { get; set; }
        public StringFilter MethodName { get; set; }
        public DateFilter Time { get; set; }
        public SystemLogOrder OrderBy { get; set; }
        internal bool HasValue => (AppUserId != null && AppUserId.HasValue) &&
            (AppUser != null && AppUser.HasValue) ||
            (Exception != null && Exception.HasValue) ||
            (ModuleName != null && ModuleName.HasValue) ||
            (ClassName != null && ClassName.HasValue) ||
            (MethodName != null && MethodName.HasValue) ||
            (Time != null && Time.HasValue);
    }
}
