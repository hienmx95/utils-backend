using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;

namespace Utils.Enums
{
    public class ChatMessageTypeEnum
    {
        public static GenericEnum text = new GenericEnum { Id = 1, Code = "text", Name = "text" };
        public static GenericEnum file = new GenericEnum { Id = 2, Code = "file", Name = "file" };

        public static List<GenericEnum> ChatMessageTypeEnumList = new List<GenericEnum>
        {
            text, file
        };
    }
}
