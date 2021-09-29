using System;
using System.Collections.Generic;

namespace Utils.Models
{
    public partial class FacebookPageDAO
    {
        public FacebookPageDAO()
        {
            FacebookMessages = new HashSet<FacebookMessageDAO>();
        }

        public long Id { get; set; }
        public string PageId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<FacebookMessageDAO> FacebookMessages { get; set; }
    }
}
