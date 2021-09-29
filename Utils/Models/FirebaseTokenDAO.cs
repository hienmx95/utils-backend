using System;
using System.Collections.Generic;

namespace Utils.Models
{
    public partial class FirebaseTokenDAO
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public string Token { get; set; }
        public string DeviceModel { get; set; }
        public string OsName { get; set; }
        public string OsVersion { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual AppUserDAO AppUser { get; set; }
    }
}
