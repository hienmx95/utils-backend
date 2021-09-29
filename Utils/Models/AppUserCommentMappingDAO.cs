using System;
using System.Collections.Generic;

namespace Utils.Models
{
    public partial class AppUserCommentMappingDAO
    {
        public long AppUserId { get; set; }
        public long CommentId { get; set; }

        public virtual AppUserDAO AppUser { get; set; }
        public virtual CommentDAO Comment { get; set; }
    }
}
