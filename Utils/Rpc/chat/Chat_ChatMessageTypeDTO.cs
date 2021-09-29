using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Entities;

namespace Utils.Rpc.chat
{
    public class Chat_ChatMessageTypeDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public Chat_ChatMessageTypeDTO(ChatMessageType ChatMessageType)
        {
            this.Id = ChatMessageType.Id;
            this.Code = ChatMessageType.Code;
            this.Name = ChatMessageType.Name;
        }
    }
}
