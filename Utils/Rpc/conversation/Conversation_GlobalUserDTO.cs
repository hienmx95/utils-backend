using Utils.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Utils.Entities;

namespace Utils.Rpc.conversation
{
    public class Conversation_GlobalUserDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Username { get; set; }
        
        public string DisplayName { get; set; }
        
        public Guid RowId { get; set; }
        

        public Conversation_GlobalUserDTO() {}
        public Conversation_GlobalUserDTO(GlobalUser GlobalUser)
        {
            
            this.Id = GlobalUser.Id;
            
            this.Username = GlobalUser.Username;
            
            this.DisplayName = GlobalUser.DisplayName;
            
            this.RowId = GlobalUser.RowId;
            
            this.Errors = GlobalUser.Errors;
        }
    }

    public class Conversation_GlobalUserFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Username { get; set; }
        
        public StringFilter DisplayName { get; set; }
        
        public GuidFilter RowId { get; set; }
        
        public GlobalUserOrder OrderBy { get; set; }
    }
}