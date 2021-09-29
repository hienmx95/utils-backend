using Utils.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Utils.Entities
{
    public class AppUserCommentMapping : DataEntity
    {
        public long AppUserId { get; set; }
        public long CommentId { get; set; }
        public AppUser AppUser { get; set; }
        public Comment Comment { get; set; }
    }
}
