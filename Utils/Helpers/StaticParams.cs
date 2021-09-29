using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Utils.Helpers
{
    public class StaticParams
    {
        public static DateTime DateTimeNow => DateTime.UtcNow;
        public static DateTime DateTimeMin => DateTime.MinValue;
        public static string ExcelFileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public static string SecretKey { get; set; }
        public static long RetryCount = 10;
        public static string ModuleName = "Utils";
        public static string FirebaseCredential = "";
    }
}
