using Utils.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Utils.Rpc.user
{
    public class AppUser_FirebaseTokenDTO: DataDTO
    {
        public string Token { get; set; }
        public string DeviceModel { get; set; }
        public string OsName { get; set; }
        public string OsVersion { get; set; }
    }
}
