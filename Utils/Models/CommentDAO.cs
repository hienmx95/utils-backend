using System;
using System.Collections.Generic;

namespace Utils.Models
{
    public partial class CommentDAO
    {
        public CommentDAO()
        {
            AppUserCommentMappings = new HashSet<AppUserCommentMappingDAO>();
        }

        public long Id { get; set; }
        public Guid DiscussionId { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
        public long CreatorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual AppUserDAO Creator { get; set; }
        public virtual ICollection<AppUserCommentMappingDAO> AppUserCommentMappings { get; set; }
    }
}
