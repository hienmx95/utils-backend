using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;
using Utils.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using Utils.Entities;
using Utils.Services.MConversation;
using Utils.Services.MConversationMessage;
using Utils.Services.MConversationType;
using Utils.Services.MGlobalUser;

namespace Utils.Rpc.conversation
{
    public class ConversationRoute : Root
    {
        public const string Parent = Module + "/conversation";
        public const string Master = Module + "/conversation/conversation-master";
        public const string Detail = Module + "/conversation/conversation-detail";
        public const string Preview = Module + "/conversation/conversation-preview";
        private const string Default = Rpc + Module + "/conversation";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-template";
        public const string BulkDelete = Default + "/bulk-delete";
        
        public const string SingleListConversationMessage = Default + "/single-list-conversation-message";
        public const string SingleListConversationType = Default + "/single-list-conversation-type";
        public const string SingleListGlobalUser = Default + "/single-list-global-user";


        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
        };

      
        private static List<string> SingleList = new List<string> { 
            SingleListConversationMessage, SingleListConversationType, SingleListGlobalUser, 
        };
        private static List<string> CountList = new List<string> { 
            
        };
        
        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "Tìm kiếm", new List<string> { 
                    Parent,
                    Master, Preview, Count, List,
                    Get,  
                }
            },
            { "Thêm", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    Detail, Create, 
                }.Concat(SingleList).Concat(CountList)
            },

            { "Sửa", new List<string> { 
                    Parent,            
                    Master, Preview, Count, List, Get,
                    Detail, Update, 
                }.Concat(SingleList).Concat(CountList)
            },

            { "Xoá", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    Delete, 
                }.Concat(SingleList)
            },
        };
    }
}
