using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;

namespace Utils.Enums
{
    public static class ZaloAttachmentTypeEnum
    {
        public static GenericEnum image = new GenericEnum { Id = 1, Code = "image", Name = "image" };
        public static GenericEnum sticker = new GenericEnum { Id = 2, Code = "sticker", Name = "sticker" };
        public static GenericEnum file = new GenericEnum { Id = 3, Code = "file", Name = "file" };
        public static List<GenericEnum> ZaloAttachmentTypeEnumList = new List<GenericEnum>
        {
            image,sticker,file
        };
    }
}
