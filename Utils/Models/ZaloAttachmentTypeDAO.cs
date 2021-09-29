using System;
using System.Collections.Generic;

namespace Utils.Models
{
    public partial class ZaloAttachmentTypeDAO
    {
        public ZaloAttachmentTypeDAO()
        {
            ZaloAttachments = new HashSet<ZaloAttachmentDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ZaloAttachmentDAO> ZaloAttachments { get; set; }
    }
}
