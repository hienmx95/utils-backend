using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;

namespace Utils.Entities
{
    public class ZaloFollowerProfile : DataEntity
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public long Gender { get; set; }
        public long UserIdByApp { get; set; }
        public string DisplayName { get;set; }
        public DateTime BirthDate { get; set; }
        public string Avatar { get; set; }
        public List<ZaloTag> Tags { get; set; }
    }
}
