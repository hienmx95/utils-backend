using System;
using System.Collections.Generic;

namespace Utils.Models
{
    public partial class FileDAO
    {
        public FileDAO()
        {
            ChatMessages = new HashSet<ChatMessageDAO>();
        }

        public long Id { get; set; }
        public string GridId { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public bool IsFile { get; set; }
        public string Path { get; set; }
        public long Level { get; set; }
        public Guid RowId { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<ChatMessageDAO> ChatMessages { get; set; }
    }
}
